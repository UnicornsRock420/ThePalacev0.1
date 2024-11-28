using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Server.Commands;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Server.Network.Sockets;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Core
{
    public static class ThreadManager
    {
        private static ConcurrentDictionary<string, Task> tasks = new ConcurrentDictionary<string, Task>();

        private static int threadManageConnections_InMilliseconds;
        private static int threadRefreshSettings_InMilliseconds;
        private static int threadManageAssets_InMilliseconds;
        private static int threadManageFiles_InMilliseconds;
        private static int threadManageQueue_InMilliseconds;
        private static int threadAbortWait_InMilliseconds;
        private static int threadPause_InMilliseconds;
        //private static int threadWait_InMilliseconds;
        private static int threadManageAssetsMax;
        private static int threadManageFilesMax;
        private static int threadManageQueueMax;

        public static volatile CancellationTokenSource cancelTokenSrc = new CancellationTokenSource();
        public static volatile ManualResetEvent manageAssetsInboundQueueSignalEvent = new ManualResetEvent(false);
        public static volatile ManualResetEvent manageAssetsOutboundQueueSignalEvent = new ManualResetEvent(false);
        public static volatile ManualResetEvent manageFilesQueueSignalEvent = new ManualResetEvent(false);
        public static volatile ManualResetEvent manageMessagesQueueSignalEvent = new ManualResetEvent(false);
        public static volatile ManualResetEvent serverShutdown = new ManualResetEvent(false);

        public static void Initialize()
        {
            threadManageConnections_InMilliseconds = ConfigManager.GetValue<int>("ThreadManageConnections_InMilliseconds", 750).Value;
            threadRefreshSettings_InMilliseconds = ConfigManager.GetValue<int>("ThreadRefreshSettings_InMilliseconds", 15000).Value;
            threadManageAssets_InMilliseconds = ConfigManager.GetValue<int>("ThreadManageAssets_InMilliseconds", 100).Value;
            threadManageFiles_InMilliseconds = ConfigManager.GetValue<int>("ThreadManageFiles_InMilliseconds", 100).Value;
            threadManageQueue_InMilliseconds = ConfigManager.GetValue<int>("ThreadManageQueue_InMilliseconds", 100).Value;
            threadAbortWait_InMilliseconds = ConfigManager.GetValue<int>("ThreadAbortWait_InMilliseconds", 1000).Value;
            threadPause_InMilliseconds = ConfigManager.GetValue<int>("ThreadPause_InMilliseconds", 500).Value;
            //threadWait_InMilliseconds = ConfigManager.GetValue<int>("ThreadWait_InMilliseconds", 60000).Value;
            threadManageAssetsMax = ConfigManager.GetValue<int>("ThreadManageAssetsMax", 4).Value;
            threadManageFilesMax = ConfigManager.GetValue<int>("ThreadManageFilesMax", 4).Value;
            threadManageQueueMax = ConfigManager.GetValue<int>("ThreadManageQueueMax", 4).Value;

            manageAssetsOutboundQueueSignalEvent.Reset();
            manageFilesQueueSignalEvent.Reset();
            manageMessagesQueueSignalEvent.Reset();
            serverShutdown.Reset();

            for (var j = 0; j < threadManageQueueMax; j++)
            {
                Action manageQueue = () =>
                {
                    do
                    {
                        Network.SessionManager.ManageMessages();

                        manageMessagesQueueSignalEvent.WaitOne();

                        Thread.Sleep(threadManageQueue_InMilliseconds);
                    } while (!IsThreadAborted());
                };
                tasks[$"{nameof(manageQueue)}-{j}"] = Task.Factory.StartNew(manageQueue, cancelTokenSrc.Token);
            }

            for (var j = 0; j < threadManageFilesMax; j++)
            {
                Action manageFiles = () =>
                {
                    do
                    {
                        FileLoader.ManageFiles();

                        manageFilesQueueSignalEvent.WaitOne();

                        Thread.Sleep(threadManageFiles_InMilliseconds);
                    } while (!IsThreadAborted());
                };
                tasks[$"{nameof(manageFiles)}-{j}"] = Task.Factory.StartNew(manageFiles, cancelTokenSrc.Token);
            }

            for (var j = 0; j < threadManageAssetsMax; j++)
            {
                Action manageAssetsOutboundQueue = () =>
                {
                    do
                    {
                        AssetLoader.ManageAssetsOutboundQueue();

                        manageAssetsOutboundQueueSignalEvent.WaitOne();

                        Thread.Sleep(threadManageAssets_InMilliseconds);
                    } while (!IsThreadAborted());
                };
                tasks[$"{nameof(manageAssetsOutboundQueue)}-{j}"] = Task.Factory.StartNew(manageAssetsOutboundQueue, cancelTokenSrc.Token);
            }

            Action manageAssetsInboundQueue = () =>
            {
                do
                {
                    AssetLoader.ManageAssetsInboundQueue();

                    manageAssetsInboundQueueSignalEvent.WaitOne();

                    Thread.Sleep(threadManageAssets_InMilliseconds);
                } while (!IsThreadAborted());
            };
            tasks[nameof(manageAssetsInboundQueue)] = Task.Factory.StartNew(manageAssetsInboundQueue, cancelTokenSrc.Token);

            Action manageConnections = () =>
            {
                do
                {
                    PalaceAsyncSocket.ManageConnections();
                    //ProxyAsyncSocket.ManageConnections();
                    WebAsyncSocket.ManageConnections();

                    Thread.Sleep(threadManageConnections_InMilliseconds);
                } while (!IsThreadAborted());
            };
            tasks[nameof(manageConnections)] = Task.Factory.StartNew(manageConnections, cancelTokenSrc.Token);

            Action refreshSettings = () =>
            {
                do
                {
                    ServerState.RefreshSettings();

                    Thread.Sleep(threadRefreshSettings_InMilliseconds);
                } while (!IsThreadAborted());
            };
            tasks[nameof(refreshSettings)] = Task.Factory.StartNew(refreshSettings, cancelTokenSrc.Token);

            Action palaceAsyncSocket = () =>
            {
                do
                {
                    PalaceAsyncSocket.Initialize();

                    Thread.Sleep(threadManageConnections_InMilliseconds);
                } while (!IsThreadAborted());
            };
            tasks[nameof(palaceAsyncSocket)] = Task.Factory.StartNew(palaceAsyncSocket, cancelTokenSrc.Token);

            //Action proxyAsyncSocket = () =>
            //{
            //    do
            //    {
            //        ProxyAsyncSocket.Initialize();

            //        Thread.Sleep(threadManageConnections_InMilliseconds);
            //    } while (!IsThreadAborted());
            //};
            //tasks[nameof(proxyAsyncSocket)] = Task.Factory.StartNew(proxyAsyncSocket, cancelTokenSrc.Token);
#if DEBUG
            Action consoleInputThread = () =>
            {
                do
                {
                    var line = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    using (var dbContext = DbConnection.For<ThePalaceEntities>())
                    {
                        CommandsEngine.Eval(new SessionState
                        {
                            dbContext = dbContext,
                        }, null, line);
                    }
                } while (!IsThreadAborted());
            };
            tasks[nameof(consoleInputThread)] = Task.Factory.StartNew(consoleInputThread, cancelTokenSrc.Token);
#endif
            WebAsyncSocket.Initialize();

            do
            {
                serverShutdown.WaitOne();
            } while (!IsThreadAborted());
        }

        public static void Dispose()
        {
            cancelTokenSrc.Cancel();

            Thread.Sleep(threadAbortWait_InMilliseconds);

            try
            {
                tasks.Keys.ToList().ForEach(key =>
                {
                    if (tasks[key] != null)
                    {
                        KillThread(key);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void KillThread(string threadIndex)
        {
            if (!tasks.ContainsKey(threadIndex))
            {
                return;
            }

            var fails = 5;

            while (fails > 0)
            {
                try
                {
                    tasks[threadIndex].Dispose();

                    tasks.TryRemove(threadIndex, out var _);

                    break;
                }
                catch
                {
                    fails--;

                    Thread.Sleep(threadPause_InMilliseconds);
                }
            }
        }

        public static bool IsThreadAborted()
        {
            var result = cancelTokenSrc.IsCancellationRequested || cancelTokenSrc.Token.IsCancellationRequested;

            if (result)
            {
                try
                {
                    Thread.CurrentThread.Abort();
                }
#if DEBUG
                catch (Exception ex)
                {
                    ex.DebugLog(false);
                }
#else
                catch { }
#endif
            }

            return result;
        }
    }
}
