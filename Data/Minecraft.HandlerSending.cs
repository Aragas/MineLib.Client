using Aragas.Core.Data;

using MineLib.Core.Data.Structs;
using MineLib.Core.Events.SendingEvents;

namespace MineLib.PGL.Data
{
    public sealed partial class Minecraft
    {
        #region InnerSending

        public void ConnectToServer(string serverHost, ushort port, string username, VarInt protocol)
        {
            NetworkHandler.DoSending(typeof(ConnectToServerEvent), new ConnectToServerArgs(serverHost, port, username, protocol));
        }

        public void KeepAlive(int value)
        {
            NetworkHandler.DoSending(typeof(KeepAliveEvent), new KeepAliveEventArgs(value));
        }

        public void SendClientInfo()
        {
             NetworkHandler.DoSending(typeof(SendClientInfoEvent), new SendClientInfoEventArgs());
        }

        public void Respawn()
        {
            NetworkHandler.DoSending(typeof(RespawnEvent), new RespawnEventArgs());
        }

        public void PlayerMoved(IPlaverMovedData data)
        {
            NetworkHandler.DoSending(typeof(PlayerMovedEvent), new PlayerMovedEventArgs(data));
        }

        public void PlayerMoved(PlaverMovedMode mode, IPlaverMovedData data)
        {
            NetworkHandler.DoSending(typeof(PlayerMovedEvent), new PlayerMovedEventArgs(mode, data));
        }

        public void PlayerSetRemoveBlock(PlayerSetRemoveBlockMode mode, IPlayerSetRemoveBlockData data)
        {
            NetworkHandler.DoSending(typeof(PlayerSetRemoveBlockEvent), new PlayerSetRemoveBlockEventArgs(mode, data));
        }

        public void PlayerSetRemoveBlock(IPlayerSetRemoveBlockData data)
        {
            NetworkHandler.DoSending(typeof(PlayerSetRemoveBlockEvent), new PlayerSetRemoveBlockEventArgs(data));
        }

        public void SendMessage(string message)
        {
            NetworkHandler.DoSending(typeof(SendMessageEvent), new SendMessageEventArgs(message));
        }

        public void PlayerHeldItem(short slot)
        {
            NetworkHandler.DoSending(typeof(PlayerHeldItemEvent), new PlayerHeldItemEventArgs(slot));
        }

        #endregion InnerSending
    }
}
