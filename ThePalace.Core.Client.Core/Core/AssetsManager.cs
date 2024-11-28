using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Protocols.Users;
using ThePalace.Core.Constants;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core
{
    public sealed class AssetsManager : Disposable
    {
        public Type ResourceType { get; set; }

        private static readonly Lazy<AssetsManager> _current = new();
        public static AssetsManager Current => _current.Value;

        public DisposableDictionary<uint, Bitmap> SmileyFaces { get; private set; } = new();
        public DisposableDictionary<int, AssetRec> Assets { get; private set; } = new();
        public List<AssetSpec[]> Macros { get; private set; } = new();

        public AssetsManager()
        {
            this._managedResources.AddRange(
                new IDisposable[]
                {
                    this.SmileyFaces,
                    this.Assets,
                });

            ApiManager.Current.RegisterApi(nameof(this.ExecuteMacro), this.ExecuteMacro);
        }
        ~AssetsManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            this.SmileyFaces = null;
            this.Assets = null;
        }

        private void ExecuteMacro(object sender = null, EventArgs e = null)
        {
            var sessionState = sender as IUISessionState;
            if (sessionState == null) return;

            var apiEvent = e as ApiEvent;
            if (apiEvent == null) return;

            var list = new List<AssetSpec>();

            if (apiEvent.HotKeyState is AssetSpec[] _assetSpecs1)
                list.AddRange(_assetSpecs1);
            if (apiEvent.EventState is AssetSpec[] _assetSpecs2)
                list.AddRange(_assetSpecs2);

            if (list.Count > 0)
                NetworkManager.Current.Send(sessionState, new Header
                {
                    eventType = EventTypes.MSG_USERPROP,
                    protocolSend = new MSG_USERPROP
                    {
                        assetSpec = list
                        .Take(9)
                        .ToList(),
                    },
                });
        }

        public void LoadSmilies(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName)) return;

            var imgSmileyFaces = null as Bitmap;

            using (var imgStream = ResourceType
                ?.Assembly
                ?.GetManifestResourceStream($"ThePalace.Core.Desktop.Core.Resources.smilies.{resourceName}"))
            {
                if (imgStream == null) return;

                imgSmileyFaces = new Bitmap(imgStream);
                if (imgSmileyFaces == null) return;

                if (this.SmileyFaces.Count > 0)
                {
                    foreach (var smileyFace in this.SmileyFaces.Values)
                        try { smileyFace.Dispose(); } catch { }

                    this.SmileyFaces.Clear();
                }
            }

            var deltaX = (UInt32)(imgSmileyFaces.Width / UIConstants.MaxNbrFaces);
            var deltaY = (UInt32)(imgSmileyFaces.Height / UIConstants.MaxNbrColors);

            for (var x = (UInt32)0; x < imgSmileyFaces.Width; x += deltaX)
                for (var y = (UInt32)0; y < imgSmileyFaces.Height; y += deltaY)
                {
                    var result = new Bitmap(AssetConstants.DefaultPropWidth, AssetConstants.DefaultPropHeight);

                    using (var canvas = Graphics.FromImage(result))
                    {
                        canvas.InterpolationMode = InterpolationMode.NearestNeighbor;
                        canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        canvas.SmoothingMode = SmoothingMode.HighQuality;

                        canvas.DrawImage(
                            imgSmileyFaces,
                            new Rectangle(
                                0, 0,
                                AssetConstants.DefaultPropWidth,
                                AssetConstants.DefaultPropHeight),
                            new Rectangle(
                                (int)x, (int)y,
                                (int)deltaX,
                                (int)deltaY),
                            GraphicsUnit.Pixel);

                        canvas.Save();
                    }

                    var index = (UInt32)0;
                    index += x / deltaX % UIConstants.MaxNbrFaces;
                    index += (y / deltaY % UIConstants.MaxNbrColors) << 8;

                    this.SmileyFaces.TryAdd(index, result);
                }

            imgSmileyFaces?.Dispose();
            imgSmileyFaces = null;
        }

        public void RegisterAsset(AssetRec assetRec)
        {
            lock (Current.Assets)
                Current.Assets.TryAdd(assetRec.assetSpec.id, assetRec);
        }

        public void FreeAssets(bool purge = false, params Int32[] propIDs)
        {
            lock (Current.Assets)
            {
                var sessions = SessionManager.Current.Sessions.Values
                    .Cast<IClientSessionState>()
                    .ToList();

                var inUsePropIDs = sessions
                    ?.SelectMany(s => s?.RoomUsersInfo?.Values
                        ?.Where(u => u?.assetSpec != null)
                        ?.SelectMany(u => u?.assetSpec))
                    ?.Select(p => p?.id ?? 0)
                    ?.Concat(sessions
                        ?.Where(s => s?.RoomInfo?.LooseProps != null)
                        ?.SelectMany(s => s?.RoomInfo?.LooseProps
                            ?.Select(l => l?.assetSpec))
                        .Select(p => p?.id ?? 0)
                    ?.Distinct()
                    ?.Where(id => id != 0))
                    ?.ToList() ?? new();

                var iQuery = Current.Assets.Values
                    .Select(a => a.assetSpec.id)
                    .AsQueryable();

                if (propIDs.Length > 0)
                    iQuery = iQuery.Where(id => propIDs.Contains(id));

                var toPurgePropIDs = iQuery
                    .Where(id => !inUsePropIDs.Contains(id))
                    .ToList();
                foreach (var propID in toPurgePropIDs)
                {
                    var prop = this.Assets[propID];

                    if (purge)
                        this.Assets.TryRemove(propID, out prop);

                    try { prop.Image?.Dispose(); prop.Image = null; } catch { }
                }
            }
        }

        public AssetRec GetAsset(IClientSessionState sessionState, AssetSpec assetSpec, bool downloadAsset = false)
        {
            var assetID = assetSpec.id;

            lock (Current.Assets)
            {
                if (Current.Assets.ContainsKey(assetID))
                    return Current.Assets[assetID];
                else if (downloadAsset)
                    ThreadManager.Current.DownloadAsset(sessionState, assetSpec);
            }

            return null;
        }
    }
}
