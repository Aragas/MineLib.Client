using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Data;
using MineLib.Core.Data.Anvil;

using MineLib.PGL.Components;
using MineLib.PGL.Extensions;

namespace MineLib.PGL.World
{
	public class ChunkVBO<T> : IDisposable where T : struct, IVertexType
	{
        GraphicsDevice GraphicsDevice { get; set; }
        //IndexBuffer IndexBuffer { get; set; }
	    VertexBuffer Buffer { get; set; }
        T ShaderType { get; set; }
        int[] OpaqueVertices { get; set; }
        int[] TransparentVertices { get; set; }

        BoundingBox BoundingBox { get; set; }

        public int TotalVerticesCount { get { return Buffer != null ? Buffer.VertexCount : 0; } }

	    public List<SectionVBO<T>> Sections { get; private set; }
        public Coordinates2D Coordinates2D { get; private set; }
        public byte[,] Biomes { get; private set; }

        public static Coordinates2D GetBiomeCoordinates(int index)
	    {
            return new Coordinates2D(index % 16, (index / 16) % 16);
	    }


		public ChunkVBO(GraphicsDevice device, Chunk chunk)
		{
		    GraphicsDevice = device;
			Coordinates2D = chunk.Coordinates;

            var size = (int)Math.Sqrt(chunk.Biomes.Length);
            Biomes = new byte[size, size];
            for (int i = 0; i < chunk.Biomes.Length; i++)
            {
                var coords = GetBiomeCoordinates(i);
                Biomes[coords.X, coords.Z] = chunk.Biomes[i];
            }

            Sections = new List<SectionVBO<T>>();
		    for (int i = 0; i < chunk.Sections.Length; i++)
		        if (chunk.Sections[i].IsFilled)
                    Sections.Add(new SectionVBO<T>(chunk.Sections[i]));

            if (Sections.Count > 0)
                BoundingBox = new BoundingBox(Sections[0].BoundingBox.Min, Sections[Sections.Count - 1].BoundingBox.Max);

            BindBuffer(Sections);
		}

		public ChunkVBO(GraphicsDevice device, List<SectionVBO<T>> sections)
		{
			GraphicsDevice = device;
            Sections = sections;

            if (Sections.Count > 0)
                BoundingBox = new BoundingBox(Sections[0].BoundingBox.Min, Sections[Sections.Count - 1].BoundingBox.Max);

            BindBuffer(Sections);
		}

        public ChunkVBO(GraphicsDevice device, Chunk center, Chunk front, Chunk back, Chunk left, Chunk right)
        {
            GraphicsDevice = device;
            Coordinates2D = center.Coordinates;

            var size = (int) Math.Sqrt(center.Biomes.Length);
            Biomes = new byte[size, size];
            for (int i = 0; i < center.Biomes.Length; i++)
            {
                var coords = GetBiomeCoordinates(i);
                Biomes[coords.X, coords.Z] = center.Biomes[i];
            }

            Sections = new List<SectionVBO<T>>();
            for (int i = 0; i < center.Sections.Length; i++)
            {
                if (!center.Sections[i].IsFilled)
                    continue;

                if (front   == null)    front   = new Chunk(Coordinates2D.Zero);
                if (back    == null)    back    = new Chunk(Coordinates2D.Zero);
                if (left    == null)    left    = new Chunk(Coordinates2D.Zero);
                if (right   == null)    right   = new Chunk(Coordinates2D.Zero);
                var top = new Section(Position.Zero); if (i > 0) top = center.Sections[i - 1];
                var bottom = new Section(Position.Zero);    if (i < center.Sections.Length - 1) bottom = center.Sections[i + 1];

                Sections.Add(new SectionVBO<T>(center.Sections[i], front.Sections[i], back.Sections[i], left.Sections[i], right.Sections[i], top, bottom));
            }

            if(Sections.Count > 0)
                BoundingBox = new BoundingBox(Sections[0].BoundingBox.Min, Sections[Sections.Count - 1].BoundingBox.Max);

            BindBuffer(Sections);
        }


        public void UpdateSection(Section section, Section front, Section back, Section left, Section right, Section top, Section bottom)
        {
            UpdateSection(new SectionVBO<T>(section, front, back, left, right, top, bottom));
        }

        public void UpdateSection(SectionVBO<T> section)
        {
            for (int i = 0; i < Sections.Count; i++)
                if (Sections[i].GlobalPos == section.GlobalPos)
                    Sections[i] = section;
            
            BindBuffer(Sections);
        }

        public void UpdateSection(int index, Section section, Section front, Section back, Section left, Section right, Section top, Section bottom)
		{
            UpdateSection(index, new SectionVBO<T>(section, front, back, left, right, top, bottom));
		}

		public void UpdateSection(int index, SectionVBO<T> section)
		{
            if(index < Sections.Count)
                return;

		    Sections[index] = section;

            BindBuffer(Sections);
		}


