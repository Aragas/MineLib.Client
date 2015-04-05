using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MineLib.Client.Graphics.Data;
using MineLib.Client.Graphics.Helper;

using MineLib.Network.Data.Anvil;

namespace MineLib.Client.Graphics.Map
{
    public class SectionVBO
    {
        public bool IsFilled { get { return PrimitiveCount > 0; } }

        public BoundingBox BoundingBox { get; private set; }

        public List<VertexPositionColorHalfTexture> OpaqueVerticies { get; private set; }
        public List<VertexPositionColorHalfTexture> TransparentVerticies { get; private set; }

        public int PrimitiveCount { get; private set; }
        public Vector3 GlobalPos { get; private set; }

        public SectionVBO(Section section)
        {
            var chunkPos = section.ChunkPosition.ToXNAVector3();
            GlobalPos = chunkPos * new Vector3(Section.Width, Section.Height, Section.Depth);

            var topFrontLeft = GlobalPos + new Vector3(16, 0, 0);
            var bottomBackRight = GlobalPos + new Vector3(0, 16, 16);

            BoundingBox = new BoundingBox(topFrontLeft, bottomBackRight);

            OpaqueVerticies = new List<VertexPositionColorHalfTexture>();
            TransparentVerticies = new List<VertexPositionColorHalfTexture>();
            BuildInside(section);

            PrimitiveCount = OpaqueVerticies.Count + TransparentVerticies.Count;
        }

        public SectionVBO(List<VertexPositionColorHalfTexture> opaqueVerticies, List<VertexPositionColorHalfTexture> transparentVerticies)
        {
            OpaqueVerticies = opaqueVerticies;
            TransparentVerticies = transparentVerticies;
        }

        public SectionVBO(Section center, Section front, Section back, Section left, Section right, Section top, Section bottom) : this(center)
        {
            if (front != null && front.IsFilled)
                BuildFrontBorders(center, front);

            if (back != null && back.IsFilled)
                BuildBackBorders(center, back);

            if (left != null && left.IsFilled)
                BuildLeftBorders(center, left);

            if (right != null && right.IsFilled)
                BuildRightBorders(center, right);

            if (top != null && top.IsFilled)
                BuildTopBorders(center, top);

            if (bottom != null && bottom.IsFilled)
                BuildBottomBorders(center, bottom);

            PrimitiveCount = OpaqueVerticies.Count + TransparentVerticies.Count;
        }

        #region Build

        private void BuildInside(Section section)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = 0; z < Section.Depth; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir) continue;

                        var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToXNAVector3();

                        if (x > 0)
                        {
                            if (block.IsTransparent)
                                TransparentVerticies.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x - 1, y, z].Light, section.Blocks[x - 1, y, z].SkyLight))));
                            else if (section.Blocks[x - 1, y, z].IsAir || section.Blocks[x - 1, y, z].IsTransparent)
                                OpaqueVerticies.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x - 1, y, z].Light, section.Blocks[x - 1, y, z].SkyLight))));
                        }
                        if (x < Section.Width - 1)
                        {
                            if (block.IsTransparent)
                                TransparentVerticies.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x + 1, y, z].Light, section.Blocks[x + 1, y, z].SkyLight))));
                            else if (section.Blocks[x + 1, y, z].IsAir || section.Blocks[x + 1, y, z].IsTransparent)
                                OpaqueVerticies.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x + 1, y, z].Light, section.Blocks[x + 1, y, z].SkyLight))));
                        }

                        if (y > 0)
                        {
                            if (block.IsTransparent)
                                TransparentVerticies.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y - 1, z].Light, section.Blocks[x, y - 1, z].SkyLight))));
                            else if (section.Blocks[x, y - 1, z].IsAir || section.Blocks[x, y - 1, z].IsTransparent)
                                OpaqueVerticies.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y - 1, z].Light, section.Blocks[x, y - 1, z].SkyLight))));
                        }
                        if (y < Section.Height - 1)
                        {
                            if (block.IsTransparent)
                                TransparentVerticies.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y + 1, z].Light, section.Blocks[x, y + 1, z].SkyLight))));
                            else if (section.Blocks[x, y + 1, z].IsAir || section.Blocks[x, y + 1, z].IsTransparent)
                                OpaqueVerticies.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y + 1, z].Light, section.Blocks[x, y + 1, z].SkyLight))));
                        }

                        if (z > 0)
                        {
                            if (block.IsTransparent)
                                TransparentVerticies.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y, z - 1].Light, section.Blocks[x, y, z - 1].SkyLight))));
                            else if (section.Blocks[x, y, z - 1].IsAir || section.Blocks[x, y, z - 1].IsTransparent)
                                OpaqueVerticies.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y, z - 1].Light, section.Blocks[x, y, z - 1].SkyLight))));
                        }
                        if (z < Section.Depth - 1)
                        {
                            if (block.IsTransparent)
                                TransparentVerticies.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y, z + 1].Light, section.Blocks[x, y, z + 1].SkyLight))));
                            else if (section.Blocks[x, y, z + 1].IsAir || section.Blocks[x, y, z + 1].IsTransparent)
                                OpaqueVerticies.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y, z + 1].Light, section.Blocks[x, y, z + 1].SkyLight))));
                        }
                    }
        }

        private void BuildFrontBorders(Section front, Section back)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = back.Blocks[x, y, 0];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = front.Blocks[x, y, Section.Depth - 1];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = front.GetGlobalPositionByArrayIndex(x, y, Section.Depth - 1).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVerticies.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVerticies.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildBackBorders(Section back, Section front)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = back.Blocks[x, y, 0];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = back.GetGlobalPositionByArrayIndex(x, y, 0).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVerticies.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVerticies.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildRightBorders(Section left, Section right)
        {
            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = left.Blocks[0, y, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = right.Blocks[Section.Width - 1, y, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = right.GetGlobalPositionByArrayIndex(Section.Width - 1, y, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVerticies.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVerticies.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildLeftBorders(Section right, Section left)
        {
            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = left.Blocks[0, y, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = left.GetGlobalPositionByArrayIndex(0, y, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVerticies.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVerticies.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildTopBorders(Section top, Section bottom)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = top.Blocks[x, 0, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = bottom.GetGlobalPositionByArrayIndex(x, Section.Height - 1, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVerticies.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVerticies.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildBottomBorders(Section bottom, Section top)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = top.Blocks[x, 0, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = top.GetGlobalPositionByArrayIndex(x, 0, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVerticies.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVerticies.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        #endregion Build

        public void IncreasedBoundingBox(Vector3 vector3)
        {
            var topFrontLeft = GlobalPos + new Vector3(16 + vector3.X, 0 - vector3.Y, 0 - vector3.Z);
            var bottomBackRight = GlobalPos + new Vector3(0 - vector3.X, 16 + vector3.Y, 16 + vector3.Z);

            BoundingBox = new BoundingBox(topFrontLeft, bottomBackRight);
        }

        public void ClearVerticies()
        {
            OpaqueVerticies = new List<VertexPositionColorHalfTexture>();
            TransparentVerticies = new List<VertexPositionColorHalfTexture>();
        }
    }
}