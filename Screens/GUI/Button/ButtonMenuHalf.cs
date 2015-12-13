using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.GUI.Button
{
    // We need to split texture render, resize didn't properly work.
    public sealed class ButtonMenuHalf : GUIButton
    {
        public ButtonMenuHalf(Client game, Screen screen, string text, Rectangle pos, EventHandler action, Color textureColor) : base(game, screen, text, action, textureColor)
        {
            ButtonRectangle = pos;
            
            ButtonRectangleFirstHalf = ButtonRectangle;
            ButtonRectangleFirstHalf.Width -= (int)(ButtonRectangleFirstHalf.Width * 0.5f);

            ButtonRectangleSecondHalf = ButtonRectangle;
            ButtonRectangleSecondHalf.X += (int)(ButtonRectangleSecondHalf.Width * 0.5f);
            ButtonRectangleSecondHalf.Width -= (int)(ButtonRectangleSecondHalf.Width * 0.5f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            if (IsNonPressable)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleFirstHalf, ButtonUnavailableFirstHalfPosition, TextureColor);
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleSecondHalf, ButtonUnavailableSecondHalfPosition, TextureColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangle, ButtonUnavailableColor);
            }

            if (IsSelected || IsSelectedMouseHover)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleFirstHalf, ButtonPressedFirstHalfPosition, TextureColor);
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleSecondHalf, ButtonPressedSecondHalfPosition, TextureColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonPressedShadowColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangle, ButtonPressedColor);
            }

            if (IsActive)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleFirstHalf, ButtonFirstHalfPosition, TextureColor);
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleSecondHalf, ButtonSecondHalfPosition, TextureColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonShadowColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangle, ButtonColor);
            }

            SpriteBatch.End();
        }

        public override void Dispose()
        {
            base.Dispose();

        }
    }
}
