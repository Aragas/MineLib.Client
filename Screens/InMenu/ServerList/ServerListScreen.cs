using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Data;
using MineLib.Core.Wrappers;

using MineLib.PGL.Screens.InGame;
using MineLib.PGL.Screens.InMenu.ServerList.ServerEntry;

using Newtonsoft.Json;

using PCLStorage;

namespace MineLib.PGL.Screens.InMenu.ServerList
{
    public sealed class ServerListScreen<T> : Screen where T : struct, IVertexType
    {
        Texture2D MainBackgroundTexture { get; set; }

        Task Parser { get; set; }

        bool ParserIsBusy { get { return Parser != null && !Parser.IsCompleted; } }

        List<Server> Servers { get; set; }

        public const string ServerListFileName = @"ServerList.json";

        public const int NetworkProtocol = 47;


        private void LoadServerList()
        {
            Servers = FileSystemWrapper.LoadSettings<List<Server>>(ServerListFileName, new List<Server>());
        }

        private void SaveServerList()
        {
            using (var stream = FileSystemWrapper.SettingsFolder.CreateFileAsync(ServerListFileName, CreationCollisionOption.OpenIfExists).Result.OpenAsync(FileAccess.ReadAndWrite).Result)
            using (var writer = new StreamWriter(stream))
                writer.Write(JsonConvert.SerializeObject(Servers, Formatting.Indented));
        }

        private void AddServerAndSaveServerList(Server server)
        {
            LoadServerList();

            Servers.Add(server);

            SaveServerList();
        }

        private void ParseServerEntries()
        {
            /*
            if (ParserIsBusy)
                return;

            Parser = new Task(() =>
            {
                if (Servers.Count > 0)
                {
                    IStatusClient serverParser = new StatusClient();

                    // Getting info for each saved server
                    foreach (Server server in Servers)
                        server.ServerResponse = serverParser.GetResponseData(server.Address.IP, server.Address.Port, NetworkProtocol);
                }
            });
            Parser.Start();
            */
        }

        private static Address StringToAddress(string address)
        {
            //string serverAddress = ServerAddressInputBox.InputBoxText;
            string host, port;

            int colonIndex = address.IndexOf(':');
            if (colonIndex != -1)
            {
                port = address.Substring(colonIndex + 1);
                host = address.Substring(0, colonIndex);

            }
            else
            {
                port = "25565";
                host = address;
            }

            return new Address
            {
                IP = host,
                Port = ushort.Parse(port)
            };
        }




        Rectangle GradientUp { get; set; }
        Texture2D GradientUpTexture { get; set; }

        Rectangle GradientDown { get; set; }
        Texture2D GradientDownTexture { get; set; }

        Rectangle BackgroundUp { get; set; }
        Rectangle BackgroundDown { get; set; }

        ServerEntryDrawer<T> ServerEntryDrawer { get; set; }

        int SelectedServerIndex { get; set; }

        public ServerListScreen(Client game) : base(game, "ServerListScreen")
        {
            MainBackgroundTexture = TextureStorage.GUITextures.OptionsBackground;

            GradientUp = new Rectangle(0, 0, ScreenRectangle.Width, 8);
            BackgroundUp = new Rectangle(0, 0, ScreenRectangle.Width, 16);
            GradientDown = new Rectangle(0, 0, ScreenRectangle.Width, 8);
            BackgroundDown = new Rectangle(0, 0, ScreenRectangle.Width, 32);

            GradientUpTexture = CreateGradientUp();
            GradientDownTexture = CreateGradientDown();

            //GUIButton connectButton = AddButtonNavigation("Connect", ButtonNavigationPosition.LeftTop, OnConnectButtonPressed);
            //AddButtonNavigation("Refresh", ButtonNavigationPosition.Top, OnRefreshButtonPressed);
            //AddButtonNavigation("Direct Connection", ButtonNavigationPosition.RightTop, OnDirectConnectionButtonPressed);

            //AddButtonNavigation("Add Server", ButtonNavigationPosition.LeftBottom, OnAddServerButtonPressed);
            //GUIButton editServerButton = AddButtonNavigation("Edit Server", ButtonNavigationPosition.Bottom, OnEditServerButtonPressed);
            //AddButtonNavigation("Return", ButtonNavigationPosition.RightBottom, OnReturnButtonPressed);

            // TODO: Better improve dat shiet
            LoadServerList();
            ParseServerEntries();

            //Servers.Add(new Server { Name = "Shit", Address = new Address() { IP = "127.0.0.1", Port = 25565} });
            SaveServerList();

            ServerEntryDrawer = new ServerEntryDrawer<T>(Game, this, Servers);//, new List<GUIButton> { connectButton, editServerButton });
            ServerEntryDrawer.OnClickedPressed += OnClickedServerEntry;
        }
       
