#define PARALLEL

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Librainian.Collections;

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

        public ThreadSafeList<VertexPositionTextureLight> OpaqueVertices { get; private set; }
        public ThreadSafeList<VertexPositionTextureLight> TransparentVertices { get; private set; }

        public int OpaqueVerticesCount { get; private set; }
        public int TotalVerticesCount { get; private set; }
        public Vector3 GlobalPos { get; private set; }

        public SectionVBO(Section section)
        {
            GlobalPos = section.ChunkPosition.ToXNAVector3() * new Vector3(Section.Width, Section.Height, Section.Depth);

            var topFrontRight = GlobalPos + new Vector3(0, 0, 0);
            var bottomBackLeft = GlobalPos + new Vector3(16, 16, 16);

            BoundingBox = new BoundingBox(topFrontRight, bottomBackLeft);

            OpaqueVertices = new ThreadSafeList<VertexPositionTextureLight>();
            TransparentVertices = new ThreadSafeList<VertexPositionTextureLight>();

#if PARALLEL
            ParallelOptions op = new ParallelOptions();
            //op.MaxDegreeOfParallelism = 2;
            Parallel.Invoke(op,
                () => { BuildInsideX1(section); },
                () => { BuildInsideX2(section); },
                () => { BuildInsideX3(section); },
                () => { BuildInsideX4(section); });
#else
            BuildInside(section);
#endif

            OpaqueVerticesCount = OpaqueVertices.Count;
            TotalVerticesCount = OpaqueVertices.Count + TransparentVertices.Count;
        }


        public SectionVBO(ThreadSafeList<VertexPositionTextureLight> opaqueVerticies, ThreadSafeList<VertexPositionTextureLight> transparentVerticies)
        {
            OpaqueVertices = opaqueVerticies;
            TransparentVertices = transparentVerticies;
        }

        public SectionVBO(Section center, Section front, Section back, Section left, Section right, Section top, Section bottom) : this(center)
        {
            var actions = new List<Action>();

#if PARALLEL
            if (front != null && front.IsFilled)
                actions.Add(() => { BuildFrontBorders(center, front); });

            if (back != null && back.IsFilled)
                actions.Add(() => { BuildBackBorders(center, back); });

            if (left != null && left.IsFilled)
                actions.Add(() => { BuildLeftBorders(center, left); });

            if (right != null && right.IsFilled)
                actions.Add(() => { BuildRightBorders(center, right); });

            if (top != null && top.IsFilled)
                actions.Add(() => { BuildTopBorders(center, top); });

            if (bottom != null && bottom.IsFilled)
                actions.Add(() => BuildBottomBorders(center, bottom));

            ParallelOptions op = new ParallelOptions();
            op.MaxDegreeOfParallelism = 4;
            Parallel.Invoke(op, actions.ToArray());
            //Parallel.ForEach(actions, action => action());
#else

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
#endif

            OpaqueVerticesCount = OpaqueVertices.Count;
            TotalVerticesCount = OpaqueVertices.Count + TransparentVertices.Count;
        }

        #region Build

        private void BuildInsideX1(Section section)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = 0; z < Section.Depth / 4; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir) continue;

                        var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToXNAVector3();

                        if (x > 0)
                        {
                            var tempBlock = section.Blocks[x - 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange( BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                    }
        }

        private void BuildInsideX2(Section section)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = Section.Depth / 4; z < Section.Depth / 4 + Section.Depth / 4; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir) continue;

                        var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToXNAVector3();

                        if (x > 0)
                        {
                            var tempBlock = section.Blocks[x - 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                    }
        }

        private void BuildInsideX3(Section section)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = Section.Depth / 4 + Section.Depth / 4; z < Section.Depth / 4 + Section.Depth / 4 + Section.Depth / 4; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir) continue;

                        var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToXNAVector3();

                        if (x > 0)
                        {
                            var tempBlock = section.Blocks[x - 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                    }
        }

        private void BuildInsideX4(Section section)
        {
            for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = Section.Depth / 4 + Section.Depth / 4 + Section.Depth / 4; z < Section.Depth; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir) continue;

                        var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToXNAVector3();

                        if (x > 0)
                        {
                            var tempBlock = section.Blocks[x - 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
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
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (x < Section.Width - 1)
                        {
                            var tempBlock = section.Blocks[x + 1, y, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (y > 0)
                        {
                            var tempBlock = section.Blocks[x, y - 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (y < Section.Height - 1)
                        {
                            var tempBlock = section.Blocks[x, y + 1, z];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }

                        if (z > 0)
                        {
                            var tempBlock = section.Blocks[x, y, z - 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
                        }
                        if (z < Section.Depth - 1)
                        {
                            var tempBlock = section.Blocks[x, y, z + 1];
                            if (tempBlock.SkyLight != 0 || tempBlock.Light != 0 || BlockVBO.BuildWithLight)
                            {
                                if (block.IsTransparent)
                                    TransparentVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                                else if (tempBlock.IsAir || tempBlock.IsTransparent)
                                    OpaqueVertices.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, tempBlock.Light, tempBlock.SkyLight))));
                            }
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
            OpaqueVertices = new ThreadSafeList<VertexPositionTextureLight>();
            TransparentVertices = new ThreadSafeList<VertexPositionTextureLight>();
        }
    }
}