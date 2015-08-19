using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;
using MineLib.PGL.Screens.GUI;
using MineLib.PGL.Screens.GUI.Button;
using MineLib.PGL.Screens.GUI.GamePad;

namespace MineLib.PGL.Screens
{
    public enum ScreenState
    {
        Active          = 0,
        Background      = 1,
        Hidden          = 2,
        JustNowActive   = 3
    }

    public abstract class Screen : MineLibComponent
    {
        public static PadDaisywheel Daisywheel { get; private set; }


        public string Name { get; private set; }
        public ScreenState ScreenState { get; set; }

        protected SpriteBatch SpriteBatch { get { return _spriteBatch ?? (_spriteBatch = new SpriteBatch(Game.GraphicsDevice)); } }
        private SpriteBatch _spriteBatch;

        protected internal TextureStorageComponent TextureStorage { get { return Game.TextureStorage; } }

        protected GUIItemMultiController GUIItemMultiController { get; private set; }


        public static Color MainBackgroundColor { get { return new Color(30, 30, 30, 255); } }
        public static Color SecondaryBackgroundColor { get { return new Color(75, 75, 75, 255); } }


        #region Resolution

        protected Vector2 DefaultResolution { get { return new Vector2(800, 600); } }
        protected Vector2 CurrentResolution { get { return new Vector2(ScreenRectangle.Width, ScreenRectangle.Height); } }
        protected float ResolutionScaleX { get { return CurrentResolution.X / DefaultResolution.X; } }
        protected float ResolutionScaleY { get { return CurrentResolution.Y / DefaultResolution.Y; } }

        protected Rectangle ScreenRectangle { get { return Game.GraphicsDevice.Viewport.Bounds; } }
        
        #endregion Resolution
       
        
        private ScreenManagerComponent ScreenManager { get { return Game.ScreenManager; } }
        
        private List<GUIButton> Buttons { get; set; }


        protected Screen(Client game, string name) : base(game)
        {
            if(Daisywheel == null)
                Daisywheel = new PadDaisywheel(Game);

            Name = name;
            GUIItemMultiController = new GUIItemMultiController(Game, this);
            Buttons = new List<GUIButton>();
        }


        protected void AddScreen(Screen screen) { ScreenManager.AddScreen(screen); }
        protected void AddScreenAndHideThis(Screen screen) { ScreenManager.AddScreen(screen); ToHidden(); }
        protected void AddScreenAndCloseThis(Screen screen) { ScreenManager.AddScreen(screen); CloseScreen(); }
        protected void AddScreenAndCloseOthers(Screen screen) { GUIItemMultiController.Dispose(); ScreenManager.CloseOtherScreens(screen); }

        protected void AddButtonMenu(string text, Rectangle pos, Action action, Color style)
        {
            var button = new ButtonMenu(Game, this, text, pos, action, style);
            Buttons.Add(button);
            GUIItemMultiController.AddGUIItem(button);
        }
        protected void AddButtonMenuHalf(string text, Rectangle pos, Action action, Color style)
        {
            var button = new ButtonMenuHalf(Game, this, text, pos, action, style);
            Buttons.Add(button);
            GUIItemMultiController.AddGUIItem(button);
        }


        public override void Draw(GameTime gameTime)
        {
            foreach (var guiButton in Buttons)
                guiButton.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            GUIItemMultiController.Update(gameTime);
        }

        public override void Dispose()
        {
            if (_spriteBatch != null)
                _spriteBatch.Dispose();

            if (GUIItemMultiController != null)
                GUIItemMultiController.Dispose();

            if(Buttons != null)
                Buttons.Clear();
        }


        protected void ToActive() { ScreenState = ScreenState.JustNowActive; }
        protected void ToBackground() { ScreenState = ScreenState.Background; }
        protected void ToHidden() { ScreenState = ScreenState.Hidden; }

        protected void Exit() { Game.Exit(); }

        protected void CloseScreen()
        {
            // If the screen has a zero transition time, remove it immediately.
            ScreenManager.RemoveScreen(this);
        }
    }
}
