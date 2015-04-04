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

        public List<VertexPositionColorHalfTexture> Vertices { get; set; }
        public int PrimitiveCount { get; private set; }
        public Vector3 GlobalPos { get; private set; }

	    public SectionVBO(Section section)
	    {
	        var chunkPos = section.ChunkPosition.ToVector3();
            GlobalPos = chunkPos * new Vector3(Section.Width, Section.Height, Section.Depth);

	        var topFrontLeft        = GlobalPos + new Vector3(16,   0,  0);
            var topFrontRight       = GlobalPos + new Vector3(0,    0,  0);
            var topBackLeft         = GlobalPos + new Vector3(16,   0,  16);
            var topBackRight        = GlobalPos + new Vector3(0,    0,  16);

            var bottomFrontLeft     = GlobalPos + new Vector3(16,   16, 0);
            var bottomFrontRight    = GlobalPos + new Vector3(0,    16, 0);
            var bottomBackLeft      = GlobalPos + new Vector3(16,   16, 16);
            var bottomBackRight     = GlobalPos + new Vector3(0,    16, 16);

            //BoundingBox = new Region8(topFrontLeft, topFrontRight, topBackLeft, topBackRight, bottomFrontLeft, bottomFrontRight, bottomBackLeft, bottomBackRight);
            BoundingBox = new BoundingBox(topFrontLeft, bottomBackRight);

	        Vertices = BuildInside(section);
	        PrimitiveCount = Vertices.Count;
	    }

	    public SectionVBO(List<VertexPositionColorHalfTexture> vertex)
		{
			Vertices = vertex;
		}

        public SectionVBO(Section center, Section front, Section back, Section left, Section right, Section top, Section bottom) : this(center)
	    {
            if (front != null && front.IsFilled)
                Vertices.AddRange(BuildFrontBorders(center, front));

            if (back != null && back.IsFilled)
                Vertices.AddRange(BuildBackBorders(center, back));

            if (left != null && left.IsFilled)
                Vertices.AddRange(BuildLeftBorders(center, left));

            if (right != null && right.IsFilled)
                Vertices.AddRange(BuildRightBorders(center, right));

            if (top != null && top.IsFilled)
                Vertices.AddRange(BuildTopBorders(center, top));

            if (bottom != null && bottom.IsFilled)
                Vertices.AddRange(BuildBottomBorders(center, bottom));

            PrimitiveCount = Vertices.Count;
	    }

	    private static List<VertexPositionColorHalfTexture> BuildInside(Section section)
        {
            var sideBlocks = new List<VertexPositionColorHalfTexture>();

			for (int x = 0; x < Section.Width; x++)
                for (int y = 0; y < Section.Height; y++)
                    for (int z = 0; z < Section.Depth; z++)
                    {
                        var block = section.Blocks[x, y, z];
                        if (block.IsAir) continue;
	                    
						var pos = section.GetGlobalPositionByArrayIndex(x, y, z).ToVector3();

						if (x > 0)
                            if (section.Blocks[x - 1, y, z].IsAir || section.Blocks[x - 1, y, z].IsTransparent)
                                sideBlocks.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x - 1, y, z].Light, section.Blocks[x - 1, y, z].SkyLight))));
                        if (x < Section.Width - 1)
                            if (section.Blocks[x + 1, y, z].IsAir || section.Blocks[x + 1, y, z].IsTransparent)
                                sideBlocks.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x + 1, y, z].Light, section.Blocks[x + 1, y, z].SkyLight))));
                        

                        if (y > 0)
                            if (section.Blocks[x, y - 1, z].IsAir || section.Blocks[x, y - 1, z].IsTransparent)
                                sideBlocks.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y - 1, z].Light, section.Blocks[x, y - 1, z].SkyLight))));
                        if (y < Section.Height - 1)
                            if (section.Blocks[x, y + 1, z].IsAir || section.Blocks[x, y + 1, z].IsTransparent)
                                sideBlocks.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y + 1, z].Light, section.Blocks[x, y + 1, z].SkyLight))));
                        

                        if (z > 0)
                            if (section.Blocks[x, y, z - 1].IsAir || section.Blocks[x, y, z - 1].IsTransparent)
                                sideBlocks.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y, z - 1].Light, section.Blocks[x, y, z - 1].SkyLight))));
                        if (z < Section.Depth - 1)
                            if (section.Blocks[x, y, z + 1].IsAir || section.Blocks[x, y, z + 1].IsTransparent)
                                sideBlocks.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, new Block(block.ID, block.Meta, section.Blocks[x, y, z + 1].Light, section.Blocks[x, y, z + 1].SkyLight))));
					}

            return sideBlocks;
        }

        private static List<VertexPositionColorHalfTexture> BuildFrontBorders(Section front, Section back)
		{
			var sideBlocks = new List<VertexPositionColorHalfTexture>();

			for (int x = 0; x < Section.Width; x++)
				for (int y = 0; y < Section.Height; y++)
				{
					var newSecBlock = back.Blocks[x, y, 0];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
					{
						var oldSecBlock = front.Blocks[x, y, Section.Depth - 1];
                        if (oldSecBlock.IsAir)
							continue;

                        var pos = front.GetGlobalPositionByArrayIndex(x, y, Section.Depth - 1).ToVector3();

						var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

						sideBlocks.AddRange(BlockVBO.CubeFaceFront(new BlockRenderInfo(pos, block)));
					}
				}
			
			return sideBlocks;
		}

        private static List<VertexPositionColorHalfTexture> BuildBackBorders(Section back, Section front)
		{
			var sideBlocks = new List<VertexPositionColorHalfTexture>();

			for (int x = 0; x < Section.Width; x++)
				for (int y = 0; y < Section.Height; y++)
				{
                    var newSecBlock = front.Blocks[x, y, Section.Depth - 1];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
					{
						var oldSecBlock = back.Blocks[x, y, 0];
                        if (oldSecBlock.IsAir)
							continue;

						var pos = back.GetGlobalPositionByArrayIndex(x, y, 0).ToVector3();

						var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

						sideBlocks.AddRange(BlockVBO.CubeFaceBack(new BlockRenderInfo(pos, block)));
					}
				}

			return sideBlocks;
		}

        private static List<VertexPositionColorHalfTexture> BuildRightBorders(Section left, Section right)
	    {
	        var sideBlocks = new List<VertexPositionColorHalfTexture>();

	        for (int y = 0; y < Section.Height; y++)
	            for (int z = 0; z < Section.Depth; z++)
	            {
	                var newSecBlock = left.Blocks[0, y, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
	                {
                        var oldSecBlock = right.Blocks[Section.Width - 1, y, z];
                        if (oldSecBlock.IsAir)
	                        continue;

                        var pos = right.GetGlobalPositionByArrayIndex(Section.Width - 1, y, z).ToVector3();

	                    var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

	                    sideBlocks.AddRange(BlockVBO.CubeFaceRight(new BlockRenderInfo(pos, block)));
	                }
	            }

	        return sideBlocks;
	    }

        private static List<VertexPositionColorHalfTexture> BuildLeftBorders(Section right, Section left)
		{
			var sideBlocks = new List<VertexPositionColorHalfTexture>();

			for (int y = 0; y < Section.Height; y++)
				for (int z = 0; z < Section.Depth; z++)
				{
					var newSecBlock = right.Blocks[Section.Width - 1, y, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
					{
                        var oldSecBlock = left.Blocks[0, y, z];
                        if (oldSecBlock.IsAir)
							continue;

						var pos = left.GetGlobalPositionByArrayIndex(0, y, z).ToVector3();

						var block = new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight);

						sideBlocks.AddRange(BlockVBO.CubeFaceLeft(new BlockRenderInfo(pos, block)));
					}
				}

			return sideBlocks;
		}

        private static List<VertexPositionColorHalfTexture> BuildTopBorders(Section top, Section bottom)
        {
            var sideBlocks = new List<VertexPositionColorHalfTexture>();

            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = top.Blocks[x, 0, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = bottom.GetGlobalPositionByArrayIndex(x, Section.Height - 1, z).ToVector3();

                        sideBlocks.AddRange(BlockVBO.CubeFaceTop(new BlockRenderInfo(pos, new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight))));
                    }
                }

            return sideBlocks;
        }

        private static List<VertexPositionColorHalfTexture> BuildBottomBorders(Section bottom, Section top)
        {
            var sideBlocks = new List<VertexPositionColorHalfTexture>();

            for (int x = 0; x < Section.Width; x++)
                for (int z = 0; z < Section.Depth; z++)
                {
                    var newSecBlock = bottom.Blocks[x, Section.Height - 1, z];
                    if (newSecBlock.IsAir || newSecBlock.IsTransparent)
                    {
                        var oldSecBlock = top.Blocks[x, 0, z];
                        if (oldSecBlock.IsAir)
                            continue;

                        var pos = top.GetGlobalPositionByArrayIndex(x, 0, z).ToVector3();

                        sideBlocks.AddRange(BlockVBO.CubeFaceBottom(new BlockRenderInfo(pos, new Block(oldSecBlock.ID, oldSecBlock.Meta, newSecBlock.Light, newSecBlock.SkyLight))));
                    }
                }

            return sideBlocks;
        }


        public void IncreasedBoundingBox(Vector3 vector3)
        {
            var topFrontLeft = GlobalPos + new Vector3(16 + vector3.X, 0 - vector3.Y, 0 - vector3.Z);
            var bottomBackRight = GlobalPos + new Vector3(0 - vector3.X, 16 + vector3.Y, 16 + vector3.Z);

            BoundingBox = new BoundingBox(topFrontLeft, bottomBackRight);
        }
	}
}