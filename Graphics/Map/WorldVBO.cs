using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Network.Data;
using MineLib.Network.Data.Anvil;

namespace MineLib.Client.Graphics.Map
{
    // -- +X - East
    // -- -X - West
    // -- +Z - South
    // -- -Z - North

	public class WorldVBO
	{
        GraphicsDevice GraphicsDevice { get; set; }
        BasicEffect BasicEffect { get; set; }

        ChunkVBO[] Chunks { get; set; }

		Thread _builder;
		public WorldVBO(GraphicsDevice device, List<Chunk> chunks)
		{
		    GraphicsDevice = device;
			BasicEffect = new BasicEffect(GraphicsDevice)
			{
				VertexColorEnabled = true,
				LightingEnabled = false,
				TextureEnabled = true,
				Texture = Client.Blocks
			};
			//BasicEffect.EnableDefaultLighting();

			Build(chunks);
		}

		private void Build(List<Chunk> chunks)
		{
			if (_builder == null || !_builder.IsAlive)
			{
				_builder = new Thread(() =>
				{
                    Chunks = new ChunkVBO[chunks.Count];

                    for (int i = 0; i < Chunks.Length; i++)
				    {
                        var coords = chunks[i].Coordinates;

                        var front   = FindChunk(chunks, coords + new Coordinates2D( 0,  1));
                        var back    = FindChunk(chunks, coords + new Coordinates2D( 0, -1));

                        var right   = FindChunk(chunks, coords + new Coordinates2D(-1,  0));
                        var left    = FindChunk(chunks, coords + new Coordinates2D( 1,  0));

				        Chunks[i] = new ChunkVBO(GraphicsDevice, chunks[i], front, back, left, right);
				    }
				}) { Name = "WorldBuilder" };
				_builder.Start();
			}
		}
        
        private static Chunk FindChunk(List<Chunk> chunks, Coordinates2D coords)
	    {
            for (int i = 0; i < chunks.Count; i++)
                if (chunks[i].Coordinates == coords)
                    return chunks[i];
            
            return null;
	    }

        public void UpdateChunk(Chunk center, Chunk front, Chunk back, Chunk left, Chunk right)
		{
            if (Chunks != null)
			    for (int i = 0; i < Chunks.Length; i++)
				    if (Chunks[i].Coordinates2D == center.Coordinates)
                        Chunks[i] = new ChunkVBO(GraphicsDevice, center, front, back, left, right);
		}

		public void UpdateSection(Section section)
		{
            /*
		    int index = section.ChunkPosition.Y;

		    for (int i = 0; i < Chunks.Length; i++)
		    {
                if (Chunks[i].Coordinates2D == new Coordinates2D(section.ChunkPosition.X, section.ChunkPosition.Z))
                    Chunks[i].UpdateSection(section);
		    }
            */
		}

	    public void Update()
		{
            if (Chunks != null)
                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
                        Chunks[i].Update();
		}

        public void Draw(Camera camera)
        {
            BasicEffect.Projection = camera.Projection;
            BasicEffect.View = camera.View;

            //GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.BlendState = new BlendState
            //{
            //    AlphaSourceBlend = Blend.SourceAlpha,
            //    AlphaDestinationBlend = Blend.InverseSourceAlpha,
            //    ColorSourceBlend = Blend.SourceAlpha,
            //    ColorDestinationBlend = Blend.InverseSourceAlpha,
            //};
            //GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (Chunks != null)
                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
                        Chunks[i].Draw(BasicEffect);
        }

		public void Draw(Camera camera, Ray ray)
		{
			BasicEffect.Projection = camera.Projection;
			BasicEffect.View = camera.View;

			//GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			//GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (Chunks != null)
                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
		                Chunks[i].Draw(BasicEffect, ray);
		}

        public void Draw(Camera camera, DRay rays)
        {
            BasicEffect.Projection = camera.Projection;
            BasicEffect.View = camera.View;

            //GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if(Chunks != null)
                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
                        Chunks[i].Draw(BasicEffect, rays);
        }
	}
}
