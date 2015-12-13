using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.GUI.Grid
{
    public class GUIGrid : GUIItem
    {
        protected Texture2D BackgroundTexture { get; set; }
        public Rectangle BackgroundRectangle { get; set; }

        protected Texture2D FrameTexture { get; set; }
        protected Point FrameSize { get; } = new Point(2, 2);
        protected Rectangle FrameTopRectangle { get; set; }
        protected Rectangle FrameBottomRectangle { get; set; }
        protected Rectangle FrameLeftRectangle { get; set; }
        protected Rectangle FrameRightRectangle { get; set; }

        public GUIGrid(Client game, Screen screen) : base(game, screen, false)
        {
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        }
    }
}
