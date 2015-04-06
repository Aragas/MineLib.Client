using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Client.Graphics.Data;

using MineLib.Network.Data;
using MineLib.Network.Data.Anvil;

namespace MineLib.Client.Graphics.Map
{
	public class ChunkVBO
	{
	    GraphicsDevice GraphicsDevice { get; set; }
        IndexBuffer IndexBuffer { get; set; }
	    VertexBuffer Buffer { get; set; }

        int OpaqueVerticesCount { get; set; }
        int TotalVerticesCount { get { return Buffer.VertexCount; } }

        public List<SectionVBO> Sections { get; private set; }
        public Coordinates2D Coordinates2D { get; private set; }


		public ChunkVBO(GraphicsDevice device, Chunk chunk)
		{
		    GraphicsDevice = device;
			Coordinates2D = chunk.Coordinates;

            Sections = new List<SectionVBO>();
		    for (int i = 0; i < chunk.Sections.Length; i++)
		        if (chunk.Sections[i].IsFilled)
                    Sections.Add(new SectionVBO(chunk.Sections[i]));
		    
		    BindBuffer(Sections);
		}

		public ChunkVBO(GraphicsDevice device, List<SectionVBO> sections)
		{
			GraphicsDevice = device;
            Sections = sections;

            BindBuffer(Sections);
		}

        public ChunkVBO(GraphicsDevice device, Chunk center, Chunk front, Chunk back, Chunk left, Chunk right)
        {
            GraphicsDevice = device;
            Coordinates2D = center.Coordinates;

            Sections = new List<SectionVBO>();
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

                Sections.Add(new SectionVBO(center.Sections[i], front.Sections[i], back.Sections[i], left.Sections[i], right.Sections[i], top, bottom));
            }

            BindBuffer(Sections);
        }


        public void UpdateSection(Section section, Section front, Section back, Section left, Section right, Section top, Section bottom)
        {
            UpdateSection(new SectionVBO(section, front, back, left, right, top, bottom));
        }

        public void UpdateSection(SectionVBO section)
        {
            for (int i = 0; i < Sections.Count; i++)
                if (Sections[i].GlobalPos == section.GlobalPos)
                    Sections[i] = section;
            
            BindBuffer(Sections);
        }

        public void UpdateSection(int index, Section section, Section front, Section back, Section left, Section right, Section top, Section bottom)
		{
            UpdateSection(index, new SectionVBO(section, front, back, left, right, top, bottom));
		}

		public void UpdateSection(int index, SectionVBO section)
		{
            if(index < Sections.Count)
                return;

		    Sections[index] = section;

            BindBuffer(Sections);
		}


		private void BindBuffer(List<SectionVBO> sectionsVbo)
		{
            var opaque = new List<VertexPositionColorHalfTexture>();
            var transparent = new List<VertexPositionColorHalfTexture>();
			for (int i = 0; i < sectionsVbo.Count; i++)
			    if (sectionsVbo[i].IsFilled)
			    {
                    opaque.AddRange(sectionsVbo[i].OpaqueVertices);
                    transparent.AddRange(sectionsVbo[i].TransparentVertices);
                    sectionsVbo[i].ClearVerticies();
			    }

            OpaqueVerticesCount = opaque.Count;
            opaque.AddRange(transparent);
            transparent.Clear();

		    if (opaque.Count > 0)
		    {
                Buffer = new VertexBuffer(GraphicsDevice, VertexPositionColorHalfTexture.VertexDeclaration, opaque.Count, BufferUsage.WriteOnly);
                Buffer.SetData(opaque.ToArray());
		        opaque.Clear(); 
		    }

		    //var indicies = new List<short>();
            //for (int i = 0; i < sectionsVbo.Count; i++) { }

            //IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, sizeof(short) * 0, BufferUsage.WriteOnly);
            //IndexBuffer.SetData(indicies.ToArray());
            //indicies.Clear();
		}

		public void Update() { }

        public void DrawOpaque(BasicEffect effect)
        {
            if (Buffer == null || TotalVerticesCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            foreach (var p in effect.CurrentTechnique.Passes)
            {
                p.Apply();

                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, OpaqueVerticesCount / 3);
            }
        }

        public void DrawTransparent(BasicEffect effect)
        {
            if (Buffer == null || TotalVerticesCount <= 0 || Buffer.VertexCount - OpaqueVerticesCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            foreach (var p in effect.CurrentTechnique.Passes)
            {
                p.Apply();

                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, OpaqueVerticesCount, (TotalVerticesCount - OpaqueVerticesCount) / 3);
            }
        }


        public void DrawOpaque(BasicEffect effect, BoundingFrustum boundingFrustum)
        {
            if (Buffer == null || TotalVerticesCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            foreach (var p in effect.CurrentTechnique.Passes)
            {
                p.Apply();

                var offset = 0;
                foreach (var section in Sections)
                {
                    if (section.OpaqueVerticesCount > 0 && boundingFrustum.Intersects(section.BoundingBox))
                    {
                        WorldVBO.DrawingOpaqueSections++;
                        GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, offset, section.OpaqueVerticesCount / 3);
                    }

                    offset += section.OpaqueVerticesCount;
                }
            }
        }

        public void DrawTransparent(BasicEffect effect, BoundingFrustum boundingFrustum)
        {
            if (Buffer == null || TotalVerticesCount <= 0 || TotalVerticesCount - OpaqueVerticesCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

            foreach (var p in effect.CurrentTechnique.Passes)
            {
                p.Apply();

                var offset = OpaqueVerticesCount;
                foreach (var section in Sections)
                {
                    var transparent = section.TotalVerticesCount - section.OpaqueVerticesCount;
                    if (transparent > 0 && boundingFrustum.Intersects(section.BoundingBox))
                    {
                        WorldVBO.DrawingTransparentSections++;
                        GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, offset, transparent / 3);
                    }

                    offset += transparent;
                }
            }
        }
	}
}