using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.PGL.Extensions;

namespace MineLib.PGL.Screens.InMenu
{
    public sealed class OptionsScreen : Screen
    {
        Screen BackScreen { get; }
        Texture2D MainBackgroundTexture { get; }
        
        public OptionsScreen(Client game, Screen backScreen) : base(game)
        {
            BackScreen = backScreen;

            Game.IsMouseVisible = true;

            MainBackgroundTexture = TextureStorage.GUITextures.OptionsBackground.Copy();
        }

        public override void OnResize()
        {
            base.OnResize();

        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.Escape) || InputManager.IsOncePressed(Buttons.B))
            {
                BackScreen.ToActive();
                CloseScreen();
            }
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            SpriteBatch.Draw(MainBackgroundTexture, Vector2.Zero, ScreenRectangle, SecondaryBackgroundColor, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.5f);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();

            MainBackgroundTexture?.Dispose();
        }
    }
}
