using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MineLib.Network.Data;
using MineLib.Network.Data.Anvil;
using MineLib.PCL.Data.BigData;

namespace MineLib.PCL.Graphics.Map
{
    // -- +X - East
    // -- -X - West
    // -- +Z - South
    // -- -Z - North

	public class WorldVBO
	{
        public World World { get; set; }

        Effect SolidBlockEffect;

        GraphicsDevice GraphicsDevice { get; set; }
        BasicEffect BasicEffect { get; set; }

        private ThreadSafeList<ChunkVBO> Chunks { get; set; }

        Task _builder;

	    public WorldVBO(GraphicsDevice device)
	    {
	        GraphicsDevice = device;

            SolidBlockEffect = Client.ContentManager.Load<Effect>("SolidBlockEffect");
            SolidBlockEffect.Parameters["World"].SetValue(Matrix.Identity);
            SolidBlockEffect.Parameters["SunColor"].SetValue(Color.White.ToVector4());
            SolidBlockEffect.Parameters["CubeTexture"].SetValue(Client.Blocks);
	    }

        public void Build()
        {
            try
            {
                if ((_builder == null) || _builder.IsCompleted)
                {
                    var chunks = new Chunk[World.Chunks.Count];
                    World.Chunks.CopyTo(chunks);
                    _builder = Task.Factory.StartNew(() => BuildWorker(chunks));
                }
            }
            catch
            {
            }
        }

        public void Build(List<Chunk> chunks)
        {
            try
            {
                if (_builder == null || _builder.IsCompleted)
                    _builder = Task.Factory.StartNew(() => BuildWorker(chunks.ToArray()));
            }
            catch { }
        }

	    private void BuildWorker(Chunk[] chunks)
	    {
	        Chunks = new ChunkVBO[chunks.Length];

	        for (int i = 0; i < Chunks.Length; i++)
	        {
	            var coords = chunks[i].Coordinates;

	            var front = FindChunk(chunks, coords + new Coordinates2D(0, 1));
	            var back = FindChunk(chunks, coords + new Coordinates2D(0, -1));

	            var right = FindChunk(chunks, coords + new Coordinates2D(-1, 0));
	            var left = FindChunk(chunks, coords + new Coordinates2D(1, 0));

	            Chunks[i] = new ChunkVBO(GraphicsDevice, chunks[i], front, back, left, right);
	        }
	    }

	    private static Chunk FindChunk(Chunk[] chunks, Coordinates2D coords)
	    {
            for (int i = 0; i < chunks.Length; i++)
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

        public void UpdateSection(int sectionIndex, Chunk center, Chunk front, Chunk back, Chunk left, Chunk right)
		{
            // TODO: Do it normal
            
            if (Chunks != null)
                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i].Coordinates2D == center.Coordinates)
                        Chunks[i] = new ChunkVBO(GraphicsDevice, center, front, back, left, right);
		}

	    public void Update()
		{
            if (Chunks != null)
                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
                        Chunks[i].Update();
		}

        public static int DrawingOpaqueSections;
        public static int DrawingTransparentSections;
        public void Draw(Camera camera)
        {
            DrawingOpaqueSections = 0;
            DrawingTransparentSections = 0;

            BasicEffect.Projection              = camera.Projection;
            BasicEffect.View                    = camera.View;

            var preDepthStencilState            = GraphicsDevice.DepthStencilState;
            var preSamplerState                 = GraphicsDevice.SamplerStates[0];

            GraphicsDevice.DepthStencilState    = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0]     = SamplerState.PointClamp;


            if (Chunks != null)
            {
                var boundingFrustum = camera.BoundingFrustum;

                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
                        Chunks[i].DrawOpaque(BasicEffect, boundingFrustum);

                for (int i = 0; i < Chunks.Length; i++)
                    if (Chunks[i] != null)
                        Chunks[i].DrawTransparent(BasicEffect, boundingFrustum);
            }


            GraphicsDevice.DepthStencilState    = preDepthStencilState;
            GraphicsDevice.SamplerStates[0]     = preSamplerState;
        }
	}
}
