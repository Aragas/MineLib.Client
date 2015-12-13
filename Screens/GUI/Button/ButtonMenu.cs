using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.GUI.Button
{
    public sealed class ButtonMenu : GUIButton
    {
        public ButtonMenu(Client game, Screen screen, string text, Rectangle pos, EventHandler action, Color textureColor) : base(game, screen, text, action, textureColor)
        {
            ButtonRectangle = pos;
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
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangle, ButtonUnavailablePosition, TextureColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangle, ButtonUnavailableColor);
            }

            if (IsSelected || IsSelectedMouseHover)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangle, ButtonPressedPosition, TextureColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangleShadow, ButtonPressedShadowColor);
                MainTextRenderer.DrawTextCentered(SpriteBatch, ButtonText, ButtonRectangle, ButtonPressedColor);
            }

            if (IsActive)
            {
                SpriteBatch.Draw(WidgetsTexture, ButtonRectangle, ButtonPosition, TextureColor);
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
