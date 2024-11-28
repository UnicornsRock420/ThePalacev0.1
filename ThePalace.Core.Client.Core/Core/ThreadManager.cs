using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Models.Threads;
using ThePalace.Core.Client.Core.Protocols.Assets;
using ThePalace.Core.Constants;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core
{
#if WINDOWS10_0_17763_0_OR_GREATER
    using Microsoft.Toolkit.Uwp.Notifications;
#endif

    public sealed class ThreadManager : Disposable
    {
        //[DllImport("kernel32.dll")]
        //public static extern bool Beep(int freq, int duration);

        private static readonly Lazy<ThreadManager> _current = new();
        public static ThreadManager Current => _current.Value;

        private DisposableDictionary<ThreadQueues, CmdTask> _threads = new();
        public IReadOnlyDictionary<ThreadQueues, CmdTask> Threads => _threads;

        private Timer _guiTimer = new();

        public ThreadManager()
        {
            this._managedResources.AddRange(
                new IDisposable[]
                {
                    ConnectionManager.Current,
                    NetworkManager.Current,
                    SessionManager.Current,
                    ScriptManager.Current,
                    AssetsManager.Current,
                    SettingsManager.Current,
                    PluginManager.Current,
                    this._guiTimer,
                    this._threads,
                });

            foreach (var type in Enum.GetValues<ThreadQueues>())
                this._threads.TryAdd(type, new CmdTask());

            this._guiTimer.Tick += new EventHandler((sender, e) => this.Run(ThreadQueues.GUI));
            this._guiTimer.Interval = 100;

            NetworkManager.Current.DataReceived += DataReceived;
        }
        ~ThreadManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            SettingsManager.Current.Save();

            base.Dispose();

            NetworkManager.Current.DataReceived -= DataReceived;

            this._threads = null;
            this._guiTimer = null;

            Cache.Dispose();

            Application.Exit();
        }

        private void DataReceived(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;

            var sessionState = sender as ISessionState;
            if (sessionState == null) return;

            lock (sessionState.ConnectionState.Queue)
            {
                sessionState.ConnectionState.Queue.TryDequeue(out Header packet);
                if (packet == null) return;

                this.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.RECEIVE, packet);
            }
        }

        public ThreadManager Initialize()
        {
            if (this.IsDisposed) return null;

            foreach (var type in Enum.GetValues<ThreadQueues>())
                if (type != ThreadQueues.GUI)
                    this._threads[type].Task =
                        Task.Factory.StartNew(t =>
                        {
                            try
                            {
                                this.Run((ThreadQueues)t);
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine(nameof(ThreadManager) + $".Crash[{type}]: {ex.Message}");
#endif
                            }
                        },
                        type,
                        NetworkManager.Current.IsRunning.Token);

            this._guiTimer.Start();

            return this;
        }

        public void Run(ThreadQueues type)
        {
            if (this.IsDisposed) return;

            try
            {
                while (!this.IsDisposed &&
                    !NetworkManager.Current.IsRunning.IsCancellationRequested)
                {
                    if (NetworkManager.Current.IsRunning.IsCancellationRequested) return;
                    else if (type != ThreadQueues.GUI)
                        this._threads[type].SignalEvent.WaitOne();

                    while (!this.IsDisposed &&
                        !NetworkManager.Current.IsRunning.IsCancellationRequested &&
                        (this._threads?[type]?.Queue?.IsEmpty ?? true) == false)
                    {
                        this._threads[type].Queue.TryDequeue(out Cmd cmd);
                        if (cmd == null) continue;

                        switch (type)
                        {
                            case ThreadQueues.GUI:
                                try
                                {
                                    cmd.CmdFnc(cmd.Values);
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Debug.WriteLine(nameof(ThreadManager) + $"[{type}]: {ex.Message}");
#endif
                                }

                                break;
                            case ThreadQueues.Network:
                                {
                                    var sessionState = cmd.Values[0] as ISessionState;
                                    if (sessionState == null) break;

                                    var netCmd = (NetworkCommandTypes)(cmd.Values[1] ?? 0);
                                    if (netCmd == 0) break;

                                    switch (netCmd)
                                    {
                                        case NetworkCommandTypes.DISCONNECT:
                                            NetworkManager.Current.Disconnect(sessionState);

                                            break;
                                        case NetworkCommandTypes.CONNECT:
                                            {
                                                var url = cmd.Values[2] as string;
                                                if (!ClientConstants.REGEX_PALACEURL.IsMatch(url)) break;

                                                var match = ClientConstants.REGEX_PALACEURL.Match(url);
                                                if (match.Groups.Count < 2) break;

                                                var host = match.Groups[1].Value;
                                                var port = match.Groups.Count > 2 &&
                                                    !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                                                        ? Convert.ToUInt16(match.Groups[2].Value) : (ushort)0;
                                                var roomID = match.Groups.Count > 3 &&
                                                    !string.IsNullOrWhiteSpace(match.Groups[3].Value)
                                                        ? Convert.ToInt16(match.Groups[3].Value) : (short)0;

                                                var jsonText = null as string;

                                                try
                                                {
                                                    using (var httpClient = new HttpClient())
                                                        jsonText = httpClient
                                                           .GetStringAsync($"http://{host}/palace.json")
                                                           .GetAwaiter()
                                                           .GetResult();
                                                }
                                                catch (Exception ex)
                                                {
#if DEBUG
                                                    Debug.WriteLine(ex.Message);
#endif
                                                }

                                                if (!string.IsNullOrWhiteSpace(jsonText))
                                                {
                                                    var jsonResponse = JsonConvert.DeserializeObject<JObject>(jsonText) as dynamic;
                                                    if (jsonResponse != null)
                                                    {
                                                        if ((uint)jsonResponse.version == 1)
                                                        {
                                                            port = Convert.ToUInt16(jsonResponse.palacePort);
                                                        }
                                                    }
                                                }

                                                sessionState.RegInfo.desiredRoom = roomID;

                                                NetworkManager.Current.Connect(sessionState, host, port);
                                            }

                                            break;
                                        case NetworkCommandTypes.RECEIVE:
                                        case NetworkCommandTypes.SEND:
                                            if ((sessionState?.ConnectionState?.IsConnected ?? false) == false) break;

                                            var packet = cmd.Values[2] as Header;
                                            if (packet == null) break;

                                            var _type = NetworkManager.GetType(typeof(ThreadManager), packet.eventType, "Business");
                                            if (_type == null)
                                            {
                                                NetworkManager.Current.Disconnect(sessionState, true);

                                                break;
                                            }

                                            try
                                            {
                                                switch (netCmd)
                                                {
                                                    case NetworkCommandTypes.RECEIVE:
                                                        {
                                                            packet.protocolReceive = packet.protocolReceiveType.GetInstance<IProtocolReceive>();
                                                            if (packet.protocolReceive == null) break;

                                                            if (packet.length > 0)
                                                                packet.protocolReceive.Deserialize(packet, packet.refNum);

                                                            var bussinessLogic = _type.GetInstance<IBusinessReceive>();
                                                            if (bussinessLogic == null) break;

                                                            bussinessLogic.Receive(sessionState, packet);
                                                        }

                                                        break;
                                                    case NetworkCommandTypes.SEND:
                                                        {
                                                            var bussinessLogic = _type.GetInstance<IBusinessSend>();
                                                            if (bussinessLogic == null) break;

                                                            bussinessLogic.Send(sessionState, packet);
                                                        }

                                                        break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
#if DEBUG
                                                Debug.WriteLine(nameof(ThreadManager) + $"[{packet.eventType}]: {ex.Message}");
#endif
                                            }

                                            break;
                                    }
                                }

                                break;
                            case ThreadQueues.Media:
                                {
                                    var sessionState = cmd.Values[0] as IUISessionState;
                                    if ((sessionState?.ConnectionState?.IsConnected ?? false) == false) break;
                                    else if (string.IsNullOrWhiteSpace(sessionState.ServerName)) break;
                                    else if (string.IsNullOrWhiteSpace(sessionState.MediaUrl)) break;

                                    var fileName = cmd.Values[1] as string;
                                    if (string.IsNullOrWhiteSpace(fileName)) break;

                                    var _serverName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(sessionState.ServerName, @" ").Trim();
                                    var fileDir = Path.Combine(Environment.CurrentDirectory, "Media", _serverName);

                                    if (!Directory.Exists(fileDir))
                                        Directory.CreateDirectory(fileDir);

                                    var _fileName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(fileName, @"_");
                                    var filePath = Path.Combine(fileDir, _fileName);

                                    if (File.Exists(filePath)) break;

                                    try
                                    {
                                        var __fileName = _fileName.Replace(@" ", @"%20");

                                        var mediaUrl = $"{sessionState.MediaUrl}{(sessionState.MediaUrl.LastOrDefault() == '/' ? string.Empty : "/")}{__fileName}";
                                        var fileData = null as byte[];

                                        using (var httpClient = new HttpClient())
                                            fileData = httpClient
                                               .GetByteArrayAsync(mediaUrl)
                                               .GetAwaiter()
                                               .GetResult();

                                        if ((fileData?.Length ?? 0) > 0)
                                        {
                                            using (var file = File.OpenWrite(filePath))
                                                file.Write(fileData, 0, fileData.Length);

                                            if (File.Exists(filePath))
                                                this.Enqueue(ThreadQueues.GUI, args =>
                                                {
                                                    var sessionState = args[0] as IUISessionState;
                                                    if (sessionState == null) return null;

                                                    var fileName = args[1] as string;
                                                    if (fileName == null) return null;

                                                    if (fileName == sessionState.RoomInfo.roomPicture)
                                                        sessionState.RefreshScreen(ScreenLayers.Base);

                                                    sessionState.RefreshUI();

                                                    if (fileName != sessionState.RoomInfo.roomPicture)
                                                        sessionState.RefreshScreen(ScreenLayers.SpotImage);
                                                    else
                                                        sessionState.RefreshScreen(new ScreenLayers[] {
                                                            ScreenLayers.LooseProp,
                                                            ScreenLayers.SpotImage,
                                                            ScreenLayers.BottomPaint,
                                                            ScreenLayers.SpotNametag,
                                                            ScreenLayers.UserProp,
                                                            ScreenLayers.UserNametag,
                                                            ScreenLayers.ScriptedImage,
                                                            ScreenLayers.ScriptedText,
                                                            ScreenLayers.SpotBorder,
                                                            ScreenLayers.TopPaint,
                                                            ScreenLayers.Messages, });

                                                    return null;
                                                }, sessionState, _fileName);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        Debug.WriteLine(nameof(ThreadManager) + $"[{type}|{filePath}]: {ex.Message}");
#endif
                                    }
                                }

                                break;
                            case ThreadQueues.Assets:
                                {
                                    var sessionState = cmd.Values[0] as IClientSessionState;
                                    if (sessionState == null) break;

                                    var assetSpecs = cmd.Values[1] as AssetSpec[];
                                    if (assetSpecs == null ||
                                        assetSpecs.Length < 1) break;

                                    var _assetSpecs = new List<AssetSpec>();

                                    foreach (var assetSpec in assetSpecs)
                                    {
                                        var asset = AssetsManager.Current.Assets.GetValueLocked(assetSpec.id);
                                        if (asset == null)
                                        {
                                            _assetSpecs.Add(assetSpec);
                                        }
                                    }

                                    var jsonResponse = null as dynamic;
                                    using (var httpClient = new HttpClient())
                                    {
                                        var getUrl = $"{sessionState.MediaUrl}webservice/props/get/";

                                        var gzBytes = JsonConvert.SerializeObject(new
                                        {
                                            props = _assetSpecs.ToArray(),
                                            api_version = 1,
                                        }).GZCompress(); //.InvokePHP<string, byte[]>(PHPCommands.GZCOMPRESS);
                                        if (gzBytes == null ||
                                            gzBytes.Length == 0) return;

                                        var content = new ByteArrayContent(gzBytes);
                                        content.Headers.ContentType = new MediaTypeHeaderValue(@"application/json");
                                        content.Headers.ContentEncoding.Add("gzip");
                                        var request = new HttpRequestMessage
                                        {
                                            Method = HttpMethod.Post,
                                            RequestUri = new Uri(getUrl),
                                            Content = content,
                                        };
                                        request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                                        request.Headers.Add("Palace-Address", $"palace://{sessionState.ConnectionState.Host}:{sessionState.ConnectionState.Port}/{sessionState.RoomID}");
                                        request.Headers.Add("Userkey", ClientConstants.RegCodeSeed);
                                        request.Headers.Add("User-Agent", "AephixCore/1.0 (Windows) gzip"); //Environment.OSVersion.Platform
                                        request.Headers.Add("Accept-Encoding", "gzip,deflate");

                                        var json = httpClient
                                           .SendAsync(request)
                                           .GetAwaiter()
                                           .GetResult()
                                           .Content
                                           .ReadAsStringAsync()
                                           .GetAwaiter()
                                           .GetResult();
                                        if (string.IsNullOrWhiteSpace(json)) return;

                                        jsonResponse = JsonConvert.DeserializeObject<JObject>(json);
                                    }
                                    if (jsonResponse == null) return;

                                    //{"img_url":"http://alluringhistoriamedia.epalaces.com/webservice/storage/","props":[{ "id":-2028848857,"success":true,"crc":"2239908557","format":"png","name":"PC Burger","size":{ "w":"38","h":"44"},"offsets":{ "x":"-26","y":"7"},"flags":"0004"}]}
                                    if ((jsonResponse?.props?.Count ?? 0) > 0)
                                    {
                                        for (var j = 0; j < jsonResponse.props.Count; j++)
                                        {
                                            var propID = Convert.ToInt32(jsonResponse.props[j].id);

                                            if ((jsonResponse.props[j].success ?? false) == true)
                                            {
                                                var downloadUrl = $"{jsonResponse.img_url}{propID}";

                                                var asset = new AssetRec(propID);
                                                asset.assetSpec.crc = Convert.ToUInt32(jsonResponse.props[j].crc);
                                                asset.name = jsonResponse.props[j].name;
                                                asset.Format = jsonResponse.props[j].format;
                                                var flags = (UInt16)Convert.ToUInt16(jsonResponse.props[j].flags);
                                                asset.propFlags = flags.SwapShort();
                                                asset.Width = Convert.ToInt16(jsonResponse.props[j].size.w);
                                                asset.Height = Convert.ToInt16(jsonResponse.props[j].size.h);
                                                asset.Offset = new();
                                                asset.Offset.h = Convert.ToInt16(jsonResponse.props[j].offsets.x);
                                                asset.Offset.v = Convert.ToInt16(jsonResponse.props[j].offsets.y);

                                                if (string.IsNullOrWhiteSpace(downloadUrl)) continue;

                                                var assetData = null as byte[];
                                                using (var httpClient = new HttpClient())
                                                {
                                                    var request = new HttpRequestMessage
                                                    {
                                                        Method = HttpMethod.Get,
                                                        RequestUri = new Uri(downloadUrl),
                                                    };
                                                    request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                                                    request.Headers.Add("Palace-Address", $"palace://{sessionState.ConnectionState.Host}:{sessionState.ConnectionState.Port}/{sessionState.RoomID}");
                                                    request.Headers.Add("Userkey", ClientConstants.RegCodeSeed);
                                                    request.Headers.Add("User-Agent", "AephixCore/1.0 (Windows) gzip"); //Environment.OSVersion.Platform
                                                    request.Headers.Add("Accept-Encoding", "gzip,deflate");

                                                    try
                                                    {
                                                        assetData = httpClient
                                                            .SendAsync(request)
                                                            .GetAwaiter()
                                                            .GetResult()
                                                            .Content
                                                            .ReadAsByteArrayAsync()
                                                            .GetAwaiter()
                                                            .GetResult();
                                                    }
                                                    catch (Exception ex)
                                                    {
#if DEBUG
                                                        Debug.WriteLine(ex.Message);
#endif
                                                    }
                                                }

                                                if (assetData == null ||
                                                    assetData.Length < 1) continue;

                                                asset.data = assetData;

                                                AssetsManager.Current.RegisterAsset(asset);
                                            }
                                            else
                                            {
                                                foreach (var _cmd in this._threads[ThreadQueues.Network].Queue)
                                                {
                                                    var netCmd = (NetworkCommandTypes)(_cmd.Values[1] ?? 0);
                                                    if (netCmd != NetworkCommandTypes.SEND) continue;

                                                    var outboundPacket = _cmd.Values[2] as Header;
                                                    if (outboundPacket == null ||
                                                        outboundPacket.eventType != EventTypes.MSG_ASSETQUERY ||
                                                        !(outboundPacket.protocolSend is MSG_ASSETQUERY msgAssetQuery) ||
                                                        msgAssetQuery == null ||
                                                        msgAssetQuery.assetSpec.id == 0) continue;

                                                    if (msgAssetQuery.assetSpec.id == propID) return;
                                                }

                                                this.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                                                {
                                                    eventType = EventTypes.MSG_ASSETQUERY,
                                                    protocolSend = new MSG_ASSETQUERY
                                                    {
                                                        assetType = LegacyAssetTypes.RT_PROP,
                                                        assetSpec = new AssetSpec
                                                        {
                                                            id = propID,
                                                        },
                                                    }
                                                });
                                            }
                                        }

                                        Current.Enqueue(ThreadQueues.GUI, args =>
                                        {
                                            var sessionState = args.FirstOrDefault() as IUISessionState;
                                            if (sessionState == null) return null;

                                            sessionState.RefreshScreen(ScreenLayers.UserProp);

                                            return null;
                                        }, sessionState);
                                    }
                                }

                                break;
                            case ThreadQueues.Audio:
                                {
                                    var sessionState = cmd.Values[0] as IUISessionState;
                                    if ((sessionState?.ConnectionState?.IsConnected ?? false) == false) break;

                                    var audioCmd = (AudioCommandTypes)(cmd.Values[1] ?? 0);
                                    if (audioCmd == 0) break;

                                    switch (audioCmd)
                                    {
                                        case AudioCommandTypes.PAUSE:
                                            {
                                            }

                                            break;
                                        case AudioCommandTypes.PLAY:
                                            {
                                            }

                                            break;
                                        case AudioCommandTypes.ASTERISK:
                                            SystemSounds.Asterisk.Play();

                                            break;
                                        case AudioCommandTypes.BEEP:
                                            SystemSounds.Beep.Play();

                                            break;
                                        case AudioCommandTypes.EXCLAMATION:
                                            SystemSounds.Exclamation.Play();

                                            break;
                                        case AudioCommandTypes.HAND:
                                            SystemSounds.Hand.Play();

                                            break;
                                        case AudioCommandTypes.QUESTION:
                                            SystemSounds.Question.Play();

                                            break;
                                    }
                                }

                                break;
#if WINDOWS10_0_17763_0_OR_GREATER
                            case ThreadQueues.Toast:
                                {
                                    var toastArgs = (ToastCfg)cmd.Values.FirstOrDefault();
                                    if (toastArgs == null) break;

                                    var toast = new ToastContentBuilder();

                                    foreach (var arg in toastArgs.Args)
                                    {
                                        var _type = arg.Value.GetType();
                                        if (_type == Int32Exts.Types.Int32)
                                            toast.AddArgument(arg.Key, (int)arg.Value);
                                        else if (_type == StringExts.Types.String)
                                            toast.AddArgument(arg.Key, (string)arg.Value);
                                        else if (_type == DoubleExts.Types.Double)
                                            toast.AddArgument(arg.Key, (double)arg.Value);
                                        else if (_type == BooleanExts.Types.Boolean)
                                            toast.AddArgument(arg.Key, (bool)arg.Value);
                                        else if (_type == FloatExts.Types.Float)
                                            toast.AddArgument(arg.Key, (float)arg.Value);
                                    }

                                    foreach (var txt in toastArgs.Text)
                                        toast.AddText(txt);

                                    toast.Show(t =>
                                    {
                                        t.ExpirationTime = toastArgs.ExpirationTime;
                                    });

                                    //#if DEBUG
                                    //Debug.WriteLine(nameof(ThreadManager) + $"[{type}]: {toastArgs.Text.FirstOrDefault()}");
                                    //#endif
                                }

                                break;
#endif
                            default:
                                try
                                {
                                    cmd.CmdFnc(cmd.Values);
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Debug.WriteLine(nameof(ThreadManager) + $"[{type}]: {ex.Message}");
#endif
                                }

                                break;
                        }
                    }

                    //#if DEBUG
                    //Debug.WriteLine(nameof(ThreadManager) + $"[{type}]");
                    //#endif

                    if (NetworkManager.Current.IsRunning.IsCancellationRequested) return;
                    else if (type == ThreadQueues.GUI) break;
                    else if (type != ThreadQueues.GUI)
                        this._threads?[type]?.SignalEvent?.Reset();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"{type}: {ex.Message}");
#endif
            }
        }

        public ThreadManager DownloadMedia(IClientSessionState sessionState, string fileName)
        {
            if (this.IsDisposed) return null;

            var _fileName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(fileName, @"_");
            var filePath = Path.Combine(Environment.CurrentDirectory, "Media", _fileName);

            if (!File.Exists(filePath))
            {
                var _serverName = FilesystemConstants.REGEX_FILESYSTEMCHARS.Replace(sessionState.ServerName, @" ").Trim();
                filePath = Path.Combine(Environment.CurrentDirectory, "Media", _serverName, _fileName);
            }

            if (!File.Exists(filePath))
            {
                foreach (var q in this._threads[ThreadQueues.Media].Queue)
                    if ((q.Values[1] as string) == _fileName) return null;

                this.Enqueue(ThreadQueues.Media, null, sessionState, _fileName);
            }

            return this;
        }

        public ThreadManager DownloadAsset(IClientSessionState sessionState, params AssetSpec[] assetSpecs)
        {
            if (this.IsDisposed ||
                assetSpecs == null ||
                assetSpecs.Length < 1) return null;

            this.Enqueue(ThreadQueues.Assets, null, sessionState, assetSpecs);

            return this;
        }

        public void DispatchToast(ToastCfg cfg)
        {
            if (this.IsDisposed) return;

            Current.Enqueue(ThreadQueues.Toast, null, cfg);
        }

        public ThreadManager Enqueue(ThreadQueues type, CmdFnc cmdFnc = null, params object[] values)
        {
            if (this.IsDisposed) return null;

            if (this._threads?.ContainsKey(type) == true)
            {
                this._threads[type].Queue?.Enqueue(new Cmd
                {
                    CmdFnc = cmdFnc,
                    Values = values,
                });

                this._threads[type].SignalEvent?.Set();
            }

            return this;
        }

        public ThreadManager RegisterFeature(ThreadQueues type, IFeature feature)
        {
            if (this.IsDisposed) return null;

            if (this._threads?.ContainsKey(type) == true)
            {
                if (feature is IProvider provider)
                    this._threads[type].Providers.Add(provider);
                else if (feature is IConsumer consumer)
                    this._threads[type].Consumers.Add(consumer);

                feature.Initialize();
            }

            return this;
        }

        public ThreadManager UnregisterFeature(ThreadQueues type, IFeature feature)
        {
            if (this.IsDisposed) return null;

            if (this._threads?.ContainsKey(type) == true)
            {
                if (feature is IProvider provider)
                    this._threads[type].Providers.Remove(provider);
                else if (feature is IConsumer consumer)
                    this._threads[type].Consumers.Remove(consumer);

                feature.Dispose();
            }

            return this;
        }
    }
}
