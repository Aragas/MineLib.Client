using Microsoft.Xna.Framework;

namespace MineLib.PCL.Graphics.Screens.GUI.Box
{
    public sealed class BoxPlayerInfo : GUIBox
    {
        public BoxPlayerInfo(Client game, Screen screen, Rectangle rect, Color style) : base(game, screen, rect, null, style) { }
        protected override void OnButtonPressed() { }
    }
}
