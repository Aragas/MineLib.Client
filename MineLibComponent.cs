using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL
{
    public abstract class MineLibComponent : IDisposable
    {
        protected Client Game { get; }
        protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;


        protected MineLibComponent(Client game) { Game = game; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public abstract void Dispose();
    }
}