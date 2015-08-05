using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PCL.Graphics
{
    public abstract class MineLibGameComponent : IDisposable
    {
        protected Client Game { get; private set; }
        protected GraphicsDevice GraphicsDevice { get { return Game.GraphicsDevice; } }


        protected MineLibGameComponent(Client game) { Game = game; }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public abstract void Dispose();
    }
}