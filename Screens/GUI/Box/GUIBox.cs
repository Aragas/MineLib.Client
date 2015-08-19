using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Screens.GUI.Button;
using MineLib.PGL.Screens.GUI.InputBox;

namespace MineLib.PGL.Screens.GUI.Box
{
    public abstract class GUIBox : GUIItem
    {
        public static Vector2 BoxSize { get { return new Vector2(450, 400); } }
        
        public float AspectRatio { get; private set; }
        public float ButtonScale { get; private set; }

        public Color UsingColor { get; set; }


        protected int BaseXOffset { get { return (int) (10 * AspectRatio); } }
        protected int BaseYOffset { get { return (int) (10 * AspectRatio); } }

        protected int BoxWidthCenter { get { return BoxRectangle.X + (int)(BoxRectangle.Width * 0.5f); } }
        protected int BoxHeightCenter { get { return BoxRectangle.Y + (int)(BoxRectangle.Height * 0.5f); } }

        protected int ElementOffset { get { return (int) (10 * AspectRatio); } }
        protected int ElementWidth { get { return (int)((BoxRectangle.Height - (BaseYOffset * 2)) * AspectRatio); } }
        protected int ElementHeight { get { return (int)(ElementWidth * (1 / BaseInputBox.AspectRatio)); } }

        protected int ButtonWidth { get { return (int)((BoxRectangle.Height - (BaseYOffset * 6)) * (AspectRatio - 0.15f)); } }
        protected int ButtonHeight { get { return (int)(ButtonWidth * (1 / ButtonMenuHalf.AspectRatio)); } }
        protected int ButtonXOffset { get { return (int) (30 * AspectRatio); } }
        protected int ButtonYOffset { get { return (int) (10 * AspectRatio); } }
        
        protected GUIButton Button { get; private set; }

        protected Rectangle BoxRectangle { get; private set; }
        

        private Vector2 BoxFrameSize { get { return new Vector2(2); } }
        private Rectangle BoxFrameTopRectangle { get; set; }
        private Rectangle BoxFrameBottomRectangle { get; set; }
        private Rectangle BoxFrameLeftRectangle { get; set; }
        private Rectangle BoxFrameRightRectangle { get; set; }

        private Rectangle GradientDownRectangle { get; set; }
        private Rectangle GradientRightRectangle { get; set; }

        private Texture2D BoxFrameTexture { get; set; }
        private Texture2D BoxTexture { get; set; }
        private Texture2D GradientDownTexture { get; set; }
        private Texture2D GradientRightTexture { get; set; }

        
        protected GUIBox(Client game, Screen screen, Rectangle boxRectangle, string buttonText, Color style) : base(game, screen)
        {
            BoxRectangle = boxRectangle;
            UsingColor = style;

            var scaleX = BoxSize.X / BoxRectangle.Width;
            var scaleY = BoxSize.Y / BoxRectangle.Height;
            AspectRatio = scaleX/scaleY;

            BoxFrameTopRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y, BoxRectangle.Width, (int)BoxFrameSize.Y);
            BoxFrameBottomRectangle = new Rectangle(BoxRectangle.X, (int)(BoxRectangle.Y + BoxRectangle.Height - BoxFrameSize.Y), BoxRectangle.Width, (int)BoxFrameSize.Y);
            BoxFrameLeftRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y, (int)BoxFrameSize.X, BoxRectangle.Height);
            BoxFrameRightRectangle = new Rectangle((int)(BoxRectangle.X + BoxRectangle.Width - BoxFrameSize.X), BoxRectangle.Y, (int)BoxFrameSize.X, BoxRectangle.Height);

            var buttonRectangle = new Rectangle(
                BoxWidthCenter - (int)(ButtonWidth * 0.5f),
                BoxRectangle.Y + BoxRectangle.Height - ButtonHeight - ButtonYOffset,
                ButtonWidth,
                ButtonHeight);
            ButtonScale = buttonRectangle.Width / ButtonMenuHalf.VanillaSize.X;

