using System;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Data;

using MineLib.PCL.Graphics.Components;
using MineLib.PCL.Graphics.Screens.GUI.Box;
using MineLib.PCL.Graphics.Screens.GUI.Button;
using MineLib.PCL.Graphics.Screens.InGame;
using MineLib.PCL.Graphics.Screens.InMenu.ServerList;

namespace MineLib.PCL.Graphics.Screens.InMenu
{
    public sealed class MainMenuScreen<T> : Screen where T : struct, IVertexType
    {
        Texture2D MainMenuTexture { get; set; }
        //Texture2D _xboxController;

        //Vector2 DefaultBoxSize { get { return new Vector2(270, 240); } }
        Vector2 DefaultBoxSize { get { return new Vector2(310, 275); } }

        Vector2 BoxSize { get { return new Vector2(DefaultBoxSize.X * Math.Min(ResolutionScaleX, ResolutionScaleY), DefaultBoxSize.Y * Math.Min(ResolutionScaleX, ResolutionScaleY)); } }

        Color Style { get { return new Color(70, 70, 70); } }

        GUIBox LastServerBox { get; set; }
        GUIBox ConnectBox { get; set; }
        GUIBox MultiplayerBox { get; set; }


        public MainMenuScreen(Client game) : base(game, "MainMenuScreen")
        {
            Game.IsMouseVisible = true;

            MainMenuTexture = MinecraftTextureStorage.GUITextures.Panorama0;


            var connectBoxRectangle = new Rectangle(
                ScreenRectangle.Center.X - (int)(BoxSize.X * 0.5f),
                ScreenRectangle.Center.Y - (int)(BoxSize.Y * 0.60f),
                (int)BoxSize.X,
                (int)BoxSize.Y);
            ConnectBox = new BoxDirectConnect(Game, this, connectBoxRectangle, OnDirectConnectButton, Style);
            var scale = ConnectBox.ButtonScale;

            var lastServerBoxRectangle = new Rectangle(
                connectBoxRectangle.X - (int)(BoxSize.X) - (int)(15 * (scale + 1 * scale)),
                ScreenRectangle.Center.Y - (int)(BoxSize.Y * 0.60f),
                (int)BoxSize.X,
                (int)BoxSize.Y);
            var server = new LastServer { Image = null, Name = "Shitty Server", LastPlayed = "Never" };
            LastServerBox = new BoxLastServer(Game, this, lastServerBoxRectangle, OnLastServerConnectButton, server, Style);

            var multiplayerBoxRectangle = new Rectangle(
                connectBoxRectangle.X + (int)(BoxSize.X) + (int)(15 * (scale + 1 * scale)),
                ScreenRectangle.Center.Y - (int)(BoxSize.Y * 0.60f),
                (int)BoxSize.X,
                (int)BoxSize.Y);
            MultiplayerBox = new BoxMultiplayer(Game, this, multiplayerBoxRectangle, OnMultiplayerButtonPressed, Style);

            LastServerBox.AddToGUIItemMultiController(GUIItemMultiController);
            ConnectBox.AddToGUIItemMultiController(GUIItemMultiController);
            MultiplayerBox.AddToGUIItemMultiController(GUIItemMultiController);


            var languageButtonRectangle = new Rectangle(
                ScreenRectangle.Center.X - (int)(ButtonMenu.VanillaSize.X * scale * 0.5f),
                connectBoxRectangle.Y + connectBoxRectangle.Height + 10,
                (int)(ButtonMenu.VanillaSize.X * scale),
                (int)(ButtonMenu.VanillaSize.Y * scale));
            AddButtonMenu("Language", languageButtonRectangle, OnLanguageButtonPressed, Style);

            var optionsButtonRectangle = new Rectangle(
                ScreenRectangle.Center.X - (int)(ButtonMenu.VanillaSize.X * scale * 0.5f),
                languageButtonRectangle.Y + languageButtonRectangle.Height + 10,
                (int)(ButtonMenu.VanillaSize.X * scale),
                (int)(ButtonMenu.VanillaSize.Y * scale));
            AddButtonMenu("Options", optionsButtonRectangle, OnOptionButtonPressed, Style);

            var exitButtonRectangle = new Rectangle(
                ScreenRectangle.Center.X - (int)(ButtonMenu.VanillaSize.X * scale * 0.5f),
                optionsButtonRectangle.Y + optionsButtonRectangle.Height + 10,
                (int)(ButtonMenu.VanillaSize.X * scale),
                (int)(ButtonMenu.VanillaSize.Y * scale));
            AddButtonMenu("Exit", exitButtonRectangle, OnExitButtonPressed, Style);

        }


        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.Escape))
                Exit();

            LastServerBox.Update(gameTime);
            ConnectBox.Update(gameTime);
            MultiplayerBox.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            SpriteBatch.Draw(MainMenuTexture, ScreenRectangle, Color.White);
            SpriteBatch.End();

            LastServerBox.Draw(gameTime);
            ConnectBox.Draw(gameTime);
            MultiplayerBox.Draw(gameTime);

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (LastServerBox != null)
                LastServerBox.Dispose();

            if (ConnectBox != null)
                ConnectBox.Dispose();

            if (MultiplayerBox != null)
                MultiplayerBox.Dispose();
        }


        private void OnDirectConnectButton(Server entry)
        {
            AddScreenAndCloseThis(new GameScreen<T>(Game, entry));
        }

        private void OnLastServerConnectButton(Server entry)
        {
            AddScreenAndCloseThis(new GameScreen<T>(Game, entry));
        }

        private void OnMultiplayerButtonPressed()
        {
            AddScreenAndCloseThis(new ServerListScreen<T>(Game));
        }

        private void OnOptionButtonPressed()
        {
            AddScreenAndCloseThis(new OptionsScreen<T>(Game));
        }

        private void OnLanguageButtonPressed()
        {
            AddScreenAndCloseThis(new LanguageScreen<T>(Game));
        }

        private void OnExitButtonPressed()
        {
            Task.Delay(200).Wait();
            Exit();
        }
    }
}
