using Microsoft.Xna.Framework;

namespace MineLib.PGL.Screens.GUI.Text
{
    public abstract class GUIText : GUIItem
    {
        protected string Text { get; private set; }
        protected Color TextColor { get; set; }

        protected Rectangle TextRectangle { get; private set; }


        protected GUIText(Client game, Screen screen, string text, Rectangle textRect, Color textColor) : base(game, screen)
        {
            Text = text;
            TextColor = textColor;
            TextRectangle = textRect;
        }

        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime) { }
    }
}
