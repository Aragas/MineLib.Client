using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MineLib.PGL.Screens.GUI.Button;
using MineLib.PGL.Screens.GUI.InputBox;

namespace MineLib.PGL.Screens.GUI
{
    /// <summary>
    /// Controlls many buttons as one entity.
    /// Adds GamePad support.
    /// Only Updates. Draw manually.
    /// </summary>
    public sealed class GUIItemMultiController : GUIItem
    {
        private readonly List<GUIItem> _items = new List<GUIItem>();


        public GUIItemMultiController(Client game, Screen screen) : base(game, screen) { }

        public override void Update(GameTime gameTime)
        {
            foreach (var guiItem in _items)
                guiItem.Update(gameTime);

            if (!Screen.Daisywheel.IsHidden)
                return;
            

            #region DPad handler

            // Don't blame me for this. Mah brain is broken.
            if (InputManager.IsOncePressed(Buttons.DPadUp) && InputManager.CurrentGamePadState.IsButtonUp(Buttons.LeftTrigger) || InputManager.IsOncePressed(Keys.Up))
            {
                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    // if button was found
                    if (_items[i].IsSelected)
                    {
                        // find the previous active button
                        for (int j = i; j >= 0; j--)
                        {
                            // if button was found, make next button active and previous button manually selected
                            if (_items[j].IsActive)
                            {
                                _items[i].ToActive();
                                _items[j].ToSelected();
                                return;
                            }
                        }
                    }

                    // if button wasn't found
                    else if (i == 0)
                    {
                        // find last active button and make it manually selected
                        for (int j = _items.Count - 1; j >= 0; j--)
                        {
                            if (_items[j].IsActive)
                            {
                                _items[j].ToSelected();
                                return;
                            }
                        }
                    }
                }
            }

            // Don't blame me for this. Mah brain is broken.
            if (InputManager.IsOncePressed(Buttons.DPadDown) && InputManager.CurrentGamePadState.IsButtonUp(Buttons.LeftTrigger) || InputManager.IsOncePressed(Keys.Down))
            {
                // check if any button is manually selected
                for (int i = 0; i < _items.Count; i++)
                {
                    // if button was found
                    if (_items[i].IsSelected)
                    {
                        // find the next active button
                        for (int j = i; j < _items.Count; j++)
                        {
                            // if button was found, make previous button active and next button manually selected
                            if (_items[j].IsActive)
                            {
                                _items[i].ToActive();
                                _items[j].ToSelected();
                                return;
                            }
                        }
                    }

                    // if button wasn't found
                    else if (i == _items.Count - 1)
                    {
                        // find first active button and make it manually selected
                        for (int j = 0; j < _items.Count; j++)
                        {
                            if (_items[j].IsActive)
                            {
                                _items[j].ToSelected();
                                return;
                            }
                        }
                    }
                }
            }

            #endregion

            if (InputManager.IsOncePressed(Buttons.A) && InputManager.CurrentGamePadState.IsButtonUp(Buttons.LeftTrigger) && InputManager.CurrentGamePadState.ThumbSticks.Left == Vector2.Zero)
                foreach (var guiItem in _items)
                {
                    if (guiItem is GUIButton && guiItem.IsSelected)
                        ((GUIButton) guiItem).PressButton();

                    else if (guiItem is GUIInputBox)
                        ((GUIInputBox) guiItem).ToSelected();
                }
        }

        public override void Draw(GameTime gameTime)
        {
            //foreach (var guiItem in _items)
            //    guiItem.Draw(gameTime);
        }

        public override void Dispose() { _items.Clear(); }


        public void AddGUIItem(GUIItem item) { _items.Add(item); }
    }
}
