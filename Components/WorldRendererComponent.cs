//#define BF  // - Best for Windows
//#define BFC // - Best for Android

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Data;
using MineLib.Core.Data.Anvil;

using MineLib.PGL.Data;
using MineLib.PGL.World;

namespace MineLib.PGL.Components
{
    public enum ShaderType
    {
        VertexPositionTexture,
        VertexPositionTextureLight,
    }


    public sealed class WorldRendererComponent : MineLibComponent
    {
#if DEBUG
        public static int Chunks;
	    public static int BuildedChunks;
        public static int DrawingOpaqueSections;
        public static int DrawingTransparentSections;
#endif

        public static ShaderType ShaderType = ShaderType.VertexPositionTextureLight;

        readonly Minecraft _minecraft;
        readonly CameraComponent _camera;

        readonly Effect _solidBlockEffect;
        readonly Effect _transparentBlockEffect;


        readonly List<ChunkVBO> _chunks = new List<ChunkVBO>();
        
        Task _builder;
        readonly CancellationTokenSource _cancellationToken;


	    public WorldRendererComponent(Client game, CameraComponent camera, Minecraft minecraft) : base(game)
	    {
            _minecraft = minecraft;

	        switch (ShaderType)
	        {
	            case ShaderType.VertexPositionTexture:

	                _solidBlockEffect = new BasicEffect(GraphicsDevice);
	                ((BasicEffect) _solidBlockEffect).Texture = Client.Blocks;
	                ((BasicEffect) _solidBlockEffect).TextureEnabled = true;

	                _transparentBlockEffect = new BasicEffect(GraphicsDevice);
	                ((BasicEffect) _transparentBlockEffect).Texture = Client.Blocks;
	                ((BasicEffect) _transparentBlockEffect).TextureEnabled = true;
	                break;


	            case ShaderType.VertexPositionTextureLight:

	                _solidBlockEffect = Game.Content.Load<Effect>("Effects\\GBuffer1");
	                _solidBlockEffect.Parameters["World"].SetValue(Matrix.Identity);
	                _solidBlockEffect.Parameters["SunColor"].SetValue(Color.White.ToVector4());
	                _solidBlockEffect.Parameters["Texture"].SetValue(Client.Blocks);
	                //_solidBlockEffect.Parameters["SpecularMap"].SetValue(Game.Content.Load<Texture2D>("Effects\\terrain_s"));
	                //_solidBlockEffect.Parameters["NormalMap"].SetValue(Game.Content.Load<Texture2D>("Effects\\terrain_nh"));

	                _transparentBlockEffect = Game.Content.Load<Effect>("Effects\\GBuffer1");
	                _transparentBlockEffect.Parameters["World"].SetValue(Matrix.Identity);
	                _transparentBlockEffect.Parameters["SunColor"].SetValue(Color.White.ToVector4());
	                _transparentBlockEffect.Parameters["Texture"].SetValue(Client.Blocks);
	                //_transparentBlockEffect.Parameters["SpecularMap"].SetValue(Game.Content.Load<Texture2D>("Effects\\terrain_s"));
	                //_transparentBlockEffect.Parameters["NormalMap"].SetValue(Game.Content.Load<Texture2D>("Effects\\terrain_nh"));
	                break;
	        }

                //foreach (var component in Game.Components)
                //    if (component is CameraComponentTC)
                //        _camera = component as CameraComponentTC;
                _camera = camera;

            _cancellationToken = new CancellationTokenSource();

            Game.Exiting += GameOnExiting;
	    }


        public void Build()
        {
            //try
            //{
                if (_minecraft != null && _minecraft.World != null && (_builder == null || _builder.IsCompleted))
                {
                    var chunks = new Chunk[_minecraft.World.Chunks.Count];
                    _minecraft.World.Chunks.CopyTo(chunks);
                    _builder = Task.Factory.StartNew(() => BuildWorker(chunks), _cancellationToken.Token);
                }
            //}
            //catch { }
            // TODO: Catch something
        }

