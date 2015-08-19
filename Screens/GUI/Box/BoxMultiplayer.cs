using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.GUI.Box
{
    public sealed class BoxMultiplayer : GUIBox
    {
        private event Action OnButton;

        private Texture2D ServersTexture { get; set; }
        private Rectangle ServersRectangle { get; set; }

        private Texture2D ServersFrameTexture { get; set; }
        private Vector2 ServersFrameSize { get { return new Vector2(2); } }
        private Rectangle ServersFrameTopRectangle { get; set; }
        private Rectangle ServersFrameBottomRectangle { get; set; }
        private Rectangle ServersFrameLeftRectangle { get; set; }
        private Rectangle ServersFrameRightRectangle { get; set; }


        public BoxMultiplayer(Client game, Screen screen, Rectangle rect, Action onButton, Color style) : base(game, screen, rect, "Servers", style)
        {
            OnButton += onButton;

            ServersTexture = new Texture2D(GraphicsDevice, 1, 1);
            ServersTexture.SetData(new[] { new Color(150, 150, 150, 255) });

            ServersFrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            ServersFrameTexture.SetData(new[] { new Color(0, 0, 0, 255) });

            ServersRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                BoxRectangle.Y + BaseYOffset,
                BoxRectangle.Width - (int)(BaseXOffset * 2.0f),
                BoxRectangle.Height - Button.ButtonRectangle.Height - (int)(BaseYOffset * 2.0f) - ElementOffset);

            ServersFrameTopRectangle = new Rectangle(ServersRectangle.X, ServersRectangle.Y, ServersRectangle.Width, (int)ServersFrameSize.Y);
            ServersFrameBottomRectangle = new Rectangle(ServersRectangle.X, (int)(ServersRectangle.Y + ServersRectangle.Height - ServersFrameSize.Y), ServersRectangle.Width, (int)ServersFrameSize.Y);
            ServersFrameLeftRectangle = new Rectangle(ServersRectangle.X, ServersRectangle.Y, (int)ServersFrameSize.X, ServersRectangle.Height);
            ServersFrameRightRectangle = new Rectangle((int)(ServersRectangle.X + ServersRectangle.Width - ServersFrameSize.X), ServersRectangle.Y, (int)ServersFrameSize.X, ServersRectangle.Height);

        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            SpriteBatch.Draw(ServersTexture, ServersRectangle, Rectangle.Empty, Color.White);

            SpriteBatch.Draw(ServersFrameTexture, ServersFrameTopRectangle,     new Rectangle(0, 0, BoxRectangle.X, (int)ServersFrameSize.Y), Color.White);
            SpriteBatch.Draw(ServersFrameTexture, ServersFrameBottomRectangle,  new Rectangle(0, 0, BoxRectangle.X, (int)ServersFrameSize.Y), Color.White);
            SpriteBatch.Draw(ServersFrameTexture, ServersFrameLeftRectangle,    new Rectangle(0, 0, BoxRectangle.Y, (int)ServersFrameSize.X), Color.White);
            SpriteBatch.Draw(ServersFrameTexture, ServersFrameRightRectangle,   new Rectangle(0, 0, BoxRectangle.Y, (int)ServersFrameSize.X), Color.White);

            SpriteBatch.End();
        }

        public override void Dispose()
        {
            if (OnButton != null)
                foreach (var @delegate in OnButton.GetInvocationList())
                    OnButton -= (Action) @delegate;
            
            if (ServersTexture != null)
                ServersTexture.Dispose();

            if (ServersFrameTexture != null)
                ServersFrameTexture.Dispose();
        }

        protected override void OnButtonPressed()
        {
            if (OnButton != null)
                OnButton();
        }
    }
}
