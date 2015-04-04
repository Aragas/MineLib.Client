using Microsoft.Xna.Framework;

using MineLib.Network.Data.Anvil;

namespace MineLib.Client.Graphics.Data
{
    public struct BlockRenderInfo
    {
        public Vector3 Position { get; private set; }
        public Block Block { get; private set; }

        public BlockRenderInfo(Vector3 pos, Block block) : this()
        {
            Position = pos;
			Block = block;
        }

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}, Z:{2}, {3}", Position.X, Position.Y, Position.Z, Block.ToString());
        }
    }
}