            if (!string.IsNullOrEmpty(buttonText))
            {
                Button = new ButtonMenuHalf(Game, Screen, buttonText, buttonRectangle, null, UsingColor);
                Button.OnButtonPressed += OnButtonPressed;
            }


            BoxTexture = new Texture2D(GraphicsDevice, 1, 1);
            BoxTexture.SetData(new[] { new Color(100, 100, 100, 240) });

            BoxFrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            BoxFrameTexture.SetData(new[] { new Color(0, 0, 0, 240) });

            var scale = (int)(1 * (1 / ButtonScale));
            GradientDownTexture = CreateGradientDown(BoxRectangle.Width + 5 - scale, 5);
            GradientRightTexture = CreateGradientRight(5, BoxRectangle.Height + 5 - scale);
            GradientDownRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y + BoxRectangle.Height - scale, GradientDownTexture.Width, GradientDownTexture.Height);
            GradientRightRectangle = new Rectangle(BoxRectangle.X + BoxRectangle.Width - scale, BoxRectangle.Y, GradientRightTexture.Width, GradientRightTexture.Height);
        }

        public virtual void AddToGUIItemMultiController(GUIItemMultiController guiButtonMultiController)
        {
            guiButtonMultiController.AddGUIItem(Button);
        }

        public override void Update(GameTime gameTime)
        {
            //Button.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            SpriteBatch.Draw(GradientDownTexture, GradientDownRectangle,    UsingColor);
            SpriteBatch.Draw(GradientRightTexture, GradientRightRectangle,  UsingColor);

            SpriteBatch.Draw(BoxTexture, BoxRectangle,                      UsingColor);

            SpriteBatch.Draw(BoxFrameTexture, BoxFrameTopRectangle,         UsingColor);
            SpriteBatch.Draw(BoxFrameTexture, BoxFrameBottomRectangle,      UsingColor);
            SpriteBatch.Draw(BoxFrameTexture, BoxFrameLeftRectangle,        UsingColor);
            SpriteBatch.Draw(BoxFrameTexture, BoxFrameRightRectangle,       UsingColor);

            SpriteBatch.End();

            Button.Draw(gameTime);
        }

        public override void Dispose()
        {
            Button.Dispose();
        }

        protected abstract void OnButtonPressed();


        private Texture2D CreateGradientDown(int width, int height)
        {
            var backgroundTex = new Texture2D(GraphicsDevice, width, height);
            var bgc = new Color[width * height];

            for (int i = 0; i < bgc.Length; i++)
            {
                if (i / width == 0)
                    bgc[i] = new Color(0, 0, 0, 165);

                if (i / width == 1)
                    bgc[i] = new Color(0, 0, 0, 135);

                if (i / width == 2)
                    bgc[i] = new Color(0, 0, 0, 105);

                if (i / width == 3)
                    bgc[i] = new Color(0, 0, 0, 75);

                if (i / width == 4)
                    bgc[i] = new Color(0, 0, 0, 45);
            }
            backgroundTex.SetData(bgc);
            return backgroundTex;
        }

        private Texture2D CreateGradientRight(int width, int height)
        {
            var backgroundTex = new Texture2D(GraphicsDevice, width, height);
            var bgc = new Color[width * height];

            for (int i = 0; i < bgc.Length; i++)
            {
                if (i % width == 0)
                    bgc[i] = new Color(0, 0, 0, 165);

                if (i % width == 1)
                    bgc[i] = new Color(0, 0, 0, 135);

                if (i % width == 2)
                    bgc[i] = new Color(0, 0, 0, 105);

                if (i % width == 3)
                    bgc[i] = new Color(0, 0, 0, 75);

                if (i % width == 4)
                    bgc[i] = new Color(0, 0, 0, 45);
            }
            backgroundTex.SetData(bgc);
            return backgroundTex;
        }
    }
}
