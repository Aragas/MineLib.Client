using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Data;

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
        private event BoxEventHandler OnConnection;

        private Color TextColor { get {  return Color.White;} }
        private Color InfoColor { get { return Color.LightGray; } }

        private BaseText ServerNameText { get; set; }

        private Texture2D ImageTexture { get; set; }
        private Rectangle ImageRectangle { get; set; }

        private Texture2D ImageFrameTexture { get; set; }
        private Vector2 ImageFrameSize { get { return new Vector2(2); } }
        private Rectangle ImageFrameTopRectangle { get; set; }
        private Rectangle ImageFrameBottomRectangle { get; set; }
        private Rectangle ImageFrameLeftRectangle { get; set; }
        private Rectangle ImageFrameRightRectangle { get; set; }

        private BaseText LastPlayedText { get; set; }

        private LastServer Server { get; set; }


        public BoxLastServer(Client game, Screen screen, Rectangle rect, BoxEventHandler onButton, LastServer server, Color style) : base(game, screen, rect, "Continue", style)
        {
            Server = server;
            OnConnection += onButton;

            var serverNameTextRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                BoxRectangle.Y + BaseYOffset,
                ElementWidth,
                ElementHeight - (int)(5 * ButtonScale));
            ServerNameText = new BaseText(Game, Screen, Server.Name, serverNameTextRectangle, TextColor);

            var lastPlayedTextRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                Button.ButtonRectangle.Y - (int)(20 * ButtonScale) - ElementOffset,
                BoxRectangle.Width - (int)(BaseXOffset * 2.0f),
                ElementHeight - (int)(10 * ButtonScale));
            LastPlayedText = new BaseText(Game, Screen, "Last Played: " + Server.LastPlayed, lastPlayedTextRectangle, InfoColor);

            ImageRectangle = new Rectangle(
                BoxRectangle.X + BaseXOffset,
                serverNameTextRectangle.Y + serverNameTextRectangle.Height,
                BoxRectangle.Width - (int)(BaseXOffset * 2.0f),
                BoxRectangle.Height - Button.ButtonRectangle.Height - serverNameTextRectangle.Height - lastPlayedTextRectangle.Height - (int)(BaseYOffset * 2.0f) - ElementOffset);

            ImageFrameTopRectangle = new Rectangle(ImageRectangle.X, ImageRectangle.Y, ImageRectangle.Width, (int)ImageFrameSize.Y);
            ImageFrameBottomRectangle = new Rectangle(ImageRectangle.X, (int)(ImageRectangle.Y + ImageRectangle.Height - ImageFrameSize.Y), ImageRectangle.Width, (int)ImageFrameSize.Y);
            ImageFrameLeftRectangle = new Rectangle(ImageRectangle.X, ImageRectangle.Y, (int)ImageFrameSize.X, ImageRectangle.Height);
            ImageFrameRightRectangle = new Rectangle((int)(ImageRectangle.X + ImageRectangle.Width - ImageFrameSize.X), ImageRectangle.Y, (int)ImageFrameSize.X, ImageRectangle.Height);

            ImageTexture = Server.Image;
            ImageTexture = new Texture2D(GraphicsDevice, 1, 1);
            ImageTexture.SetData(new[] { new Color(150, 150, 150, 255) });

            ImageFrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            ImageFrameTexture.SetData(new[] { new Color(0, 0, 0, 255) });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ServerNameText.Update(gameTime);
            LastPlayedText.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ServerNameText.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            SpriteBatch.Draw(ImageTexture, ImageRectangle, Rectangle.Empty, Color.White);

            SpriteBatch.Draw(ImageFrameTexture, ImageFrameTopRectangle,     new Rectangle(0, 0, ImageRectangle.X, (int)ImageFrameSize.Y), Color.White);
            SpriteBatch.Draw(ImageFrameTexture, ImageFrameBottomRectangle,  new Rectangle(0, 0, ImageRectangle.X, (int)ImageFrameSize.Y), Color.White);
            SpriteBatch.Draw(ImageFrameTexture, ImageFrameLeftRectangle,    new Rectangle(0, 0, ImageRectangle.Y, (int)ImageFrameSize.X), Color.White);
            SpriteBatch.Draw(ImageFrameTexture, ImageFrameRightRectangle,   new Rectangle(0, 0, ImageRectangle.Y, (int)ImageFrameSize.X), Color.White);

            SpriteBatch.End();

            LastPlayedText.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (OnConnection != null)
                foreach (var @delegate in OnConnection.GetInvocationList())
                    OnConnection -= (BoxEventHandler) @delegate;

            if (ServerNameText != null)
                ServerNameText.Dispose();

            if (ImageTexture != null)
                ImageTexture.Dispose();

            if (ImageFrameTexture != null)
                ImageFrameTexture.Dispose();

            if (LastPlayedText != null)
                LastPlayedText.Dispose();
        }

        protected override void OnButtonPressed()
        {
            if (OnConnection != null)
                OnConnection(new Server());
        }
    }
}
