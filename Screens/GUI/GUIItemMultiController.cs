using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MineLib.PGL.Screens.GUI.Button;
using MineLib.PGL.Screens.GUI.InputBox;

namespace MineLib.PGL.Screens.GUI
{
    /// <summary>
    /// Controlls many GUIItems as one entity.
    /// </summary>
    public sealed class GUIItemMultiController
    {
        private List<GUIItem> _items = new List<GUIItem>();

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _items.Count; i++)
                _items[i].Update(gameTime);
            
            //if (!Screen.Daisywheel.IsHidden)
            //    return;

            if (InputManager.MenuUIUp)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    GUIItem itemCurrent = _items[i];
                    if (!itemCurrent.CanBeSelected)
                        continue;

                    if (itemCurrent.IsSelected)
                    {
                        GUIItem itemPrevious = null;

                        #region Find previous CanBeSelected item

                        for (var j = i - 1; j >= 0; j--)
                        {
                            var item = _items[j];

                            if (item.CanBeSelected)
                            {
                                itemPrevious = item; // Previous
                                break;
                            }
                        }
                        #endregion

                        if (itemPrevious == null)
                            return; // Nothing found

                        itemCurrent.ToActive();

                        itemPrevious.ToSelected();
                        return;
                    }
                    else if (i == _items.Count - 1)
                    {
                        #region Find first CanBeSelected item
                        for (var j = 0; j < _items.Count; j++)
                        {
                            var item = _items[j];
                            if (item.CanBeSelected)
                            {
                                item.ToSelected();
                                break;
                            }
                        }
                        #endregion

                        return;
                    }
                }
            }
            if (InputManager.MenuUIDown)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    GUIItem itemCurrent = _items[i];
                    if (!itemCurrent.CanBeSelected)
                        continue;

                    if (itemCurrent.IsSelected)
                    {
                        GUIItem itemNext = null;

                        #region Find next CanBeSelected item
                        for (var j = i + 1; j < _items.Count; j++)
                        {
                            var item = _items[j];
                            if (item.CanBeSelected)
                            {
                                itemNext = item; // Next
                                break;
                            }
                        }
                        #endregion

                        if (itemNext == null)
                            return; // Nothing found

                        itemCurrent.ToActive();

                        itemNext.ToSelected();
                        return;
                    }
                    else if (i == _items.Count - 1)
                    {
                        #region Find last CanBeSelected item
                        for (var j = _items.Count - 1; j >= 0; j--)
                        {
                            var item = _items[j];
                            if (item.CanBeSelected)
                            {
                                item.ToSelected();
                                break;
                            }
                        }
                        #endregion

                        return;
                    }
                }
            }

            if (InputManager.MenuUIPressed)
                foreach (var guiItem in _items)
                {
                    if (guiItem.IsSelected)
                    {
                        (guiItem as GUIButton)?.PressButton();
                        (guiItem as GUIInputBox)?.PressEnter();
                    }
                }
        }
        public void Draw(GameTime gameTime)
        {
            foreach (var guiItem in _items)
                guiItem.Draw(gameTime);
        }

        public void AddGUIItem(GUIItem item) { _items.Add(item); }
        public void AddGUIItems(params GUIItem[] item) { _items.AddRange(item); }

        public void Clear() { _items.Clear(); }
    }
}
