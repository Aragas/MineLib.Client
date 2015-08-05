using Microsoft.Xna.Framework.Graphics;

using MineLib.PCL.Graphics.BMFont;
using MineLib.PCL.Graphics.Components;

namespace MineLib.PCL.Graphics.Screens.GUI
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


    public abstract class GUIItem : MineLibGUIGameComponent
    {
        protected static FontRenderer MainTextRenderer { get; set; }

        protected MinecraftTextureStorageComponent MinecraftTextureStorage { get { return Game.MinecraftTextureStorage; } }
        
        protected SpriteBatch SpriteBatch { get { return _spriteBatch ?? (_spriteBatch = new SpriteBatch(Game.GraphicsDevice)); } }
        private SpriteBatch _spriteBatch;


        public bool IsActive { get { return ItemState == GUIItemState.Active; } }
        public bool IsJustNowActive { get { return ItemState == GUIItemState.JustNowActive; } }
        public bool IsSelected { get { return ItemState == GUIItemState.Selected; } }
        public bool IsSelectedMouseHover { get { return ItemState == GUIItemState.SelectedMouseHover; } }
        public bool IsNonPressable { get { return ItemState == GUIItemState.NonPressable; } }
        public bool IsHidden { get { return ItemState == GUIItemState.Hidden; } }


        private GUIItemState ItemState { get; set; }


        protected GUIItem(Client game, Screen screen) : base(game, screen)
        {
            if (MainTextRenderer == null)
                //MainTextRenderer = new FontRenderer(FontLoader.LoadFile("PixelUnicode.fnt"), Game.Content.Load<Texture2D>("Fonts\\PixelUnicode_0"));
                //MainTextRenderer = new FontRenderer(FontLoader.LoadFile("PixelUnicode.fnt"), FontLoader.LoadTextures(GraphicsDevice, "PixelUnicode"));
                MainTextRenderer = new FontRenderer(GraphicsDevice, "PixelUnicode");
        }


        public void ToActive() { ItemState = GUIItemState.Active; }
        public void ToJustNowActive() { ItemState = GUIItemState.JustNowActive; }
        public void ToSelected() { ItemState = GUIItemState.Selected; }
        public void ToSelectedMouseHover() { ItemState = GUIItemState.SelectedMouseHover; }
        public void ToNonPressable() { ItemState = GUIItemState.NonPressable; }
        public void ToHidden() { ItemState = GUIItemState.Hidden; }


        public override void Dispose()
        {

            if (_spriteBatch != null)
                _spriteBatch.Dispose();
        }
    }
}
