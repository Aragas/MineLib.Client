using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;
using MineLib.PGL.Screens.GUI;

namespace MineLib.PGL.Screens
{
    public struct GUIItemSize
    {
        public int Width { get; }
        public int Height { get; }

        public Point Center { get; }

        public GUIItemSize(int value)
        {
            Width = value;
            Height = value;
            Center = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
        }
        public GUIItemSize(int width, int height)
        {
            Width = width;
            Height = height;
            Center = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
        }

        public GUIItemSize Scale(float value)
        {
            return new GUIItemSize((int)(Width * value), (int)(Height * value));
        }

        public GUIItemSize ScaleN(float value)
        {
            return new GUIItemSize((int)(Width * value + (value - 1) * 35), (int)(Height * value));
        }
    }

    public enum ScreenState
    {
        Active = 0,
        Background = 1,
        Hidden = 2,
        JustNowActive = 3
    }
    public abstract class Screen : MineLibComponent
    {
        protected SpriteBatch SpriteBatch => _spriteBatch ?? (_spriteBatch = new SpriteBatch(Game.GraphicsDevice));
        private SpriteBatch _spriteBatch;
        protected TextureStorageComponent TextureStorage => Game.TextureStorage;

        private GUIItemMultiController GUIItemMultiController { get; } = new GUIItemMultiController();

        #region ScreenState

        private ScreenState ScreenState { get; set; }

        public bool IsActive => ScreenState == ScreenState.Active;
        public bool IsBackground => ScreenState == ScreenState.Background;
        public bool IsHidden => ScreenState == ScreenState.Hidden;
        protected internal bool IsJustNowActive => ScreenState == ScreenState.JustNowActive;

        public void ToActive() { ScreenState = IsJustNowActive ? ScreenState.Active : ScreenState.JustNowActive; }
        public void ToBackground() { ScreenState = ScreenState.Background; }
        public void ToHidden() { ScreenState = ScreenState.Hidden; }

        #endregion ScreenState

        #region ScreenManager

        private ScreenManagerComponent ScreenManager => Game.ScreenManager;

        protected void AddScreen(Screen screen) { ScreenManager.AddScreen(screen); }
        protected void AddScreenAndHideThis(Screen screen) { ScreenManager.AddScreen(screen); ToHidden(); }
        protected void AddScreenAndCloseThis(Screen screen) { CloseScreen(); ScreenManager.AddScreen(screen); }
        protected void AddScreenAndCloseOthers(Screen screen) { ScreenManager.AddScreen(screen); ScreenManager.CloseOtherScreens(screen); }

        #endregion ScreenManager

        #region Resolution

        protected Rectangle ScreenRectangle => Game.GraphicsDevice.Viewport.Bounds;

        private Vector2 DefaultResolution { get; } = Client.DefaultResolution.ToVector2();
        private Vector2 CurrentResolution { get; set; }
        private float ResolutionScaleX { get; set; }
        private float ResolutionScaleY { get; set; }
        public float MinResolutionScale { get; private set; }


        private int DefaultFontSmallSize { get; } = 16;
        private int DefaultFontNormalSize { get; } = 32;
        private int DefaultFontBigSize { get; } = 48;

        public int FontSmallSize { get; private set; }
        public int FontNormalSize { get; private set; }
        public int FontBigSize { get; private set; }

        private GUIItemSize DefaultBoxSize { get; } = new GUIItemSize(260, 250);
        private GUIItemSize DefaultButtonSize { get; } = new GUIItemSize(400, 40);
        private GUIItemSize DefaultButtonHalfSize { get; } = new GUIItemSize(200, 40);
        public GUIItemSize BoxSize { get; private set; }
        public GUIItemSize ButtonSize { get; private set; }
        public GUIItemSize ButtonHalfSize { get; private set; }

        #endregion Resolution

        protected static Color MainBackgroundColor { get; } = new Color(30, 30, 30, 255);
        protected static Color SecondaryBackgroundColor { get; } = new Color(75, 75, 75, 255);

        private bool _disposed;

        protected Screen(Client game) : base(game) { OnResize(); }

        protected void AddGUIItem(GUIItem item) { GUIItemMultiController.AddGUIItem(item); }
        protected void AddGUIItems(params GUIItem[] items) { GUIItemMultiController.AddGUIItems(items); }

        public virtual void OnResize()
        {
            GUIItemMultiController.Clear();

            CurrentResolution = new Vector2(ScreenRectangle.Width, ScreenRectangle.Height);

            ResolutionScaleX = CurrentResolution.X / DefaultResolution.X;
            ResolutionScaleY = CurrentResolution.Y / DefaultResolution.Y;
            MinResolutionScale = Math.Min(ResolutionScaleX, ResolutionScaleY);


            BoxSize = DefaultBoxSize.ScaleN(MinResolutionScale);
            ButtonSize = DefaultButtonSize.Scale(MinResolutionScale);
            ButtonHalfSize = DefaultButtonHalfSize.Scale(MinResolutionScale);


            FontSmallSize = (int) (DefaultFontSmallSize * MinResolutionScale);
            FontNormalSize = (int) (DefaultFontNormalSize * MinResolutionScale);
            FontBigSize = (int) (DefaultFontBigSize * MinResolutionScale);
        }

        public override void Update(GameTime gameTime) { if(_disposed) return; GUIItemMultiController.Update(gameTime); }
        public override void Draw(GameTime gameTime)
        {
#if DEBUG
            DebugComponent.GUIItemsDrawCalls = 0;
#endif
            GUIItemMultiController.Draw(gameTime);
        }

        protected void CloseScreen() { ScreenManager.RemoveScreen(this); }
        protected void Exit() { Game.Exit(); }

        public override void Dispose() { _disposed = true; _spriteBatch?.Dispose(); GUIItemMultiController?.Clear(); }
    }
}
