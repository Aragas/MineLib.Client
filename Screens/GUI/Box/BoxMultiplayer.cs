using System;

using Microsoft.Xna.Framework;

using MineLib.PGL.Screens.GUI.Grid;

namespace MineLib.PGL.Screens.GUI.Box
{
    public sealed class BoxMultiplayer : GUIBox
    {
        private event EventHandler OnButton;

        public BoxMultiplayer(Client game, Screen screen, Rectangle rect, EventHandler onButton, Color style) : base(game, screen, rect, "Servers", style)
        {
            OnButton += onButton;


            var GridRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                BoxRectangle.Y + BoxGrid.OffsetY,
                BoxGrid.Width, BoxGrid.Height - ButtonSize.Height);
            var Grid = new BaseGrid(Game, Screen, GridRectangle);
            AddGUIItem(Grid);
        }
        protected override void OnButtonPressed(object sender, EventArgs eventArgs)
        {
            OnButton?.Invoke(this, EventArgs.Empty);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        }

        public override void Dispose()
        {
            base.Dispose();

            if (OnButton != null)
                foreach (var @delegate in OnButton.GetInvocationList())
                    OnButton -= (EventHandler) @delegate;
        }
    }
}
