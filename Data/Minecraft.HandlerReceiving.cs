using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using MineLib.Core.Data.Anvil;
using MineLib.Core.Events;
using MineLib.Core.Events.ReceiveEvents;
using MineLib.Core.Events.ReceiveEvents.Anvil;

using MineLib.PGL.Components;
using MineLib.PGL.Extensions;

namespace MineLib.PGL.Data
{
    public sealed partial class Minecraft
    {
        private Dictionary<Type, List<Func<ReceiveEvent, Task>>> ReceiveHandlers { get; set; }

        public override void RegisterReceiveEvent(Type receiveType, Func<ReceiveEvent, Task> func)
        {
            if (!receiveType.GetTypeInfo().IsSubclassOf(typeof(ReceiveEvent)))
                throw new InvalidOperationException("Type type must implement MineLib.Core.Events.ReceiveEvent");

            if (ReceiveHandlers.ContainsKey(receiveType))
                ReceiveHandlers[receiveType].Add(func);
            else
                ReceiveHandlers.Add(receiveType, new List<Func<ReceiveEvent, Task>> { func });
        }
        public override void DeregisterReceiveEvent(Type receiveType, Func<ReceiveEvent, Task> func)
        {
            if (!receiveType.GetTypeInfo().IsSubclassOf(typeof(ReceiveEvent)))
                throw new InvalidOperationException("Type type must implement MineLib.Core.Events.ReceiveEvent");

            if (ReceiveHandlers.ContainsKey(receiveType))
                ReceiveHandlers[receiveType].Remove(func);
        }

        public override void DoReceiveEvent(Type receiveType, ReceiveEvent args)
        {
            if (!receiveType.GetTypeInfo().IsSubclassOf(typeof(ReceiveEvent)))
                throw new InvalidOperationException("AsyncSending type must implement MineLib.Core.Events.ReceiveEvent");

            if (ReceiveHandlers.ContainsKey(receiveType))
                foreach (var func in ReceiveHandlers[receiveType])
                    func(args);
        }

        private void RegisterSupportedReceiveEvents()
        {
            RegisterReceiveEvent(typeof(ChatMessageEvent), OnChatMessage);

            RegisterReceiveEvent(typeof(PlayerPositionEvent), OnPlayerPosition);
            RegisterReceiveEvent(typeof(PlayerLookEvent), OnPlayerLook);
            RegisterReceiveEvent(typeof(HeldItemChangeEvent), OnHeldItemChange);
            RegisterReceiveEvent(typeof(SpawnPointEvent), OnSpawnPoint);
            RegisterReceiveEvent(typeof(UpdateHealthEvent), OnUpdateHealth);
            RegisterReceiveEvent(typeof(RespawnEvent), OnRespawn);
            RegisterReceiveEvent(typeof(ActionEvent), OnAction);
            RegisterReceiveEvent(typeof(SetExperienceEvent), OnSetExperience);

            RegisterReceiveEvent(typeof(ChunkEvent), OnChunk);
            RegisterReceiveEvent(typeof(ChunkArrayEvent), OnChunkArray);
            RegisterReceiveEvent(typeof(BlockChangeEvent), OnBlockChange);
            RegisterReceiveEvent(typeof(MultiBlockChangeEvent), OnMultiBlockChange);
            RegisterReceiveEvent(typeof(BlockActionEvent), OnBlockAction);

            RegisterReceiveEvent(typeof(BlockBreakActionEvent), OnBlockBreakAction);

            RegisterReceiveEvent(typeof(TimeUpdateEvent), OnTimeUpdate);

        }


        #region InnerReceiving

        private async Task OnChatMessage(ReceiveEvent receiveEvent)
        {
            var data = (ChatMessageEvent) receiveEvent;

            ChatHistory.Add(data.Message);
        }


        #region Anvil

        private async Task OnChunk(ReceiveEvent receiveEvent)
        {
            var data = (ChunkEvent) receiveEvent;

            if (data.Chunk.PrimaryBitMap == 0)
            {
                World.RemoveChunk(data.Chunk.Coordinates);
                return;
            }

            // -- Add the chunk to the world
            World.SetChunk(data.Chunk);
        }

        private async Task OnChunkArray(ReceiveEvent receiveEvent)
        {
            var data = (ChunkArrayEvent) receiveEvent;

            foreach (var chunk in data.Chunks)
                World.SetChunk(chunk);
        }

        private async Task OnBlockChange(ReceiveEvent receiveEvent)
        {
            var data = (BlockChangeEvent) receiveEvent;

            var id = (ushort) (data.Block >> 4);
            var meta = (byte) (data.Block & 0xF);

            var block = new Block(id, meta);

            World.SetBlock(data.Location, block);
        }

        private async Task OnMultiBlockChange(ReceiveEvent receiveEvent)
        {
            var data = (MultiBlockChangeEvent) receiveEvent;

            foreach (var record in data.Records)
            {
                var id = (ushort) (record.BlockIDMeta >> 4);
                var meta = (byte) (record.BlockIDMeta & 0xF);

                World.SetBlock(record.Coordinates, data.ChunkLocation, new Block(id, meta));
            }      
        }

        private async Task OnBlockAction(ReceiveEvent receiveEvent)
        {
            var data = (BlockActionEvent) receiveEvent;
        }

        private async Task OnBlockBreakAction(ReceiveEvent receiveEvent)
        {
            var data = (BlockBreakActionEvent) receiveEvent;
        }

        #endregion


        private async Task OnPlayerPosition(ReceiveEvent receiveEvent)
        {
            var data = (PlayerPositionEvent) receiveEvent;

#if DEBUG
            DebugComponent.PlayerPos = data.Position.ToXNAVector3();
#endif

            Player.Position.Vector3 = data.Position;
        }

        private async Task OnPlayerLook(ReceiveEvent receiveEvent)
        {
            var data = (PlayerLookEvent) receiveEvent;
        }

        private async Task OnHeldItemChange(ReceiveEvent receiveEvent)
        {
            var data = (HeldItemChangeEvent) receiveEvent;
        }

        private async Task OnSpawnPoint(ReceiveEvent receiveEvent)
        {
            var data = (SpawnPointEvent) receiveEvent;

#if DEBUG
            DebugComponent.PlayerPos = data.Location.ToXNAVector3();
#endif

            Player.Position.Vector3 = data.Location;
        }

        private async Task OnUpdateHealth(ReceiveEvent receiveEvent)
        {
            var data = (UpdateHealthEvent) receiveEvent;  
        }

        private async Task OnRespawn(ReceiveEvent receiveEvent)
        {
            var data = (RespawnEvent) receiveEvent;
        }

        private async Task OnAction(ReceiveEvent receiveEvent)
        {
            var data = (ActionEvent) receiveEvent;
        }

        private async Task OnSetExperience(ReceiveEvent receiveEvent)
        {
            var data = (SetExperienceEvent) receiveEvent;
        }

        private async Task OnTimeUpdate(ReceiveEvent receiveEvent)
        {
            var data = (TimeUpdateEvent)receiveEvent;
            World.AgeOfTheWorld = data.WorldAge;
            World.TimeOfDay = data.TimeOfDay;
        }

        #endregion InnerReceiving
    }
}
