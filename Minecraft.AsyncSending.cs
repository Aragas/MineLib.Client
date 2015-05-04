using System;
using MineLib.Network;
using MineLib.Network.Data.Structs;

namespace MineLib.PCL
{
    public partial class Minecraft
    {
        public IAsyncResult BeginConnectToServer(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginConnectToServer), new BeginConnectToServerArgs(asyncCallback, state));
        }

        public IAsyncResult BeginKeepAlive(int value, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginKeepAlive), new BeginKeepAliveArgs(value, asyncCallback, state));
        }

        public IAsyncResult BeginSendClientInfo(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginSendClientInfo), new BeginSendClientInfoArgs(asyncCallback, state));
        }

        public IAsyncResult BeginRespawn(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginRespawn), new BeginRespawnArgs(asyncCallback, state));
        }

        public IAsyncResult BeginPlayerMoved(IPlaverMovedData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerMoved), new BeginPlayerMovedArgs(data, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerMoved(PlaverMovedMode mode, IPlaverMovedData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerMoved), new BeginPlayerMovedArgs(mode, data, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerSetRemoveBlock(PlayerSetRemoveBlockMode mode, IPlayerSetRemoveBlockData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerSetRemoveBlock), new BeginPlayerSetRemoveBlockArgs(mode, data, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerSetRemoveBlock(IPlayerSetRemoveBlockData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerSetRemoveBlock), new BeginPlayerSetRemoveBlockArgs(data, asyncCallback, state));
        }

        public IAsyncResult BeginSendMessage(string message, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginSendMessage), new BeginSendMessageArgs(message, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerHeldItem(short slot, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerHeldItem), new BeginPlayerHeldItemArgs(slot, asyncCallback, state));
        }
    }
}
