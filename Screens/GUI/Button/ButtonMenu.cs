using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.GUI.Button
{
    public sealed class ButtonMenu : GUIButton
    {
        public static Vector2 VanillaSize = new Vector2(400, 40); // Vanilla settings
        public static readonly float AspectRatio = VanillaSize.X / VanillaSize.Y;


        public ButtonMenu(Client game, Screen screen, string text, Rectangle pos, Action action, Color textureColor) : base(game, screen, text, action, textureColor)
        {
            ButtonRectangle = pos;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            if (IsNonPressable)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangle, ButtonUnavailablePosition, TextureColor);
                MainTextRenderer.DrawText(SpriteBatch, ButtonText, ButtonRectangle, ButtonUnavailableColor);
            }

            if (IsSelected || IsSelectedMouseHover)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangle, ButtonPressedPosition, TextureColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonPressedShadowColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangle, ButtonPressedColor);
            }

            if (IsActive)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangle, ButtonPosition, TextureColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonShadowColor);
                MainTextRenderer.DrawTextCenteredStretched(SpriteBatch, ButtonText, ButtonRectangle, ButtonColor);
            }

            SpriteBatch.End();
        }
    }
}
