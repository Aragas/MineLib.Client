using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.PGL.Screens.GUI.Button;
using MineLib.PGL.Screens.InMenu;

namespace MineLib.PGL.Screens.InGame
{
    public sealed class GameOptionScreen : Screen
    {
        GameScreen GameScreen { get; }
        Texture2D MainBackgroundTexture { get; }

        public GameOptionScreen(Client game, GameScreen gameScreen) : base(game)
        {
            GameScreen = gameScreen;

            MainBackgroundTexture = new Texture2D(GraphicsDevice, 1, 1);
            MainBackgroundTexture.SetData(new[] { new Color(0, 0, 0, 170) });
        }
        private void OnReturnToGameButtonPressed(object sender, EventArgs eventArgs)
        {
            Game.IsMouseVisible = false;

            ToHidden();
            GameScreen.ToActive();
        }
        private void OnOptionButtonPressed(object sender, EventArgs eventArgs)
        {
            AddScreenAndHideThis(new OptionsScreen(Game, this));
        }

        public override void OnResize()
        {
            base.OnResize();

            var xOffset = 0;
            var yOffset = 0;

            var returnToGameRectangle = new Rectangle(
                xOffset = ScreenRectangle.Center.X - ButtonSize.Center.X,
                yOffset += ScreenRectangle.Center.Y - (ButtonSize.Center.Y + 15) * 3,
                ButtonSize.Width, ButtonSize.Height);
            var ReturnToGameButton = new ButtonMenu(Game, this, "Return to Game", returnToGameRectangle, OnReturnToGameButtonPressed, SecondaryBackgroundColor);

            var optionsRectangle = new Rectangle(
                xOffset = ScreenRectangle.Center.X - ButtonSize.Center.X,
                yOffset += -ButtonSize.Center.Y - (ButtonSize.Center.Y + 15) * 2,
                ButtonSize.Width, ButtonSize.Height);
            var OptionsButton = new ButtonMenu(Game, this, "Options", optionsRectangle, OnOptionButtonPressed, SecondaryBackgroundColor);

            AddGUIItem(ReturnToGameButton);
            AddGUIItem(OptionsButton);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.Escape))
                if (IsActive)
                {
                    Game.IsMouseVisible = false;

                    ToHidden();
                    GameScreen.ToActive();
                }
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            SpriteBatch.Draw(MainBackgroundTexture, ScreenRectangle, Color.White);
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
