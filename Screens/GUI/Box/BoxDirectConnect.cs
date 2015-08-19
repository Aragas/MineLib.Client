using Microsoft.Xna.Framework;

using MineLib.Core.Data;

using MineLib.PGL.Screens.GUI.InputBox;
using MineLib.PGL.Screens.GUI.Text;

namespace MineLib.PGL.Screens.GUI.Box
{
    public delegate void BoxEventHandler(Server entry);
    public sealed class BoxDirectConnect : GUIBox
    {
        private event BoxEventHandler OnConnection;

        private Color TextColor { get { return Color.White; } }

        private string ServerName { get { return "Server Name:"; } }
        private BaseText ServerNameText { get; set; }
        private BaseInputBox ServerNameInputBox { get; set; }
        
        private string ServerHost { get { return "Server Host:"; } }
        private BaseText ServerHostText { get; set; }
        private BaseInputBox ServerHostInputBox { get; set; }

        private string ServerProtocol { get { return "Server Protocol:"; } }
        private BaseText ServerProtocolText { get; set; }
        //private BaseInputBox ServerProtocolInputBox { get; set; }

        // CheckBox


        public BoxDirectConnect(Client game, Screen screen, Rectangle rect, BoxEventHandler onButton, Color style) : base(game, screen, rect, "Connect", style)
        {
            OnConnection += onButton;

            var serverNameTextRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                BoxRectangle.Y + BaseYOffset,
                ElementWidth,
                ElementHeight - (int)(5 * ButtonScale));
            ServerNameText = new BaseText(Game, Screen, ServerName, serverNameTextRectangle, TextColor);

            var serverNameInputBoxRectangle = new Rectangle(
                BoxWidthCenter - (int)(ElementWidth * 0.5f),
                serverNameTextRectangle.Y + serverNameTextRectangle.Height,
                ElementWidth,
                ElementHeight);
            ServerNameInputBox = new BaseInputBox(Game, Screen, serverNameInputBoxRectangle, null, UsingColor);


            var serverHostTextRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                serverNameInputBoxRectangle.Y + serverNameInputBoxRectangle.Height,
                ElementWidth,
                ElementHeight - (int)(5 * ButtonScale));
            ServerHostText = new BaseText(Game, Screen, ServerHost, serverHostTextRectangle, TextColor);

            var serverHostInputBoxRectangle = new Rectangle(
                BoxWidthCenter - (int)(ElementWidth * 0.5f),
                serverHostTextRectangle.Y + serverHostTextRectangle.Height,
                ElementWidth,
                ElementHeight);
            ServerHostInputBox = new BaseInputBox(Game, Screen, serverHostInputBoxRectangle, null, UsingColor);


            var serverProtocolTextRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                serverHostInputBoxRectangle.Y + serverHostInputBoxRectangle.Height,
                ElementWidth,
                ElementHeight - (int)(5 * ButtonScale));
            ServerProtocolText = new BaseText(Game, Screen, ServerProtocol, serverProtocolTextRectangle, TextColor);

            var serverProtocolListRectangle = new Rectangle(
                BoxWidthCenter - (int)(ElementWidth * 0.5f),
                serverProtocolTextRectangle.Y + serverProtocolTextRectangle.Height + ElementOffset,
                ElementWidth,
                ElementHeight);
            //ServerHostInputBox = new BaseInputBox(Game, Screen, serverHostInputBoxRectangle, null, true);
        }

        public override void AddToGUIItemMultiController(GUIItemMultiController guiItemMultiController)
        {
            guiItemMultiController.AddGUIItem(ServerNameInputBox);
            guiItemMultiController.AddGUIItem(ServerHostInputBox);

            base.AddToGUIItemMultiController(guiItemMultiController);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ServerNameText.Update(gameTime);
            //ServerNameInputBox.Update(gameTime);

            ServerHostText.Update(gameTime);
            //ServerHostInputBox.Update(gameTime);

            ServerProtocolText.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ServerNameText.Draw(gameTime);
            ServerNameInputBox.Draw(gameTime);

            ServerHostText.Draw(gameTime);
            ServerHostInputBox.Draw(gameTime);

            ServerProtocolText.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (OnConnection != null)
                foreach (var @delegate in OnConnection.GetInvocationList())
                    OnConnection -= (BoxEventHandler) @delegate;

            if (ServerNameText != null)
                ServerNameText.Dispose();

            if (ServerNameInputBox != null)
                ServerNameInputBox.Dispose();


            if (ServerHostText != null)
                ServerHostText.Dispose();

            if (ServerHostInputBox != null)
                ServerHostInputBox.Dispose();


            if (ServerProtocolText != null)
                ServerProtocolText.Dispose();
        }

        protected override void OnButtonPressed()
        {
            if (OnConnection != null)
                OnConnection(new Server());
        }

    }
}
