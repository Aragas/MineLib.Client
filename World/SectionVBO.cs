//#define FULLBLOCKS

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Data.Anvil;

using MineLib.PGL.Extensions;

namespace MineLib.PGL.World
{
    public class SectionVBO : IDisposable
    {
        public Vector3 GlobalPos { get; private set; }

        public bool IsFilled { get { return TotalVerticesCount > 0; } }

        public BoundingBox BoundingBox { get; private set; }


        public IVertexType[] OpaqueVerticesArray { get { return OpaqueVertices.ToArray(); } }
        public int OpaqueVerticesCount { get { return OpaqueVertices.Count; } }

        private List<IVertexType> OpaqueVertices { get; set; }

        public IVertexType[] TransparentVerticesArray { get { return TransparentVertices.ToArray(); } }
        public int TransparentVerticesCount { get { return TransparentVertices.Count; } }
        private List<IVertexType> TransparentVertices { get; set; }

        public int TotalVerticesCount { get { return OpaqueVerticesCount + TransparentVerticesCount; } }

        //public ThreadSafeList<int> Indicies;

        public SectionVBO(Section section)
        {
            GlobalPos = section.ChunkPosition.ToXNAVector3() * new Vector3(Section.Width, Section.Height, Section.Depth);

            var topFrontRight = GlobalPos + new Vector3(0, 0, 0);
            var bottomBackLeft = GlobalPos + new Vector3(16, 16, 16);

            BoundingBox = new BoundingBox(topFrontRight, bottomBackLeft);

            OpaqueVertices = new List<IVertexType>();
            TransparentVertices = new List<IVertexType>();
            //Indicies = new ThreadSafeList<int>();


            /*
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = 0; z < Section.Depth; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir)
                            continue;
                        var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToXNAVector3();
                        OpaqueVertices.AddRange(BlockVBO.CubeFull(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, 15, 15))));
                    }

            */


            BuildInside(section);
        }


        public SectionVBO(Section center, Section front, Section back, Section left, Section right, Section top, Section bottom) : this(center)
        {
            ///*
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
            //*/

            //OpaqueVerticesCount = OpaqueVertices.Count;
            //TotalVerticesCount = OpaqueVertices.Count + TransparentVertices.Count;
        }


        #region Build

#if FULLBLOCKS

