using System;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.PGL.Extensions;
using MineLib.PGL.Screens.GUI.Box;
using MineLib.PGL.Screens.GUI.Button;
using MineLib.PGL.Screens.InGame;
using MineLib.PGL.Screens.InMenu.ServerList;

namespace MineLib.PGL.Screens.InMenu
{
    public sealed class MainMenuScreen : Screen
    {
        Texture2D MainBackgroundTexture { get; }
        //Texture2D _xboxController;

        public MainMenuScreen(Client game) : base(game)
        {
            Game.IsMouseVisible = true;

            MainBackgroundTexture = TextureStorage.GUITextures.Panorama0.Copy();
        }
        private void OnDirectConnectButton(object sender, ConnectionEventArgs args) { AddScreenAndCloseThis(new GameScreen(Game, args.Entry)); }
        private void OnLastServerConnectButton(object sender, ConnectionEventArgs args) { AddScreenAndCloseThis(new GameScreen(Game, args.Entry)); }
        private void OnMultiplayerButtonPressed(object sender, EventArgs args) { AddScreenAndHideThis(new ServerListScreen(Game, this)); }
        private void OnOptionButtonPressed(object sender, EventArgs args) { AddScreenAndHideThis(new OptionsScreen(Game, this));}
        private void OnLanguageButtonPressed(object sender, EventArgs args) { AddScreenAndHideThis(new LanguageScreen(Game, this)); }
        private void OnExitButtonPressed(object sender, EventArgs args) { Task.Delay(200).Wait(); Exit(); }

        public override void OnResize()
        {
            base.OnResize();

            int boxCount = 3;
            int buttonCount = 3;
            int boxInterval = (ScreenRectangle.Width - BoxSize.Width * boxCount) / (boxCount + 1);
            int buttonInterval = (ScreenRectangle.Center.Y - BoxSize.Center.Y - ButtonSize.Height * boxCount) / (buttonCount + 1);

            int xOffset = 0;
            int yOffset = 0;

            #region Boxes
            // Left
            var lastServerBoxRectangle = new Rectangle(
                xOffset += ScreenRectangle.X + boxInterval,
                yOffset = ScreenRectangle.Center.Y - BoxSize.Center.Y,
                BoxSize.Width, BoxSize.Height);
            var server = new LastServer { Image = null, Name = "Shitty Server", LastPlayed = "Never" };
            var LastServerBox = new BoxLastServer(Game, this, lastServerBoxRectangle, OnLastServerConnectButton, server, SecondaryBackgroundColor);

            // Center
            var connectBoxRectangle = new Rectangle(
                xOffset += BoxSize.Width + boxInterval,
                yOffset = ScreenRectangle.Center.Y - BoxSize.Center.Y,
                BoxSize.Width, BoxSize.Height);
            var ConnectBox = new BoxDirectConnect(Game, this, connectBoxRectangle, OnDirectConnectButton, SecondaryBackgroundColor);

            // Right
            var multiplayerBoxRectangle = new Rectangle(
                xOffset += BoxSize.Width + boxInterval,
                yOffset = ScreenRectangle.Center.Y - BoxSize.Center.Y,
                BoxSize.Width, BoxSize.Height);
            var MultiplayerBox = new BoxMultiplayer(Game, this, multiplayerBoxRectangle, OnMultiplayerButtonPressed, SecondaryBackgroundColor);

            AddGUIItems(LastServerBox.GetGUIItems());
            AddGUIItems(ConnectBox.GetGUIItems());
            AddGUIItems(MultiplayerBox.GetGUIItems());
            #endregion Boxes

            #region Buttons
            // First
            var languageButtonRectangle = new Rectangle(
                xOffset = ScreenRectangle.Center.X - ButtonSize.Center.X,
                yOffset += BoxSize.Height + buttonInterval,
                ButtonSize.Width, ButtonSize.Height);
            var LanguageButton = new ButtonMenu(Game, this, "Language", languageButtonRectangle, OnLanguageButtonPressed, SecondaryBackgroundColor);

            // Second
            var optionsButtonRectangle = new Rectangle(
                xOffset = ScreenRectangle.Center.X - ButtonSize.Center.X,
                yOffset += ButtonSize.Height + buttonInterval,
                ButtonSize.Width, ButtonSize.Height);
            var OptionsButton = new ButtonMenu(Game, this, "Options", optionsButtonRectangle, OnOptionButtonPressed, SecondaryBackgroundColor);

            // Third
            var exitButtonRectangle = new Rectangle(
                xOffset = ScreenRectangle.Center.X - ButtonSize.Center.X,
                yOffset += ButtonSize.Height + buttonInterval,
                ButtonSize.Width, ButtonSize.Height);
            var ExitButton = new ButtonMenu(Game, this, "Exit", exitButtonRectangle, OnExitButtonPressed, SecondaryBackgroundColor);

            AddGUIItem(LanguageButton);
            AddGUIItem(OptionsButton);
            AddGUIItem(ExitButton);
            #endregion Buttons
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.Escape))
                Exit();
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
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
