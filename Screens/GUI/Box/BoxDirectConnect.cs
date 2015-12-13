using System;

using Microsoft.Xna.Framework;

using MineLib.Core.Data;

using MineLib.PGL.Screens.GUI.InputBox;
using MineLib.PGL.Screens.GUI.Text;

namespace MineLib.PGL.Screens.GUI.Box
{
    public sealed class BoxDirectConnect : GUIBox
    {
        private event EventHandler<ConnectionEventArgs> OnConnection;
        
        private string ServerName { get; } = "Server Name:";      
        private string ServerHost { get; } = "Server Host:";
        private string ServerProtocol { get; } = "Server Protocolg:";

        // CheckBox


        public BoxDirectConnect(Client game, Screen screen, Rectangle rect, EventHandler<ConnectionEventArgs> onButton, Color style) : base(game, screen, rect, "Connect", style)
        {
            OnConnection += onButton;


            var yOffset = 0;

            var serverNameTextRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += BoxRectangle.Y + BoxGrid.OffsetY,
                BoxGrid.Width, FontNormalSize);
            var ServerNameText = new BaseText(Game, Screen, ServerName, serverNameTextRectangle, Color.White);
            AddGUIItem(ServerNameText);

            var serverNameInputBoxRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += FontNormalSize,
                BoxGrid.Width, FontNormalSize);
            var ServerNameInputBox = new BaseInputBox(Game, Screen, serverNameInputBoxRectangle, null, UsingColor);
            AddGUIItem(ServerNameInputBox);
            
            var serverHostTextRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += FontNormalSize,
                BoxGrid.Width, FontNormalSize);
            var ServerHostText = new BaseText(Game, Screen, ServerHost, serverHostTextRectangle, Color.White);
            AddGUIItem(ServerHostText);

            var serverHostInputBoxRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += FontNormalSize,
                BoxGrid.Width, FontNormalSize);
            var ServerHostInputBox = new BaseInputBox(Game, Screen, serverHostInputBoxRectangle, null, UsingColor);
            AddGUIItem(ServerHostInputBox);
            
            var serverProtocolTextRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += FontNormalSize,
                BoxGrid.Width, FontNormalSize);
            var ServerProtocolText = new BaseText(Game, Screen, ServerProtocol, serverProtocolTextRectangle, Color.White);
            AddGUIItem(ServerProtocolText);

            var serverProtocolListRectangle = new Rectangle(
                BoxRectangle.X + BoxGrid.OffsetX,
                yOffset += FontNormalSize,
                BoxGrid.Width, FontNormalSize);
            //var ServerHostInputBox = new BaseInputBox(Game, Screen, serverHostInputBoxRectangle, null, true);
            //AddGUIItem(ServerHostInputBox);
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
