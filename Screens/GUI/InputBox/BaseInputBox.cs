using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Screens.GUI.Text;

namespace MineLib.PGL.Screens.GUI.InputBox
{
    public sealed class BaseInputBox : GUIInputBox
    {
        public static Vector2 Size = new Vector2(404, 44);
        public static Vector2 VanillaSize = new Vector2(400, 55); // Vanilla settings
        public static readonly float AspectRatio = VanillaSize.X / VanillaSize.Y;

        public static Vector2 FrameSize = new Vector2(2);

        private Rectangle FrameTopRectangle { get; set; }
        private Rectangle FrameBottomRectangle { get; set; }
        private Rectangle FrameLeftRectangle { get; set; }
        private Rectangle FrameRightRectangle { get; set; }

        private Rectangle DepthFrameRectangle { get; set; }


        private Texture2D FrameTexture { get; set; }
        private Texture2D DepthFrameTexture { get; set; }

        private InputBoxText InputBoxText { get; set; }


        public BaseInputBox(Client game, Screen screen, Rectangle pos, InputBoxEventHandler onEnter, Color style) : base(game, screen, onEnter, style)
        {
            InputBoxRectangle = pos;
            Size = new Vector2(InputBoxRectangle.Width, InputBoxRectangle.Height);

            FrameTopRectangle = new Rectangle(InputBoxRectangle.X, InputBoxRectangle.Y, (int)Size.X, (int)FrameSize.Y);
            FrameBottomRectangle = new Rectangle(InputBoxRectangle.X, (int)(InputBoxRectangle.Y + Size.Y - FrameSize.Y), (int)Size.X, (int)FrameSize.Y);
            FrameLeftRectangle = new Rectangle(InputBoxRectangle.X, InputBoxRectangle.Y, (int)FrameSize.X, (int)Size.Y);
            FrameRightRectangle = new Rectangle((int)(InputBoxRectangle.X + Size.X - FrameSize.X), InputBoxRectangle.Y, (int)FrameSize.X, (int)Size.Y);

            DepthFrameRectangle = new Rectangle(FrameTopRectangle.X + 2, FrameTopRectangle.Y + 2, FrameTopRectangle.Width - 2 - 2, FrameTopRectangle.Height);

            InputBoxText = new InputBoxText(Game, Screen, InputBoxRectangle, TextColor, TextShadowColor, this);


            BlackTexture = new Texture2D(GraphicsDevice, 1, 1);
            BlackTexture.SetData(new[] { Color.LightGray });

            FrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            FrameTexture.SetData(new[] { Color.Black });

            DepthFrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            DepthFrameTexture.SetData(new[] { Color.Gray });
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InputBoxText.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            SpriteBatch.Draw(BlackTexture, InputBoxRectangle, Rectangle.Empty, UsingColor);

            SpriteBatch.Draw(FrameTexture, FrameTopRectangle, new Rectangle(0, 0, (int)Size.X, (int)FrameSize.Y), UsingColor);
            SpriteBatch.Draw(FrameTexture, FrameBottomRectangle, new Rectangle(0, 0, (int)Size.X, (int)FrameSize.Y), UsingColor);
            SpriteBatch.Draw(FrameTexture, FrameLeftRectangle, new Rectangle(0, 0, (int)Size.Y, (int)FrameSize.X), UsingColor);
            SpriteBatch.Draw(FrameTexture, FrameRightRectangle, new Rectangle(0, 0, (int)Size.Y, (int)FrameSize.X), UsingColor);

            SpriteBatch.Draw(DepthFrameTexture, DepthFrameRectangle, new Rectangle(0, 0, (int)Size.X, (int)FrameSize.Y), UsingColor);

            SpriteBatch.End();

            InputBoxText.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (FrameTexture != null)
                FrameTexture.Dispose();

            if (DepthFrameTexture != null)
                DepthFrameTexture.Dispose();

            if(InputBoxText != null)
                InputBoxText.Dispose();
        }
    }
}
