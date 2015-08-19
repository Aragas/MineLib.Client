using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Data.Anvil;
using MineLib.Core.Interfaces;

using MineLib.PGL.Components;
using MineLib.PGL.Extensions;

namespace MineLib.PGL.Data
{
    public partial class Minecraft<T> where T : struct, IVertexType
    {
        private Dictionary<Type, List<Func<IReceive, Task>>> ReceiveHandlers { get; set; }

        public void RegisterReceiveEvent(Type receiveType, Func<IReceive, Task> func)
        {
            var any = receiveType.GetTypeInfo().ImplementedInterfaces.Any(p => p == typeof(IReceive));
            if (!any)
                throw new InvalidOperationException("Type type must implement MineLib.Core.IReceiveAsync");

            if (ReceiveHandlers.ContainsKey(receiveType))
                ReceiveHandlers[receiveType].Add(func);
            else
                ReceiveHandlers.Add(receiveType, new List<Func<IReceive, Task>> { func });
        }

        public void DeregisterReceiveEvent(Type receiveType, Func<IReceive, Task> func)
        {
            var any = receiveType.GetTypeInfo().ImplementedInterfaces.Any(p => p == typeof(IReceive));
            if (!any)
                throw new InvalidOperationException("Type type must implement MineLib.Core.IReceiveAsync");

            if (ReceiveHandlers.ContainsKey(receiveType))
                ReceiveHandlers[receiveType].Remove(func);
        }

        public void DoReceiveEvent(Type receiveType, IReceive args)
        {
            var any = receiveType.GetTypeInfo().ImplementedInterfaces.Any(p => p == typeof(IReceive));
            if (!any)
                throw new InvalidOperationException("AsyncSending type must implement MineLib.Core.IReceiveAsync");

            if (ReceiveHandlers.ContainsKey(receiveType))
                foreach (var func in ReceiveHandlers[receiveType])
                    func(args);
        }

        private void RegisterSupportedReceiveEvents()
        {
            RegisterReceiveEvent(typeof(OnChatMessage), OnChatMessage);

            RegisterReceiveEvent(typeof(OnPlayerPosition), OnPlayerPosition);
            RegisterReceiveEvent(typeof(OnPlayerLook), OnPlayerLook);
            RegisterReceiveEvent(typeof(OnHeldItemChange), OnHeldItemChange);
            RegisterReceiveEvent(typeof(OnSpawnPoint), OnSpawnPoint);
            RegisterReceiveEvent(typeof(OnUpdateHealth), OnUpdateHealth);
            RegisterReceiveEvent(typeof(OnRespawn), OnRespawn);
            RegisterReceiveEvent(typeof(OnAction), OnAction);
            RegisterReceiveEvent(typeof(OnSetExperience), OnSetExperience);

            RegisterReceiveEvent(typeof(OnChunk), OnChunk);
            RegisterReceiveEvent(typeof(OnChunkList), OnChunkList);
            RegisterReceiveEvent(typeof(OnBlockChange), OnBlockChange);
            RegisterReceiveEvent(typeof(OnMultiBlockChange), OnMultiBlockChange);
            RegisterReceiveEvent(typeof(OnBlockAction), OnBlockAction);

            RegisterReceiveEvent(typeof(OnBlockBreakAction), OnBlockBreakAction);

            RegisterReceiveEvent(typeof(OnTimeUpdate), OnTimeUpdate);

        }



        private async Task OnChatMessage(IReceive receiveEvent)
        {
            var data = (OnChatMessage) receiveEvent;

            ChatHistory.Add(data.Message);
        }


        #region Anvil

        private async Task OnChunk(IReceive receiveEvent)
        {
            var data = (OnChunk) receiveEvent;

            if (data.Chunk.PrimaryBitMap == 0)
            {
                World.RemoveChunk(data.Chunk.Coordinates);
                return;
            }

            // -- Add the chunk to the world
            World.SetChunk(data.Chunk);
        }

        private async Task OnChunkList(IReceive receiveEvent)
        {
            var data = (OnChunkList) receiveEvent;

            foreach (var chunk in data.Chunks.GetChunk())
                World.SetChunk(chunk);
        }

        private async Task OnBlockChange(IReceive receiveEvent)
        {
            var data = (OnBlockChange) receiveEvent;

            var id = (ushort) (data.Block >> 4);
            var meta = (byte) (data.Block & 0xF);

            var block = new Block(id, meta);

            World.SetBlock(data.Location, block);
        }

        private async Task OnMultiBlockChange(IReceive receiveEvent)
        {
            var data = (OnMultiBlockChange) receiveEvent;

            foreach (var record in data.Records)
            {
                var id = (ushort) (record.BlockIDMeta >> 4);
                var meta = (byte) (record.BlockIDMeta & 0xF);

                World.SetBlock(record.Coordinates, data.ChunkLocation, new Block(id, meta));
            }      
        }

        private async Task OnBlockAction(IReceive receiveEvent)
        {
            var data = (OnBlockAction) receiveEvent;
        }

        private async Task OnBlockBreakAction(IReceive receiveEvent)
        {
            var data = (OnBlockBreakAction) receiveEvent;
        }

        #endregion


        private async Task OnPlayerPosition(IReceive receiveEvent)
        {
            var data = (OnPlayerPosition) receiveEvent;

#if DEBUG
            DebugComponent<T>.PlayerPos = data.Position.ToXNAVector3();
#endif

            Player.Position.Vector3 = data.Position;
        }

        private async Task OnPlayerLook(IReceive receiveEvent)
        {
            var data = (OnPlayerLook) receiveEvent;
        }

        private async Task OnHeldItemChange(IReceive receiveEvent)
        {
            var data = (OnHeldItemChange) receiveEvent;
        }

        private async Task OnSpawnPoint(IReceive receiveEvent)
        {
            var data = (OnSpawnPoint) receiveEvent;

#if DEBUG
            DebugComponent<T>.PlayerPos = data.Location.ToXNAVector3();
#endif

            Player.Position.Vector3 = data.Location;
        }

        private async Task OnUpdateHealth(IReceive receiveEvent)
        {
            var data = (OnUpdateHealth) receiveEvent;  
        }

        private async Task OnRespawn(IReceive receiveEvent)
        {
            var data = (OnRespawn) receiveEvent;
        }

        private async Task OnAction(IReceive receiveEvent)
        {
            var data = (OnAction) receiveEvent;
        }

        private async Task OnSetExperience(IReceive receiveEvent)
        {
            var data = (OnSetExperience) receiveEvent;
        }

        private async Task OnTimeUpdate(IReceive receiveEvent)
        {
            var data = (OnTimeUpdate)receiveEvent;
            World.AgeOfTheWorld = data.WorldAge;
            World.TimeOfDay = data.TimeOfDay;
        }
    }
}
