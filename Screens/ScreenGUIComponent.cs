using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;

namespace MineLib.PGL.Screens
{
    public abstract class ScreenGUIComponent : IDisposable
    {
        protected Client Game { get; }
        protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        protected Rectangle ScreenRectangle => Game.GraphicsDevice.Viewport.Bounds;
        protected Screen Screen { get; private set; }


        protected ScreenGUIComponent(Client game, Screen screen) { Game = game; Screen = screen; }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(GameTime gameTime)
        {
#if DEBUG
            DebugComponent.GUIItemsDrawCalls++;
#endif
        }

        public abstract void Dispose();
    }
}