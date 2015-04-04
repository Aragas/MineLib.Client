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
        int PrimitiveCount { get; set; }


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
			PrimitiveCount = 0;

            var verticies = new List<VertexPositionColorHalfTexture>();
			for (int i = 0; i < sectionsVbo.Count; i++)
			    if (sectionsVbo[i].IsFilled)
			    {
			        verticies.AddRange(sectionsVbo[i].Vertices);
                    sectionsVbo[i].Vertices = new List<VertexPositionColorHalfTexture>();
			    }
			

            PrimitiveCount = verticies.Count;
            if (PrimitiveCount <= 0)
                return;

            Buffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorHalfTexture), PrimitiveCount, BufferUsage.WriteOnly);
            Buffer.SetData(verticies.ToArray());
            verticies.Clear();


            //var indicies = new List<short>();
            //for (int i = 0; i < sectionsVbo.Count; i++) { }

            //IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, sizeof(short) * 0, BufferUsage.WriteOnly);
            //IndexBuffer.SetData(indicies.ToArray());
            //indicies.Clear();
		}

		public void Update() { }

        public void Draw(BasicEffect effect)
        {
            if(PrimitiveCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;
            foreach (var p in effect.CurrentTechnique.Passes)
            {
                p.Apply();
                
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, PrimitiveCount);
            }
        }

		public void Draw(BasicEffect effect, Ray ray)
		{
            if (PrimitiveCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            //GraphicsDevice.Indices = IndexBuffer;

			foreach (var p in effect.CurrentTechnique.Passes)
			{
				p.Apply();
                
                int offset = 0, length = 0;
			    for (int i = 0; i < Sections.Count; i++)
			    {
			        if (Sections[i].Vertices.Count <= 0)
			            continue;

			        length += Sections[i].Vertices.Count;

			        if (Sections[i].BoundingBox.Intersects(ray) != null)
			            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, offset, length);


			        offset += Sections[i].Vertices.Count;
			    }
			}
		}

        public void Draw(BasicEffect effect, DRay rays)
        {
            if (PrimitiveCount <= 0)
                return;

            GraphicsDevice.SetVertexBuffer(Buffer);
            foreach (var p in effect.CurrentTechnique.Passes)
            {
                p.Apply();

                int offset = 0, length = 0;
                for (int i = 0; i < Sections.Count; i++)
                {
                    if (Sections[i].Vertices.Count <= 0)
                        continue;

                    length += Sections[i].Vertices.Count;

                    for (int j = 0; j < rays.Rays.Count; j++)
                    {
                        var pos = new Microsoft.Xna.Framework.Vector3(Coordinates2D.X * 16, 16 * i, Coordinates2D.Z * 16);
                        var pos1 = pos - rays.CamPosition;
                        if (pos1.Length() > new Microsoft.Xna.Framework.Vector3(100).Length())
                            Sections[i].IncreasedBoundingBox(new Microsoft.Xna.Framework.Vector3(16));

                        if (Sections[i].BoundingBox.Intersects(rays.Rays[j]) != null)
                        {
                            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, offset, length);
                            break;
                        }
                    }

                    offset += Sections[i].Vertices.Count;
                }
            }
        }
	}
}