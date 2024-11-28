using ThePalace.Core.Server.Models;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ThePalace.Core.Server.Network.Sockets
{
    public class WebSocketHub : WebSocketBehavior
    {
        private WebSocketConnectionState connectionState;

        public WebSocketHub()
        {
        }

        protected override void OnOpen()
        {
            connectionState = WebAsyncSocket.Accept(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            WebAsyncSocket.DropConnection(connectionState);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            WebAsyncSocket.Receive(connectionState, e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
        }

        public new void Send(string data)
        {
            base.Send(data);
        }
    }
}