        public override void Update(GameTime gameTime)
        {
            if (InputManager.IsOncePressed(Keys.Back) ||
                (InputManager.IsOncePressed(Buttons.B) && InputManager.CurrentGamePadState.IsButtonUp(Buttons.LeftTrigger) && InputManager.CurrentGamePadState.ThumbSticks.Left == Vector2.Zero))
                AddScreenAndCloseThis(new MainMenuScreen<T>(Game));

            ServerEntryDrawer.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // We can't handle ServerEntryDrawer as a GUIItem, drawing order is important. We draw it in mid-cycle ot this draw call
            ServerEntryDrawer.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            #region Background

            SpriteBatch.Draw(GradientUpTexture, new Vector2(0, 63), GradientUp, Color.White);
            SpriteBatch.Draw(MainBackgroundTexture, Vector2.Zero, BackgroundUp, SecondaryBackgroundColor, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0f);

            SpriteBatch.Draw(GradientDownTexture, new Vector2(0, ScreenRectangle.Height - 128 - 8), GradientDown, Color.White);
            SpriteBatch.Draw(MainBackgroundTexture, new Vector2(0, ScreenRectangle.Height - 128), BackgroundDown, SecondaryBackgroundColor, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0f);

            #endregion

            SpriteBatch.End();
        }

        public override void Dispose()
        {
            if(GradientUpTexture != null)
                GradientUpTexture.Dispose();

            if(GradientDownTexture != null)
                GradientDownTexture.Dispose();

            //SaveServerList(Servers);

            // Unload content only if we are in game
            //if (ScreenManager.GetScreen("GameScreen") != null)
            //    ScreenManager.Content.Unload();
        }


        void OnConnectButtonPressed()
        {
            AddScreenAndCloseThis(new GameScreen<T>(Game, Servers[SelectedServerIndex]));

            //bool status = gameScreen.Connect();
            //AddScreenAndCloseThis(status ? (Screen)gameScreen : new ServerListScreen(GameClient));
        }
        void OnRefreshButtonPressed()
        {
            ParseServerEntries();
        }
        void OnDirectConnectionButtonPressed()
        {
            //AddScreenAndCloseThis(new DirectConnectionScreen(GameClient));
        }

        void OnAddServerButtonPressed()
        {
            //AddScreenAndCloseThis(new AddServerScreen(GameClient));
        }
        void OnEditServerButtonPressed()
        {
            //AddScreenAndCloseThis(new EditServerScreen(GameClient, Servers, SelectedServerIndex));
        }
        void OnReturnButtonPressed()
        {
            AddScreenAndCloseThis(new MainMenuScreen<T>(Game));
        }

        void OnClickedServerEntry(int index)
        {
            SelectedServerIndex = index;
        }


        private Texture2D CreateGradientUp()
        {
            Texture2D backgroundTex = new Texture2D(GraphicsDevice, ScreenRectangle.Width, 8);
            Color[] bgc = new Color[ScreenRectangle.Width * 8];

            for (int i = 1; i < bgc.Length + 1; i++)
            {
                if ((float)i / ScreenRectangle.Width > 1f)
                    bgc[i - 1] = new Color(0, 0, 0, 255);

                if ((float)i / ScreenRectangle.Width > 2f)
                    bgc[i - 1] = new Color(0, 0, 0, 225);

                if ((float)i / ScreenRectangle.Width > 3f)
                    bgc[i - 1] = new Color(0, 0, 0, 195);

                if ((float)i / ScreenRectangle.Width > 4f)
                    bgc[i - 1] = new Color(0, 0, 0, 165);

                if ((float)i / ScreenRectangle.Width > 5f)
                    bgc[i - 1] = new Color(0, 0, 0, 135);

                if ((float)i / ScreenRectangle.Width > 6f)
                    bgc[i - 1] = new Color(0, 0, 0, 105);

                if ((float)i / ScreenRectangle.Width > 7f)
                    bgc[i - 1] = new Color(0, 0, 0, 75);

                if ((float)i / ScreenRectangle.Width > 8f)
                    bgc[i - 1] = new Color(0, 0, 0, 45);
            }
            backgroundTex.SetData(bgc);
            return backgroundTex;
        }

        private Texture2D CreateGradientDown()
        {
            Texture2D backgroundTex = new Texture2D(GraphicsDevice, ScreenRectangle.Width, 8);
            Color[] bgc = new Color[ScreenRectangle.Width * 8];

            for (int i = 1; i < bgc.Length + 1; i++)
            {
                if ((float)i / ScreenRectangle.Width > 1f)
                    bgc[i - 1] = new Color(0, 0, 0, 45);

                if ((float)i / ScreenRectangle.Width > 2f)
                    bgc[i - 1] = new Color(0, 0, 0, 75);

                if ((float)i / ScreenRectangle.Width > 3f)
                    bgc[i - 1] = new Color(0, 0, 0, 105);

                if ((float)i / ScreenRectangle.Width > 4f)
                    bgc[i - 1] = new Color(0, 0, 0, 135);

                if ((float)i / ScreenRectangle.Width > 5f)
                    bgc[i - 1] = new Color(0, 0, 0, 165);

                if ((float)i / ScreenRectangle.Width > 6f)
                    bgc[i - 1] = new Color(0, 0, 0, 195);

                if ((float)i / ScreenRectangle.Width > 7f)
                    bgc[i - 1] = new Color(0, 0, 0, 225);

                if ((float)i / ScreenRectangle.Width > 8f)
                    bgc[i - 1] = new Color(0, 0, 0, 255);
            }
            backgroundTex.SetData(bgc);
            return backgroundTex;
        }
    }
}
