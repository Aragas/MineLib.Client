using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MineLib.Network.Data.Anvil;
using MineLib.PCL.Graphics.Data;
using MineLib.PCL.Graphics.Helper;

namespace MineLib.PCL.Graphics.Map
{
    public class SectionVBO
    {
        public bool IsFilled { get { return TotalVerticesCount > 0; } }

        public BoundingBox BoundingBox { get; private set; }

        public List<VertexPositionTextureLight> OpaqueVertices { get; private set; }
        public List<VertexPositionTextureLight> TransparentVertices { get; private set; }

        public int OpaqueVerticesCount { get; private set; }
        public int TotalVerticesCount { get; private set; }
        public Vector3 GlobalPos { get; private set; }

        public SectionVBO(Section section)
        {
            GlobalPos = section.ChunkPosition.ToXNAVector3() * new Vector3(Section.Width, Section.Height, Section.Depth);

            var topFrontRight = GlobalPos + new Vector3(0, 0, 0);
            var bottomBackLeft = GlobalPos + new Vector3(16, 16, 16);

            BoundingBox = new BoundingBox(topFrontRight, bottomBackLeft);

            OpaqueVertices = new List<VertexPositionTextureLight>();
            TransparentVertices = new List<VertexPositionTextureLight>();
            BuildInside(section);

            OpaqueVerticesCount = OpaqueVertices.Count;
            TotalVerticesCount = OpaqueVertices.Count + TransparentVertices.Count;
        }

        public SectionVBO(List<VertexPositionTextureLight> opaqueVerticies, List<VertexPositionTextureLight> transparentVerticies)
        {
            OpaqueVertices = opaqueVerticies;
            TransparentVertices = transparentVerticies;
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

            OpaqueVerticesCount = OpaqueVertices.Count;
            TotalVerticesCount = OpaqueVertices.Count + TransparentVertices.Count;
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
                            var tempBlock = section.Blocks[x - 1, y, z];
                            if (tempBlock.SkyLight == 0 && tempBlock.Light == 0 && !BlockVBO.BuildWithLight)
                                goto cont1;

                            if (block.IsTransparent)
                                TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                        }

                    cont1:
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if (tempBlock.SkyLight == 0 && tempBlock.Light == 0 && !BlockVBO.BuildWithLight)
                                goto cont2;

                            if (block.IsTransparent)
                                TransparentVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                        }

                    cont2:
                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if (tempBlock.SkyLight == 0 && tempBlock.Light == 0 && !BlockVBO.BuildWithLight)
                                goto cont3;

                            if (block.IsTransparent)
                                TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                        }
                    cont3:
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if (tempBlock.SkyLight == 0 && tempBlock.Light == 0 && !BlockVBO.BuildWithLight)
                                goto cont4;

                            if (block.IsTransparent)
                                TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                        }

                    cont4:
                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if (tempBlock.SkyLight == 0 && tempBlock.Light == 0 && !BlockVBO.BuildWithLight)
                                goto cont5;

                            if (block.IsTransparent)
                                TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                        }
                    cont5:
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if (tempBlock.SkyLight == 0 && tempBlock.Light == 0 && !BlockVBO.BuildWithLight)
                                continue;

                            if (block.IsTransparent)
                                TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                        }
                    }
        }

        private void BuildFrontBorders(Section front, Section back)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = back.Blocks[x, y, 0];
                    if (newSecBlock.SkyLight == 0 && newSecBlock.Light == 0 && !BlockVBO.BuildWithLight)
                        continue;

                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = front.Blocks[x, y, Section.Depth - 1];
                         if (oldSecBlock.IsAir)
                            continue;

                        var pos = front.GetGlobalPositionByArrayIndex(x, y, Section.Depth - 1).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildBackBorders(Section back, Section front)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (newSecBlock.SkyLight == 0 && newSecBlock.Light == 0 && !BlockVBO.BuildWithLight)
                        continue;

                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = back.Blocks[x, y, 0];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = back.GetGlobalPositionByArrayIndex(x, y, 0).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, block)));
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
                        if (newSecBlock.SkyLight == 0 && newSecBlock.Light == 0 && !BlockVBO.BuildWithLight)
                            continue;

                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = right.GetGlobalPositionByArrayIndex(Section.Width - 1, y, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildLeftBorders(Section right, Section left)
        {
            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (newSecBlock.SkyLight == 0 && newSecBlock.Light == 0 && !BlockVBO.BuildWithLight)
                        continue;

                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = left.Blocks[0, y, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = left.GetGlobalPositionByArrayIndex(0, y, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildTopBorders(Section top, Section bottom)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = top.Blocks[x, 0, z];
                    if (newSecBlock.SkyLight == 0 && newSecBlock.Light == 0 && !BlockVBO.BuildWithLight)
                        continue;

                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = bottom.GetGlobalPositionByArrayIndex(x, Section.Height - 1, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        private void BuildBottomBorders(Section bottom, Section top)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (newSecBlock.SkyLight == 0 && newSecBlock.Light == 0 && !BlockVBO.BuildWithLight)
                        continue;

                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = top.Blocks[x, 0, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = top.GetGlobalPositionByArrayIndex(x, 0, z).ToXNAVector3();

                        var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

                        if (oldSecBlock.IsTransparent)
                            TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, block)));
                        else
                            OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, block)));
                    }
                }
        }

        #endregion Build

        public void ClearVerticies()
        {
            OpaqueVertices = new List<VertexPositionTextureLight>();
            TransparentVertices = new List<VertexPositionTextureLight>();
        }
    }
}