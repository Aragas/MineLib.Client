#if DEBUG

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.BMFont;

namespace MineLib.PGL.Components
{
    public sealed class DebugComponent : DrawableGameComponent
    {
        public static bool BoundingFrustumEnabled = false;

        public static Vector3 PlayerPos;
        public static Vector3 CameraPos;

        public static int Vertices;


        readonly SpriteBatch _spriteBatch;
        readonly FontRenderer _fontRenderer;

        int _frameRate;
        int _frameCounter;
        double _elapsedTime;


        public DebugComponent(Game game) : base(game)
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fontRenderer = new FontRenderer(GraphicsDevice, "PixelUnicode");
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_elapsedTime >= 1000f)
            {
                _elapsedTime -= 1000f;
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }

        private const int Height = 54;
        public override void Draw(GameTime gameTime)
        {
            var width = (int) (Game.GraphicsDevice.Viewport.Width * 0.5f);

            _frameCounter++;

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            var pos =                                               PlayerPos.ToString();
            var fps = string.Format("FPS  : {0}",                   _frameRate);
            var ram = string.Format("RAM : {0} (KB)",               GC.GetTotalMemory(false) / 1024);

            var gpu = string.Format("Chunks : {0}",                 WorldRendererComponent.Chunks);
            var cpu = string.Format("Build Chunks : {0}",           WorldRendererComponent.BuildedChunks);
            var opa = string.Format("Opaque Sections : {0}",        WorldRendererComponent.DrawingOpaqueSections);
            var tra = string.Format("Transparent Sections : {0}",   WorldRendererComponent.DrawingTransparentSections);
            var cam =                                               CameraPos.ToString();
            var ver = string.Format("Verticies : {0}",              Vertices);

            _fontRenderer.DrawText(_spriteBatch, pos, new Rectangle(6,  1,      width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, pos, new Rectangle(5,  0,      width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, fps, new Rectangle(6,  31,     width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, fps, new Rectangle(5,  30,     width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(6,  61,     width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(5,  60,     width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, gpu, new Rectangle(6,  91,     width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, gpu, new Rectangle(5,  90,     width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, cpu, new Rectangle(6,  121,    width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, cpu, new Rectangle(5,  120,    width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, cam, new Rectangle(width + 1,  1, width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, cam, new Rectangle(width,      0, width, Height), Color.White);

            if (BoundingFrustumEnabled)
            {
                _fontRenderer.DrawText(_spriteBatch, opa, new Rectangle(6, 151, width, Height), Color.Black);
                _fontRenderer.DrawText(_spriteBatch, opa, new Rectangle(5, 150, width, Height), Color.White);

                _fontRenderer.DrawText(_spriteBatch, tra, new Rectangle(6, 181, width, Height), Color.Black);
                _fontRenderer.DrawText(_spriteBatch, tra, new Rectangle(5, 180, width, Height), Color.White);
            }

            _fontRenderer.DrawText(_spriteBatch, ver, new Rectangle(6, 211, width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, ver, new Rectangle(5, 210, width, Height), Color.White);

            _spriteBatch.End();
        }
    }
}

#endif