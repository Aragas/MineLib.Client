using System;

using Microsoft.Xna.Framework;

namespace MineLib.PGL.Screens.GUI.Box
{
    public sealed class BoxPlayerInfo : GUIBox
    {
        public BoxPlayerInfo(Client game, Screen screen, Rectangle rect, Color style) : base(game, screen, rect, null, style) { }
        protected override void OnButtonPressed(object sender, EventArgs eventArgs) { }

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

        }
    }
}
