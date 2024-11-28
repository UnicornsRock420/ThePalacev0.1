using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Factories;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Network
{
    public static class AssetLoader
    {
        private static volatile ConcurrentDictionary<Int32, AssetState> inboundQueue = new ConcurrentDictionary<Int32, AssetState>();
        private static volatile Queue<AssetState> outboundQueue = new Queue<AssetState>();

        public static volatile ConcurrentDictionary<Int32, AssetRec> assetsCache = new ConcurrentDictionary<Int32, AssetRec>();

        public static void ManageAssetsOutboundQueue()
        {
            AssetState assetState = null;

            if (outboundQueue.Count > 0)
            {
                lock (outboundQueue)
                {
                    if (outboundQueue.Count > 0)
                    {
                        assetState = outboundQueue.Dequeue();
                    }
                }
            }

            if (assetState == null)
            {
                ThreadManager.manageAssetsOutboundQueueSignalEvent.Reset();

                return;
            }

            if (assetState.sessionState.driver.IsConnected() && !assetState.assetStream.HasUnsavedChanges)
            {
                var assetSend = new Business.MSG_ASSETSEND();
                assetSend.Send(null, new Message
                {
                    assetState = assetState,
                    sessionState = assetState.sessionState,
                });
            }

            if (!assetState.sessionState.driver.IsConnected() || !assetState.assetStream.hasData)
            {
                assetState.assetStream.Dispose();
            }
            else
            {
                lock (outboundQueue)
                {
                    outboundQueue.Enqueue(assetState);

                    ThreadManager.manageAssetsOutboundQueueSignalEvent.Set();
                }
            }
        }

        public static void ManageAssetsInboundQueue()
        {
            if (inboundQueue.Count < 1)
            {
                ThreadManager.manageAssetsInboundQueueSignalEvent.Reset();

                return;
            }

            inboundQueue.Values.ToList().ForEach(entry =>
            {
                if (!entry.sessionState.driver.IsConnected())
                {
                    entry.assetStream.Dispose();

                    inboundQueue.Remove(entry.assetStream.assetRec.assetSpec.id);
                }
            });

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var cacheKeys = assetsCache.Keys.ToList();
                var assetIDs = dbContext.Assets1.AsNoTracking()
                    .Where(a => cacheKeys.Contains(a.AssetId))
                    .Select(a => a.AssetId)
                    .ToList();

                assetsCache.ToList().ForEach(entry =>
                {
                    if (!assetIDs.Contains(entry.Key))
                    {
                        assetsCache.Remove(entry.Key);
                    }
                });
            }
        }

        public static void OutboundQueueTransfer(SessionState sessionState, AssetSpec assetSpec)
        {
            var assetStream = new AssetStream();

            if (assetStream.Open(assetSpec))
            {
                lock (outboundQueue)
                {
                    outboundQueue.Enqueue(new AssetState
                    {
                        sessionState = sessionState,
                        assetStream = assetStream,
                    });
                }

                ThreadManager.manageAssetsOutboundQueueSignalEvent.Set();
            }
        }

        public static void AppendInboundChunk(SessionState sessionState, AssetStream chunk)
        {
            AssetState entry;

            if (inboundQueue.ContainsKey(chunk.assetRec.assetSpec.id))
            {
                entry = inboundQueue[chunk.assetRec.assetSpec.id];
            }
            else
            {
                entry = new AssetState
                {
                    sessionState = sessionState,
                    assetStream = chunk,
                };

                inboundQueue[chunk.assetRec.assetSpec.id] = entry;
            }

            if (entry.sessionState.driver.IsConnected() && entry.assetStream.hasData && entry.sessionState.UserID == sessionState.UserID)
            {
                entry.assetStream.CopyChunkData(chunk);

                entry.assetStream.assetRec.blockNbr++;
            }

            if (entry.assetStream.assetRec.blockNbr == entry.assetStream.assetRec.nbrBlocks)
            {
                entry.assetStream.Write();
            }

            if (!entry.sessionState.driver.IsConnected() || !entry.assetStream.hasData || entry.assetStream.assetRec.blockNbr == entry.assetStream.assetRec.nbrBlocks)
            {
                entry.assetStream.Dispose();

                inboundQueue.Remove(entry.assetStream.assetRec.assetSpec.id);
            }
        }

        public static void CheckAssets(SessionState sessionState, AssetSpec propSpec)
        {
            CheckAssets(sessionState, new AssetSpec[] { propSpec });
        }

        public static void CheckAssets(SessionState sessionState, IEnumerable<AssetSpec> propSpecs)
        {
            try
            {
                using (var assetStream = new AssetStream())
                {
                    foreach (var propSpec in propSpecs)
                    {
                        if (!inboundQueue.ContainsKey(propSpec.id))
                        {
                            if (!assetStream.Open(propSpec))
                            {
                                var assetQuery = new Protocols.MSG_ASSETQUERY
                                {
                                    assetType = LegacyAssetTypes.RT_PROP,
                                    assetSpec = propSpec,
                                };

                                sessionState.Send(assetQuery, EventTypes.MSG_ASSETQUERY, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