        private void AddCubeSide(Vector3 pos, Block block, bool isTranspared)
        {
            if (isTranspared)
            {
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.NegativeX, block)));
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.NegativeY, block)));
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.NegativeZ, block)));
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.PositiveX, block)));
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.PositiveY, block)));
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.PositiveZ, block)));
            }
            else
            {
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.NegativeX, block)));
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.NegativeY, block)));
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.NegativeZ, block)));
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.PositiveX, block)));
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.PositiveY, block)));
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, BlockFace.PositiveZ, block)));
            }
        }


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
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid))
                                AddCubeSide(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid))
                                AddCubeSide(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid))
                                AddCubeSide(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid))
                                AddCubeSide(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid))
                                AddCubeSide(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid))
                                AddCubeSide(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                    }
        }


        private void BuildFrontBorders(Section front, Section back)
        {
            var pos1 = front.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = back.Blocks[x, y, 0];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid) // IsOpaque
                        continue;


                    var oldSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, y, Section.Depth - 1);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildBackBorders(Section back, Section front)
        {
            var pos1 = back.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = back.Blocks[x, y, 0];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, y, 0);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildRightBorders(Section left, Section right)
        {
            var pos1 = right.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = left.Blocks[0, y, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(Section.Width - 1, y, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildLeftBorders(Section right, Section left)
        {
            var pos1 = left.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = left.Blocks[0, y, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(0, y, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildTopBorders(Section top, Section bottom)
        {
            var pos1 = bottom.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = top.Blocks[x, 0, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, Section.Height - 1, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildBottomBorders(Section bottom, Section top)
        {
            var pos1 = top.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = top.Blocks[x, 0, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, 0, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, block, block.IsTransparent || block.IsFluid);
                }
        }

#else

        private void AddCubeSide1(Vector3 pos, BlockFace face, Block block, bool isTranspared)
        {
            //if (isTranspared)
            //{
            //    var v = BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, face, block), Indicies.Count, out i);
            //    TransparentVertices.AddRange(v);
            //    Indicies.AddRange(i);
            //}
            //else
            //{
            //    int[] i;
            //    var v = BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, face, block), Indicies.Count, out i);
            //    OpaqueVertices.AddRange(v);
            //    Indicies.AddRange(i);
            //}
        }

        private void AddCubeSide(Vector3 pos, BlockFace face, Block block, bool isTranspared)
        {
            if (isTranspared)
                TransparentVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, face, block)));
            else
                OpaqueVertices.AddRange(BlockVBO.CreateQuadSideTriagled(new BlockRenderInfo(pos, face, block)));
        }
        

        private static bool Check(Block block, Block tempBlock)
        {
            if (tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) // render only near air blocks
                if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) // if block have light or non-light block enabled
                    if (!block.IsFluid || !tempBlock.IsFluid) // render only liqiuds surface
                        return true;

            return false;
        }

        private static bool CheckN(Block newSecBlock, Block block)
        {
            if (newSecBlock.IsAir || newSecBlock.IsTransparent || newSecBlock.IsFluid)
                if (block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight)
                    if (!newSecBlock.IsFluid || !block.IsFluid)
                        return true;

            return false;
        }


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
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid)) 
                                AddCubeSide(pos, BlockFace.PositiveX, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid)) 
                                AddCubeSide(pos, BlockFace.NegativeX, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid)) 
                                AddCubeSide(pos, BlockFace.PositiveY, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid)) 
                                AddCubeSide(pos, BlockFace.NegativeY, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid)) 
                                AddCubeSide(pos, BlockFace.PositiveZ, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if ((tempBlock.IsAir || tempBlock.IsTransparent || tempBlock.IsFluid) && (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight) && (!block.IsFluid || !tempBlock.IsFluid)) 
                                AddCubeSide(pos, BlockFace.NegativeZ, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight), block.IsTransparent || block.IsFluid);
                        }
                    }
        }


        private void BuildFrontBorders(Section front, Section back)
        {
            var pos1 = front.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = back.Blocks[x, y, 0];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid) // IsOpaque
                        continue;


                    var oldSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, y, Section.Depth - 1);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, BlockFace.NegativeZ, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildBackBorders(Section back, Section front)
        {
            var pos1 = back.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                {
                    var newSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = back.Blocks[x, y, 0];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, y, 0);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, BlockFace.PositiveZ, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildRightBorders(Section left, Section right)
        {
            var pos1 = right.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = left.Blocks[0, y, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(Section.Width - 1, y, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, BlockFace.NegativeX, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildLeftBorders(Section right, Section left)
        {
            var pos1 = left.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int y = 0; y < Section.Height; y++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = left.Blocks[0, y, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(0, y, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, BlockFace.PositiveX, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildTopBorders(Section top, Section bottom)
        {
            var pos1 = bottom.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = top.Blocks[x, 0, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid) 
                        continue;


                    var oldSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, Section.Height - 1, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, BlockFace.NegativeY, block, block.IsTransparent || block.IsFluid);
                }
        }

        private void BuildBottomBorders(Section bottom, Section top)
        {
            var pos1 = top.GetGlobalPositionByArrayIndex(0, 0, 0).ToXNAVector3();

            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (!newSecBlock.IsAir && !newSecBlock.IsTransparent && !newSecBlock.IsFluid)
                        continue;


                    var oldSecBlock = top.Blocks[x, 0, z];
                    if (oldSecBlock.IsAir)
                        continue;

                    var pos = pos1 + new Vector3(x, 0, z);

                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);
                    if ((block.SkyLight != 0 || block.Light != 0 || BlockVBO.BuildWithLight) && (!newSecBlock.IsFluid || !block.IsFluid))
                        AddCubeSide(pos, BlockFace.PositiveY, block, block.IsTransparent || block.IsFluid);
                }
        }

#endif

        #endregion Build


        public void ClearVerticies()
        {
            if (OpaqueVertices != null)
            {
                OpaqueVertices.Clear();
                OpaqueVertices = null;
                OpaqueVertices = new List<IVertexType>();
            }


            if (TransparentVertices != null)
            {
                TransparentVertices.Clear();
                TransparentVertices = null;
                TransparentVertices = new List<IVertexType>();
            }

            //OpaqueVertices = new List<VertexPositionTextureLight>();
            //TransparentVertices = new List<VertexPositionTextureLight>();
        }

        public void Dispose()
        {
            if(OpaqueVertices != null)
                OpaqueVertices.Clear();

            if (TransparentVertices != null)
                TransparentVertices.Clear();
        }
	}
}