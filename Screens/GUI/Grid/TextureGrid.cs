using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.GUI.Grid
{
    public class TextureGrid : GUIGrid
    {
        public TextureGrid(Client game, Screen screen, Rectangle backgroundRectangle, Texture2D backgroundTexture) : base(game, screen)
        {
            BackgroundRectangle = backgroundRectangle;
            BackgroundTexture = backgroundTexture;

            FrameTopRectangle = new Rectangle(BackgroundRectangle.X, BackgroundRectangle.Y, BackgroundRectangle.Width, FrameSize.Y);
            FrameBottomRectangle = new Rectangle(BackgroundRectangle.X, BackgroundRectangle.Y + BackgroundRectangle.Height - FrameSize.Y, BackgroundRectangle.Width, FrameSize.Y);
            FrameLeftRectangle = new Rectangle(BackgroundRectangle.X, BackgroundRectangle.Y, FrameSize.X, BackgroundRectangle.Height);
            FrameRightRectangle = new Rectangle(BackgroundRectangle.X + BackgroundRectangle.Width - FrameSize.X, BackgroundRectangle.Y, FrameSize.X, BackgroundRectangle.Height);

            FrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            FrameTexture.SetData(new[] { new Color(0, 0, 0, 255) });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            SpriteBatch.Draw(BackgroundTexture, BackgroundRectangle, Rectangle.Empty, Color.White);

            SpriteBatch.Draw(FrameTexture, FrameTopRectangle, new Rectangle(0, 0, BackgroundRectangle.X, FrameSize.Y), Color.White);
            SpriteBatch.Draw(FrameTexture, FrameBottomRectangle, new Rectangle(0, 0, BackgroundRectangle.X, FrameSize.Y), Color.White);
            SpriteBatch.Draw(FrameTexture, FrameLeftRectangle, new Rectangle(0, 0, BackgroundRectangle.Y, FrameSize.X), Color.White);
            SpriteBatch.Draw(FrameTexture, FrameRightRectangle, new Rectangle(0, 0, BackgroundRectangle.Y, FrameSize.X), Color.White);

            SpriteBatch.End();
        }

        public override void Dispose()
        {
            base.Dispose();

            BackgroundTexture?.Dispose();
            FrameTexture?.Dispose();
        }
    }
}
