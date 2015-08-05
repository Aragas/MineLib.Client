using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PCL.Graphics.Screens.GUI.Button
{
    // We need to split texture render, resize didn't properly work.
    public sealed class ButtonMenuHalf : GUIButton
    {
        public static Vector2 VanillaSize = new Vector2(196, 40); // Vanilla settings
        public static readonly float AspectRatio = VanillaSize.X / VanillaSize.Y;


        public ButtonMenuHalf(Client game, Screen screen, string text, Rectangle pos, Action action, Color textureColor) : base(game, screen, text, action, textureColor)
        {
            ButtonRectangle = pos;
            
            ButtonRectangleFirstHalf = ButtonRectangle;
            ButtonRectangleFirstHalf.Width -= (int)(ButtonRectangleFirstHalf.Width * 0.5f);

            ButtonRectangleSecondHalf = ButtonRectangle;
            ButtonRectangleSecondHalf.X += (int)(ButtonRectangleSecondHalf.Width * 0.5f);
            ButtonRectangleSecondHalf.Width -= (int)(ButtonRectangleSecondHalf.Width * 0.5f);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            if (IsNonPressable)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleFirstHalf, ButtonUnavailableFirstHalfPosition, TextureColor);
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleSecondHalf, ButtonUnavailableSecondHalfPosition, TextureColor);
                MainTextRenderer.DrawText(SpriteBatch, ButtonText, ButtonRectangle, ButtonUnavailableColor);
            }

            if (IsSelected || IsSelectedMouseHover)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleFirstHalf, ButtonPressedFirstHalfPosition, TextureColor);
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleSecondHalf, ButtonPressedSecondHalfPosition, TextureColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonPressedShadowColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangle, ButtonPressedColor);
            }

            if (IsActive)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleFirstHalf, ButtonFirstHalfPosition, TextureColor);
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangleSecondHalf, ButtonSecondHalfPosition, TextureColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonShadowColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangle, ButtonColor);
            }

            SpriteBatch.End();
        }
    }
}
