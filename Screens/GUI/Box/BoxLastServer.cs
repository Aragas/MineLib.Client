using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Data;

using MineLib.PGL.Screens.GUI.Grid;
using MineLib.PGL.Screens.GUI.Text;

namespace MineLib.PGL.Screens.GUI.Box
{
    public struct LastServer
    {
        public Texture2D Image { get; set; }
        public string Name { get; set; }
        public string LastPlayed { get; set; }
    }
    public sealed class BoxLastServer : GUIBox
    {
        private event EventHandler<ConnectionEventArgs> OnConnection;

        private LastServer Server { get; }


        public BoxLastServer(Client game, Screen screen, Rectangle rect, EventHandler<ConnectionEventArgs> onButton, LastServer server, Color style) : base(game, screen, rect, "Continue", style)
        {
            Server = server;
            OnConnection += onButton;


            var yOffset = 0;

            var serverNameTextRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += BoxRectangle.Y + BoxGrid.OffsetY,
                BoxGrid.Width, FontNormalSize);
            var ServerNameText = new BaseText(Game, Screen, Server.Name, serverNameTextRectangle, Color.White);
            AddGUIItem(ServerNameText);

            var GridRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += FontNormalSize,
                BoxGrid.Width, BoxGrid.Height - FontNormalSize - ButtonSize.Height - FontSmallSize);
            var Grid = new BaseGrid(Game, Screen, GridRectangle);
            AddGUIItem(Grid);
            
            var lastPlayedTextRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += Grid.BackgroundRectangle.Height,
                BoxGrid.Width, FontSmallSize);
            var LastPlayedText = new BaseText(Game, Screen, "Last Played: " + Server.LastPlayed, lastPlayedTextRectangle, Color.LightGray);
            AddGUIItem(LastPlayedText);
        }
        protected override void OnButtonPressed(object sender, EventArgs eventArgs)
        {
            OnConnection?.Invoke(this, new ConnectionEventArgs(new Server()));
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

            if (OnConnection != null)
                foreach (var @delegate in OnConnection.GetInvocationList())
                    OnConnection -= (EventHandler<ConnectionEventArgs>) @delegate;
        }
    }
}
