using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MineLib.PGL.Screens.InMenu
{
    public sealed class LanguageScreen : Screen
    {
        Texture2D MainBackgroundTexture { get; set; }

        public LanguageScreen(Client game) : base(game, "LanguageScreen")
        {
            Game.IsMouseVisible = true;

            MainBackgroundTexture = TextureStorage.GUITextures.OptionsBackground;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.Back) || InputManager.IsOncePressed(Buttons.B))
                AddScreenAndCloseThis(new MainMenuScreen(Game));
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            SpriteBatch.Draw(MainBackgroundTexture, Vector2.Zero, ScreenRectangle, SecondaryBackgroundColor, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 1f);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            //if (MainBackgroundTexture != null)
            //    MainBackgroundTexture.Dispose();
        }
    }
}
