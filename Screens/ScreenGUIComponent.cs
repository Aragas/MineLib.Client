using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens
{
    public abstract class ScreenGUIComponent : IDisposable
    {
        protected Client Game { get; private set; }
        protected GraphicsDevice GraphicsDevice { get { return Game.GraphicsDevice; } }
        protected Screen Screen { get; private set; }
        protected Rectangle ScreenRectangle { get { return Game.GraphicsDevice.Viewport.Bounds; } }


        protected ScreenGUIComponent(Client game, Screen screen) { Game = game; Screen = screen; }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public abstract void Dispose();
    }
}