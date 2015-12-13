using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.BMFont;
using MineLib.PGL.Components;

namespace MineLib.PGL.Screens.GUI
{
    public enum GUIItemState
    {
        Active,
        JustNowActive,
        Selected,
        SelectedMouseHover,
        NonPressable,
        Hidden
    }

    public abstract class GUIItem : ScreenGUIComponent
    {
        protected static FontRenderer MainTextRenderer { get; private set; }

        protected SpriteBatch SpriteBatch => _spriteBatch ?? (_spriteBatch = new SpriteBatch(Game.GraphicsDevice));
        private SpriteBatch _spriteBatch;
        protected TextureStorageComponent TextureStorage => Game.TextureStorage;

        public bool CanBeSelected { get; }

        #region ItemState

        private GUIItemState ItemState { get; set; }

        public bool IsActive => ItemState == GUIItemState.Active;
        public bool IsJustNowActive => ItemState == GUIItemState.JustNowActive;
        public bool IsSelected => ItemState == GUIItemState.Selected;
        public bool IsSelectedMouseHover => ItemState == GUIItemState.SelectedMouseHover;
        public bool IsNonPressable => ItemState == GUIItemState.NonPressable;
        public bool IsHidden => ItemState == GUIItemState.Hidden;

        public void ToActive() { ItemState = GUIItemState.Active; }
        public void ToJustNowActive() { ItemState = GUIItemState.JustNowActive; }
        public void ToSelected() { ItemState = GUIItemState.Selected; }
        public void ToSelectedMouseHover() { ItemState = GUIItemState.SelectedMouseHover; }
        public void ToNonPressable() { ItemState = GUIItemState.NonPressable; }
        public void ToHidden() { ItemState = GUIItemState.Hidden; }

        #endregion ItemState

        #region Resolution

        protected float MinResolutionScale => Screen.MinResolutionScale;

        protected int FontSmallSize => Screen.FontSmallSize;
        protected int FontNormalSize => Screen.FontNormalSize;
        protected int FontBigSize => Screen.FontBigSize;

        protected GUIItemSize BoxSize => Screen.BoxSize;
        protected GUIItemSize ButtonSize => Screen.ButtonSize;
        protected GUIItemSize ButtonHalfSize => Screen.ButtonHalfSize;

        #endregion Resolution

        protected GUIItem(Client game, Screen screen, bool canBeSelected ) : base(game, screen)
        {
            if (MainTextRenderer == null)
                MainTextRenderer = new FontRenderer(GraphicsDevice, "PixelUnicode", 16, 32, 48);

            CanBeSelected = canBeSelected;
        }

        public override void Dispose()
        {
            _spriteBatch?.Dispose();
        }
    }
}
