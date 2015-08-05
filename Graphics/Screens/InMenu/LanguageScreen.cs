using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.PCL.Graphics.Components;

namespace MineLib.PCL.Graphics.Screens.InMenu
{
    public sealed class LanguageScreen<T> : Screen where T : struct, IVertexType
    {
        Texture2D MainBackgroundTexture { get; set; }

        public LanguageScreen(Client game) : base(game, "LanguageScreen")
        {
            Game.IsMouseVisible = true;

            MainBackgroundTexture = MinecraftTextureStorage.GUITextures.OptionsBackground;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.Back) || InputManager.IsOncePressed(Buttons.B))
                AddScreenAndCloseThis(new MainMenuScreen<T>(Game));
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
