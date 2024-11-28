using System.Collections.Generic;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Factories;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Network
{
    public static class FileLoader
    {
        private static volatile Queue<MediaState> mediaStreams = new Queue<MediaState>();

        public static void QueueTransfer(SessionState sessionState, MediaStream stream)
        {
            lock (mediaStreams)
            {
                mediaStreams.Enqueue(new MediaState
                {
                    sessionState = sessionState,
                    mediaStream = stream,
                });

                ThreadManager.manageFilesQueueSignalEvent.Set();
            }
        }

        public static void ManageFiles()
        {
            MediaState mediaState = null;

            if (mediaStreams.Count > 0)
            {
                lock (mediaStreams)
                {
                    if (mediaStreams.Count > 0)
                    {
                        mediaState = mediaStreams.Dequeue();
                    }
                }
            }

            if (mediaState == null)
            {
                ThreadManager.manageFilesQueueSignalEvent.Reset();

                return;
            }

            if (mediaState.sessionState.driver.IsConnected() && mediaState.mediaStream.hasData)
            {
                var fileSend = new Business.MSG_FILESEND();
                fileSend.Send(null, new Message
                {
                    mediaState = mediaState,
                    sessionState = mediaState.sessionState,
                });
            }

            if (!mediaState.sessionState.driver.IsConnected() || !mediaState.mediaStream.hasData)
            {
                mediaState.mediaStream.Dispose();
            }
            else
            {
                lock (mediaStreams)
                {
                    mediaStreams.Enqueue(mediaState);

                    ThreadManager.manageFilesQueueSignalEvent.Set();
                }
            }
        }
    }
}