        private void BuildWorker(Chunk[] chunks)
		{
            _chunks.Clear();
            for (int i = 0; i < chunks.Length; i++)
            {
                var coords = chunks[i].Coordinates;

                var front   = FindChunk(chunks, coords + new Coordinates2D( 0,  1));
                var back    = FindChunk(chunks, coords + new Coordinates2D( 0, -1));

                var right   = FindChunk(chunks, coords + new Coordinates2D(-1,  0));
                var left    = FindChunk(chunks, coords + new Coordinates2D( 1,  0));


                if (_cancellationToken.IsCancellationRequested)
                    return;
                else
                    _chunks.Add(new ChunkVBO(GraphicsDevice, chunks[i], front, back, left, right));
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
            for (int i = 0; i < _chunks.Count; i++)
                if (_chunks[i].Coordinates2D == center.Coordinates)
                    _chunks[i] = new ChunkVBO(GraphicsDevice, center, front, back, left, right);
	    }

	    public void UpdateSection(int sectionIndex, Chunk center, Chunk front, Chunk back, Chunk left, Chunk right)
	    {
	        // TODO: Do it normal

            for (int i = 0; i < _chunks.Count; i++)
                if (_chunks[i].Coordinates2D == center.Coordinates)
                    _chunks[i] = new ChunkVBO(GraphicsDevice, center, front, back, left, right);
	    }

	    public override void Update(GameTime gameTime)
		{
	        if (_minecraft != null && _minecraft.World != null)
	        {
	            if (_minecraft.World.Chunks.Count >= 441 && _minecraft.World.Chunks.Count > _chunks.Count)
	                Build();

#if DEBUG
                Chunks = _minecraft.World.Chunks.Count;
#endif
            }

#if DEBUG
            BuildedChunks = _chunks.Count;
#endif

            for (int i = 0; i < _chunks.Count; i++)
                if (_chunks[i] != null)
                    _chunks[i].Update();

	        if (_minecraft != null && _minecraft.World != null && !(_solidBlockEffect is BasicEffect) && !(_transparentBlockEffect is BasicEffect))
	        {
                var hours = (float) _minecraft.World.RealTime.Hours;
                _solidBlockEffect.Parameters["TimeOfDay"].SetValue(hours);
                _transparentBlockEffect.Parameters["TimeOfDay"].SetValue(hours);
	        }
		}

        public override void Draw(GameTime gameTime)
        {
#if DEBUG
#if  BF || BFC
            DebugComponent.BoundingFrustumEnabled    = true;
#else
            DebugComponent.BoundingFrustumEnabled    = false;
#endif
            DrawingOpaqueSections = 0;
            DrawingTransparentSections                  = 0;
#endif
            int vertices                                = 0;

            var solidBlockEffect = _solidBlockEffect as BasicEffect;
            if(solidBlockEffect != null)
                _camera.ApplyTo(solidBlockEffect);
            else
                _camera.ApplyTo(_solidBlockEffect);

            var transparentBlockEffect = _transparentBlockEffect as BasicEffect;
            if (transparentBlockEffect != null)
                _camera.ApplyTo(transparentBlockEffect);
            else
                _camera.ApplyTo(_transparentBlockEffect);


            foreach (var pass in _solidBlockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                for (int i = 0; i < _chunks.Count; i++)
                    if (_chunks[i] != null)
                    {
                        vertices += _chunks[i].TotalVerticesCount;
#if BF
                        _chunks[i].DrawOpaque(_camera.BoundingFrustum);
#elif BFC
                        _chunks[i].DrawOpaqueChunk(_camera.BoundingFrustum);
#else
                        _chunks[i].DrawOpaque();
#endif
                    }
            }


            foreach (var pass in _transparentBlockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                for (int i = 0; i < _chunks.Count; i++)
                    if (_chunks[i] != null)
                    {
                        vertices += _chunks[i].TotalVerticesCount;
#if BF
                        _chunks[i].DrawTransparent(_camera.BoundingFrustum);
#elif BFC
                        _chunks[i].DrawTransparentChunk(_camera.BoundingFrustum);
#else
                        _chunks[i].DrawTransparent();
#endif
                    }
            }

#if DEBUG
            DebugComponent.Vertices                  = vertices;
#endif
        }

        public void DrawR(GameTime gameTime)
        {
            _camera.ApplyTo(_solidBlockEffect);
            _camera.ApplyTo(_transparentBlockEffect);


            foreach (var pass in _solidBlockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (int i = 0; i < _chunks.Count; i++)
                    if (_chunks[i] != null)
                        _chunks[i].DrawOpaque(_camera.BoundingFrustum);
            }


            foreach (var pass in _transparentBlockEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (int i = 0; i < _chunks.Count; i++)
                    if (_chunks[i] != null)
                        _chunks[i].DrawTransparent(_camera.BoundingFrustum);
            }
        }

        private void GameOnExiting(object sender, EventArgs eventArgs)
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();
        }

        public override void Dispose()
        {
            if(_cancellationToken != null)
                _cancellationToken.Cancel();

            if (_chunks != null)
            {
                for (int i = 0; i < _chunks.Count; i++)
                    _chunks[i].Dispose();
                
                _chunks.Clear();
            }
        }
	}
}
