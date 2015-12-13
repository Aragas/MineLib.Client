using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Screens.GUI.Button;

namespace MineLib.PGL.Screens.GUI.Box
{
    public struct Grid
    {
        public int OffsetX => (int) (10 * Scaling);
        public int OffsetY => (int)(10 * Scaling);
        public int Width => Source.Width - OffsetX * 2;
        public int Height => Source.Height - OffsetY * 2;

        private Rectangle Source { get; }
        private float Scaling { get; set; }

        public Grid(Rectangle rectangle, float scale)
        {
            Source = rectangle;
            Scaling = scale;
        }

        public void Scale(float scale)
        {
            Scaling = scale;
        }
    }

    public abstract class GUIBox : GUIItem
    {
        protected Rectangle BoxRectangle { get; }
        protected Color UsingColor { get; }
        
        protected Grid BoxGrid { get; }

        private List<GUIItem> GUIItems { get; } = new List<GUIItem>();
        
        #region Frame & Gradient

        private Point BoxFrameSize { get; } = new Point(2, 2);
        private Rectangle BoxFrameTopRectangle { get; }
        private Rectangle BoxFrameBottomRectangle { get; }
        private Rectangle BoxFrameLeftRectangle { get; }
        private Rectangle BoxFrameRightRectangle { get; }

        private Rectangle GradientDownRectangle { get; }
        private Rectangle GradientRightRectangle { get; }

        private Texture2D BoxFrameTexture { get; }
        private Texture2D BoxTexture { get; }
        private Texture2D GradientDownTexture { get; }
        private Texture2D GradientRightTexture { get; }

        #endregion Frame & Gradient

        protected GUIBox(Client game, Screen screen, Rectangle boxRectangle, string buttonText, Color style) : base(game, screen, false)
        {
            BoxRectangle = boxRectangle;
            UsingColor = style;

            BoxGrid = new Grid(boxRectangle, MinResolutionScale);


            var buttonRectangle = new Rectangle(
                BoxRectangle.Center.X - ButtonHalfSize.Center.X,
                BoxRectangle.Y + BoxRectangle.Height - ButtonHalfSize.Height - BoxGrid.OffsetY,
                ButtonHalfSize.Width, ButtonHalfSize.Height);

            if (!string.IsNullOrEmpty(buttonText))
            {
                var Button = new ButtonMenuHalf(Game, Screen, buttonText, buttonRectangle, null, UsingColor);
                Button.OnButtonPressed += OnButtonPressed;
                GUIItems.Add(Button);
            }


            BoxTexture = new Texture2D(GraphicsDevice, 1, 1);
            BoxTexture.SetData(new[] { new Color(100, 100, 100, 240) });

            BoxFrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            BoxFrameTexture.SetData(new[] { new Color(0, 0, 0, 240) });

            var scale = (int) MinResolutionScale;
            GradientDownTexture = CreateGradientDown(BoxRectangle.Width + 5 - scale, 5);
            GradientRightTexture = CreateGradientRight(5, BoxRectangle.Height + 5 - scale);
            GradientDownRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y + BoxRectangle.Height - scale, GradientDownTexture.Width, GradientDownTexture.Height);
            GradientRightRectangle = new Rectangle(BoxRectangle.X + BoxRectangle.Width - scale, BoxRectangle.Y, GradientRightTexture.Width, GradientRightTexture.Height);


            BoxFrameTopRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y, BoxRectangle.Width, BoxFrameSize.Y);
            BoxFrameBottomRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y + BoxRectangle.Height - BoxFrameSize.Y, BoxRectangle.Width, BoxFrameSize.Y);
            BoxFrameLeftRectangle = new Rectangle(BoxRectangle.X, BoxRectangle.Y, BoxFrameSize.X, BoxRectangle.Height);
            BoxFrameRightRectangle = new Rectangle(BoxRectangle.X + BoxRectangle.Width - BoxFrameSize.X, BoxRectangle.Y, BoxFrameSize.X, BoxRectangle.Height);
    }
        protected abstract void OnButtonPressed(object sender, EventArgs eventArgs);

        protected void AddGUIItem(GUIItem item) { GUIItems.Insert(GUIItems.Count - 1, item); } // Button should be last.
        protected void AddGUIItems(params GUIItem[] items) { GUIItems.InsertRange(GUIItems.Count - 1, items); } // Button should be last.

        public GUIItem[] GetGUIItems()
        {
            var list = new List<GUIItem> { this };
            list.AddRange(GUIItems);
            return list.ToArray();
        }

        public override void Update(GameTime gameTime) { }
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

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();

            GUIItems?.Clear();

            BoxFrameTexture?.Dispose();
            BoxTexture?.Dispose();

            GradientDownTexture?.Dispose();
            GradientRightTexture?.Dispose();
        }


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
