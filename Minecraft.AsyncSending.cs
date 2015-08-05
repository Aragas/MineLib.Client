using Microsoft.Xna.Framework.Graphics;

using MineLib.Core;
using MineLib.Core.Data;
using MineLib.Core.Data.Structs;
using MineLib.Core.Interfaces;

namespace MineLib.PCL
{
    public partial class Minecraft<T> where T : struct, IVertexType
    {
        public void ConnectToServer(string serverHost, ushort port, string username, VarInt protocol)
        {
            _networkHandler.DoSending(typeof(ConnectToServer), new ConnectToServerArgs(serverHost, port, username, protocol));
        }

        public void KeepAlive(int value)
        {
            _networkHandler.DoSending(typeof(KeepAlive), new KeepAliveArgs(value));
        }

        public void SendClientInfo()
        {
             _networkHandler.DoSending(typeof(SendClientInfo), new SendClientInfoArgs());
        }

        public void Respawn()
        {
            _networkHandler.DoSending(typeof(Respawn), new RespawnArgs());
        }

        public void PlayerMoved(IPlaverMovedData data)
        {
            _networkHandler.DoSending(typeof(PlayerMoved), new PlayerMovedArgs(data));
        }

        public void PlayerMoved(PlaverMovedMode mode, IPlaverMovedData data)
        {
            _networkHandler.DoSending(typeof(PlayerMoved), new PlayerMovedArgs(mode, data));
        }

        public void PlayerSetRemoveBlock(PlayerSetRemoveBlockMode mode, IPlayerSetRemoveBlockData data)
        {
            _networkHandler.DoSending(typeof(PlayerSetRemoveBlock), new PlayerSetRemoveBlockArgs(mode, data));
        }

        public void PlayerSetRemoveBlock(IPlayerSetRemoveBlockData data)
        {
            _networkHandler.DoSending(typeof(PlayerSetRemoveBlock), new PlayerSetRemoveBlockArgs(data));
        }

        public void SendMessage(string message)
        {
            _networkHandler.DoSending(typeof(SendMessage), new SendMessageArgs(message));
        }

        public void PlayerHeldItem(short slot)
        {
            _networkHandler.DoSending(typeof(PlayerHeldItem), new PlayerHeldItemArgs(slot));
        }
    }
}
