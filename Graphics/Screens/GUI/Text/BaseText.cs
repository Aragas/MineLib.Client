using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PCL.Graphics.Screens.GUI.Text
{
    public sealed class BaseText : GUIText
    {
        public BaseText(Client game, Screen screen, string text, Rectangle textRect, Color textColor) : base(game, screen, text, textRect, textColor)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            MainTextRenderer.DrawText(SpriteBatch, Text, TextRectangle, TextColor);
            SpriteBatch.End();
        }
    }
}
