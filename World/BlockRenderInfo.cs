using Microsoft.Xna.Framework;

using MineLib.Core.Data.Anvil;

namespace MineLib.PGL.World
{
    public struct BlockRenderInfo
    {
        public BlockFace Face { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector2 Texture { get; private set; }
        public Block Block { get; private set; }

        public BlockRenderInfo(Vector3 pos, Block block) : this()
        {
            Position = pos;
            Face = BlockFace.NegativeZ;
            Block = block;

        }

        public BlockRenderInfo(Vector3 pos, BlockFace face, Block block) : this()
        {
            Position = pos;
            Face = face;
			Block = block;

            switch (Block.ID)
            {
                case 1:
                    Texture = new Vector2(1, 0) * BlockVBO.TextureScale;
                    break;

                case 2:
                    if(Face == BlockFace.NegativeY)
                        Texture = new Vector2(0, 0) * BlockVBO.TextureScale;
                    else
                        Texture = new Vector2(3, 0) * BlockVBO.TextureScale;
                    break;

                case 3:
                    Texture = new Vector2(2, 0) * BlockVBO.TextureScale;
                    break;

                case 8:
                case 9:
                    Texture = new Vector2(14, 0) * BlockVBO.TextureScale;
                    break;

                case 10:
                case 11:
                    Texture = new Vector2(15, 15) * BlockVBO.TextureScale;
                    break;

                case 12:
                    Texture = new Vector2(2, 1) * BlockVBO.TextureScale;
                    break;

                case 17:
                    Texture = new Vector2(4, 1) * BlockVBO.TextureScale;
                    break;

                case 18:
                case 161:
                    Texture = new Vector2(4, 3) * BlockVBO.TextureScale;
                    break;

                default:
                    Texture = new Vector2(0, 1) * BlockVBO.TextureScale;
                    break;
            }
        }

        public override string ToString() { return string.Format("X:{0}, Y:{1}, Z:{2}, {3}", Position.X, Position.Y, Position.Z, Block); }
    }
}