	    private void BindBuffer(List<SectionVBO<T>> sectionsVbo)
	    {
	        var opaque = new List<T>();
            OpaqueVertices = new int[sectionsVbo.Count];

            var transparent = new List<T>();
            TransparentVertices = new int[sectionsVbo.Count];

            for (int i = 0; i < sectionsVbo.Count; i++)
	            if (sectionsVbo[i].IsFilled)
	            {
                    // REWRITE: Blocking thread
                    opaque.AddRange(sectionsVbo[i].OpaqueVerticesArray);
	                OpaqueVertices[i] = sectionsVbo[i].OpaqueVerticesCount;

                    transparent.AddRange(sectionsVbo[i].TransparentVerticesArray);
                    TransparentVertices[i] = sectionsVbo[i].TransparentVerticesCount;

                    sectionsVbo[i].ClearVerticies();
	            }

	        //OpaqueVerticesCount = opaque.Count;
	        opaque.AddRange(transparent);
	        transparent.Clear();

	        if (opaque.Count > 0)
	        {
	            Buffer = new VertexBuffer(GraphicsDevice, ShaderType.VertexDeclaration, opaque.Count, BufferUsage.WriteOnly);
	            Buffer.SetData(opaque.ToArray());
	            opaque.Clear();
	        }
	        //
	        //
	        //var indicies = new List<int>();
	        //for (int i = 0; i < sectionsVbo.Count; i++)
	        //    indicies.AddRange(sectionsVbo[i].Indicies);
	        //
	        //if (indicies.Count > 0)
	        //{
	        //    IndexBuffer = new IndexBuffer(GraphicsDevice, typeof (int), indicies.Count, BufferUsage.WriteOnly);
	        //    IndexBuffer.SetData(indicies.ToArray());
	        //    indicies.Clear();
	        //}
	    }

	    public void Update() { }


        public void DrawOpaque()
        {
            int opaqueVerticesCount = 0;
            foreach (var opaqueVertex in OpaqueVertices)
                opaqueVerticesCount += opaqueVertex;

            if (Buffer == null || TotalVerticesCount <= 0) 
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, opaqueVerticesCount / 3);
            //GraphicsDevice.DrawIndexed Primitives(PrimitiveType.TriangleList, 0, 0, IndexBuffer.IndexCount, 0, IndexBuffer.IndexCount / 3);
        }

        public void DrawOpaque(BoundingFrustum boundingFrustum)
        {
            if (Buffer == null || TotalVerticesCount <= 0) 
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            int offset = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                if (OpaqueVertices[i] > 0 && boundingFrustum.FastIntersect(Sections[i].BoundingBox))
                {
#if DEBUG
                    WorldRendererComponent<T>.DrawingOpaqueSections++;
#endif
                    GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, offset, OpaqueVertices[i] / 3);
                    //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, section.OpaqueVerticesCount, offset, section.OpaqueVerticesCount / 3);
                }

                offset += OpaqueVertices[i];
            }
        }

        public void DrawOpaqueChunk(BoundingFrustum boundingFrustum)
        {
            if(!boundingFrustum.FastIntersect(BoundingBox))
                return;

#if DEBUG
                    WorldRendererComponent<T>.DrawingOpaqueSections++;
#endif

            int opaqueVerticesCount = 0;
            foreach (var opaqueVertex in OpaqueVertices)
                opaqueVerticesCount += opaqueVertex;

            if (Buffer == null || TotalVerticesCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, opaqueVerticesCount / 3);
            //GraphicsDevice.DrawIndexed Primitives(PrimitiveType.TriangleList, 0, 0, IndexBuffer.IndexCount, 0, IndexBuffer.IndexCount / 3);
        }


        public void DrawTransparent()
        {
            int opaqueVerticesCount = 0;
            foreach (var opaqueVertex in OpaqueVertices)
                opaqueVerticesCount += opaqueVertex;

            if (Buffer == null || TotalVerticesCount <= 0 || Buffer.VertexCount - opaqueVerticesCount <= 0) 
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
			GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, opaqueVerticesCount, (TotalVerticesCount - opaqueVerticesCount) / 3);
        }

        public void DrawTransparent(BoundingFrustum boundingFrustum)
        {
            int opaqueVerticesCount = 0;
            foreach (var opaqueVertex in OpaqueVertices)
                opaqueVerticesCount += opaqueVertex;

            if (Buffer == null || TotalVerticesCount <= 0 || TotalVerticesCount - opaqueVerticesCount <= 0) 
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            int offset = opaqueVerticesCount;
            for (int i = 0; i < Sections.Count; i++)
            {
                var section = Sections[i];
                if (TransparentVertices[i] > 0 && boundingFrustum.FastIntersect(section.BoundingBox))
                {
#if DEBUG
                    WorldRendererComponent<T>.DrawingTransparentSections++;
#endif
                    GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, offset, TransparentVertices[i] / 3);
                    //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, transparentCount , offset, transparentCount / 3);
                }

                offset += TransparentVertices[i];
            }
        }

        public void DrawTransparentChunk(BoundingFrustum boundingFrustum)
        {
            if (!boundingFrustum.FastIntersect(BoundingBox))
                return;

#if DEBUG
                    WorldRendererComponent<T>.DrawingTransparentSections++;
#endif

            int opaqueVerticesCount = 0;
            foreach (var opaqueVertex in OpaqueVertices)
                opaqueVerticesCount += opaqueVertex;

            if (Buffer == null || TotalVerticesCount <= 0 || Buffer.VertexCount - opaqueVerticesCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, opaqueVerticesCount, (TotalVerticesCount - opaqueVerticesCount) / 3);
        }

        public void Dispose()
	    {
            if (Buffer != null)
                Buffer.Dispose();

	        if (Sections != null)
	        {
	            for (int i = 0; i < Sections.Count; i++)
	                Sections[i].Dispose();
	            
	            Sections.Clear();
	        }
	    }
	}
}