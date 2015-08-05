using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PCL.Graphics.Screens.GUI.InputBox;

namespace MineLib.PCL.Graphics.Screens.GUI.Text
{
    public sealed class InputBoxText : GUIText
    {
        private Color TextShadowColor { get; set; }

        private GUIInputBox InputBox { get; set; } // Readonly

        private Rectangle TextShadowRectangle { get; set; }

        private const int CycleNumb = 40;
        private int CycleCount = CycleNumb;


        public InputBoxText(Client game, Screen screen, Rectangle textRect, Color textColor, Color shadowColor, GUIInputBox inputBox) : base(game, screen, null, textRect, textColor)
        {
            TextColor = textColor;
            TextShadowColor = shadowColor;

            TextShadowRectangle = new Rectangle(TextRectangle.X + 1, TextRectangle.Y + 1, TextRectangle.Width, TextRectangle.Height);

            InputBox = inputBox;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputBox.IsSelected)
            {
                if (CycleCount > CycleNumb)
                {
                    InputBox.ShowInput = !InputBox.ShowInput;
                    CycleCount = 0;
                }

                CycleCount++;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            if (InputBox.IsSelected && InputBox.ShowInput)
            {
                MainTextRenderer.DrawText(SpriteBatch, InputBox.Text + "_", TextShadowRectangle, TextShadowColor);
                MainTextRenderer.DrawText(SpriteBatch, InputBox.Text + "_", TextRectangle, TextColor);
            }
            else
            {
                MainTextRenderer.DrawText(SpriteBatch, InputBox.Text, TextShadowRectangle, TextShadowColor);
                MainTextRenderer.DrawText(SpriteBatch, InputBox.Text, TextRectangle, TextColor);
            }

            SpriteBatch.End();
        }
    }
}
