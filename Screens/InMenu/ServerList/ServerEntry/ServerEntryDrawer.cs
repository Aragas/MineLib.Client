using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Data;

using MineLib.PGL.Screens.GUI;
using MineLib.PGL.Screens.GUI.Button;

namespace MineLib.PGL.Screens.InMenu.ServerList.ServerEntry
{
    public delegate void ServerEntryEventHandler(int index);
    public sealed class ServerEntryDrawer : GUIItem
    {
        public event ServerEntryEventHandler OnClickedPressed;

        Action OnEntryClicked;

        #region Constants

        const int TOP_BACKGROUND = 64;
        const int TOP_BACKGROUND_GRADIENT = 8;

        const int BOTTOM_BACKGROUND = 128;
        const int BOTTOM_BACKGROUND_GRADIENT = 8;

        const int SLIDER_WIDTH = 16;

        const int SPACE_BETWEEN_ELEMENTS = 6;

        const int SECOND_LINE = 32;

        const int SPACE_FOR_DATA = 200;

        #endregion

        Vector2 ServerEntrySize = new Vector2(700, 76);
        //Vector2 ServerEntrySize = new Vector2(608, 72); // Vanilla settings
        Vector2 FrameSize = new Vector2(2);

        #region Resources

        int CurrentProtocol { get { return ServerListScreen.NetworkProtocol; } }

        List<Server> Servers { get; set; }

        bool[] ServerSelected { get; set; }

        Rectangle[] ServerEntryRectangles { get; set; }

        Texture2D[] ServerEntryImageTextures { get; set; }
        Rectangle[] ServerEntryImageRectangles { get; set; }

        Rectangle[] WhiteFrameTopRectangles { get; set; }
        Rectangle[] WhiteFrameBottomRectangles { get; set; }
        Rectangle[] WhiteFrameLeftRectangles { get; set; }
        Rectangle[] WhiteFrameRightRectangles { get; set; }

        Rectangle SliderRectangle { get; set; }
        Rectangle SliderSelectedRectangle { get; set; }

        Rectangle AvailableScreenRectangle { get; set; }

        Rectangle[] ServerNameRectangles { get; set; }
        Rectangle[] ServerPlayersVectors { get; set; }
        Rectangle[] ServerAddressVectors { get; set; }
        Rectangle[] ServerPingVectors { get; set; }

        Vector2 BackgroundVector { get; set; }

        Texture2D WhiteFrameTexture { get; set; }
        Texture2D ServerEntryImageTexture { get; set; }
        Texture2D BlackTexture { get; set; }
        Texture2D BackgroundTexture { get; set; }

        float TextScale { get; set; }

        #endregion

        public ServerEntryDrawer(Client game, ServerListScreen screen, List<Server> server) : base(game, screen)
        {
            Servers = server;

            ServerSelected = new bool[Servers.Count];

            ServerEntryRectangles = new Rectangle[Servers.Count];

            ServerEntryImageTextures = new Texture2D[Servers.Count];
            ServerEntryImageRectangles = new Rectangle[Servers.Count];

            WhiteFrameTopRectangles = new Rectangle[Servers.Count];
            WhiteFrameBottomRectangles = new Rectangle[Servers.Count];
            WhiteFrameLeftRectangles = new Rectangle[Servers.Count];
            WhiteFrameRightRectangles = new Rectangle[Servers.Count];

            ServerNameRectangles    = new Rectangle[Servers.Count];
            ServerPlayersVectors    = new Rectangle[Servers.Count];
            ServerAddressVectors    = new Rectangle[Servers.Count];
            ServerPingVectors       = new Rectangle[Servers.Count];





            BackgroundTexture = Screen.TextureStorage.GUITextures.OptionsBackground;

            ServerEntryImageTexture = new Texture2D(GraphicsDevice, 1, 1);
            ServerEntryImageTexture.SetData(new[] { new Color(255, 255, 255, 255) });

            BlackTexture = new Texture2D(GraphicsDevice, 1, 1);
            BlackTexture.SetData(new[] { new Color(0, 0, 0, 255) });

            WhiteFrameTexture = new Texture2D(GraphicsDevice, 1, 1);
            WhiteFrameTexture.SetData(new[] { Color.LightGray });

            #region First Load

            BackgroundVector = Vector2.Zero;

            for (int i = 0; i < Servers.Count; i++)
            {
                ServerEntryRectangles[i] = GetServerEntryRectangle(i);

                ServerEntryImageRectangles[i] = new Rectangle(ServerEntryRectangles[i].X + SPACE_BETWEEN_ELEMENTS, ServerEntryRectangles[i].Y + SPACE_BETWEEN_ELEMENTS, ServerEntryImageTexture.Width, ServerEntryImageTexture.Height);
                ServerEntryImageRectangles[i] = new Rectangle(ServerEntryRectangles[i].X + SPACE_BETWEEN_ELEMENTS, ServerEntryRectangles[i].Y + SPACE_BETWEEN_ELEMENTS, ServerEntryImageTexture.Width, ServerEntryImageTexture.Height);

                WhiteFrameTopRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y, (int)ServerEntrySize.X, (int)FrameSize.Y);
                WhiteFrameBottomRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, (int)(ServerEntryRectangles[i].Y + ServerEntrySize.Y - FrameSize.Y), (int)ServerEntrySize.X, (int)FrameSize.Y);
                WhiteFrameLeftRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y, (int)FrameSize.X, (int)ServerEntrySize.Y);
                WhiteFrameRightRectangles[i] = new Rectangle((int)(ServerEntryRectangles[i].X + ServerEntrySize.X - FrameSize.X), ServerEntryRectangles[i].Y, (int)FrameSize.X, (int)ServerEntrySize.Y);

                SliderRectangle = new Rectangle(ServerEntryRectangles[0].X + ServerEntryRectangles[0].Width, ServerEntryRectangles[0].Y - TOP_BACKGROUND_GRADIENT, SLIDER_WIDTH, ScreenRectangle.Width - TOP_BACKGROUND - BOTTOM_BACKGROUND);

                ServerNameRectangles[i] = new Rectangle(ServerEntryImageRectangles[i].X + ServerEntryImageRectangles[i].Width + SPACE_BETWEEN_ELEMENTS, 
                    ServerEntryImageRectangles[i].Y,
                    200, 50);
                ServerPlayersVectors[i] = new Rectangle(ServerEntryRectangles[i].X + ServerEntryRectangles[i].Width - SPACE_FOR_DATA, ServerNameRectangles[i].Y,
                    SPACE_FOR_DATA, 50);
                ServerAddressVectors[i] = new Rectangle(ServerNameRectangles[i].X, ServerNameRectangles[i].Y + SECOND_LINE,
                    200, 50);
                ServerPingVectors[i] = new Rectangle(ServerEntryRectangles[i].X + ServerEntryRectangles[i].Width - SPACE_FOR_DATA, ServerAddressVectors[i].Y,
                    SPACE_FOR_DATA, 50);
            }

            #endregion

            AvailableScreenRectangle = new Rectangle(0, TOP_BACKGROUND, ScreenRectangle.Width, ScreenRectangle.Height - TOP_BACKGROUND - BOTTOM_BACKGROUND);

            TextScale = 0.125f;
        }

        /// <summary>
        /// Link GUIButtons to ServerEntryDrawer.
        /// </summary>
        /// <param name="screen">Screen</param>
        /// <param name="server">Server list</param>
        /// <param name="buttons">GUIButtons</param>
        public ServerEntryDrawer(Client game, ServerListScreen screen, List<Server> server, IEnumerable<GUIButton> buttons) : base(game, screen)
        {
            Servers = server;

            ServerSelected = new bool[Servers.Count];

            ServerEntryRectangles = new Rectangle[Servers.Count];

            ServerEntryImageTextures = new Texture2D[Servers.Count];
            ServerEntryImageRectangles = new Rectangle[Servers.Count];

            WhiteFrameTopRectangles = new Rectangle[Servers.Count];
            WhiteFrameBottomRectangles = new Rectangle[Servers.Count];
            WhiteFrameLeftRectangles = new Rectangle[Servers.Count];
            WhiteFrameRightRectangles = new Rectangle[Servers.Count];

            ServerNameRectangles = new Rectangle[Servers.Count];
            ServerPlayersVectors = new Rectangle[Servers.Count];
            ServerAddressVectors = new Rectangle[Servers.Count];
            ServerPingVectors = new Rectangle[Servers.Count];

            foreach (GUIButton guiButton in buttons)
            {
                guiButton.ToNonPressable();
                OnEntryClicked += guiButton.ToActive;
            }
        }


        void MenuUp()
        {
            for (int i = ServerSelected.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                    break;

                if (ServerSelected[i])
                {
                    ServerSelected[i] = false;
                    ServerSelected[i - 1] = true;

                    if(OnClickedPressed != null)
                        OnClickedPressed(i - 1);

                    break;
                }
            }

            if (BackgroundVector.Y >= 0)
                return;

            BackgroundVector = new Vector2(BackgroundVector.X, BackgroundVector.Y + ServerEntrySize.Y);

            for (int i = 0; i < Servers.Count; i++)
                ServerEntryRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y + (int)ServerEntrySize.Y, (int)ServerEntrySize.X, (int)ServerEntrySize.Y);
            
            UpdateOthersPositions();
        }
        void MenuDown()
        {
            int visibleY = ScreenRectangle.Height - (64 + 8 + 128 + 8);
            double maxEntryToShow = Math.Floor(visibleY / ServerEntrySize.Y);


            for (int i = 0; i < ServerSelected.Length; i++)
            {
                if (ServerSelected[i])
                {
                    if (i != ServerSelected.Length - 1)
                    {
                        ServerSelected[i] = false;
                        ServerSelected[i + 1] = true;

                        if(OnClickedPressed != null)
                            OnClickedPressed(i + 1);
                    }

                    if (ServerEntrySize.Y * maxEntryToShow > ServerEntrySize.Y * (i + 1))
                        return;

                    if (ServerEntrySize.Y * (i + 1) > ServerEntrySize.Y * (ServerSelected.Length - 1))
                        return;

                    break;
                }
            }

            BackgroundVector = new Vector2(BackgroundVector.X, BackgroundVector.Y - ServerEntrySize.Y);

            for (int i = 0; i < Servers.Count; i++)
                ServerEntryRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y - (int)ServerEntrySize.Y, (int)ServerEntrySize.X, (int)ServerEntrySize.Y);
            
            UpdateOthersPositions();
        }

        void MouseScrollUp()
        {
            int mouse = 10;

            if (BackgroundVector.Y >= 0)
                return;

            BackgroundVector = new Vector2(BackgroundVector.X, BackgroundVector.Y + mouse);

            for (int i = 0; i < Servers.Count; i++)
                ServerEntryRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y + mouse, (int)ServerEntrySize.X, (int)ServerEntrySize.Y);
            
            UpdateOthersPositions();
        }
        void MouseScrollDown()
        {
            if (ServerEntryRectangles[ServerEntryRectangles.Length - 1].Y + ServerEntrySize.Y < ScreenRectangle.Height - (128))
                return;

            int mouse = 10;

            BackgroundVector = new Vector2(BackgroundVector.X, BackgroundVector.Y - mouse);

            for (int i = 0; i < Servers.Count; i++)
                ServerEntryRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y - mouse, (int)ServerEntrySize.X, (int)ServerEntrySize.Y);
            
            UpdateOthersPositions();
        }

        void UpdateOthersPositions()
        {
            for (int i = 0; i < Servers.Count; i++)
            {
                ServerEntryImageRectangles[i] = new Rectangle(ServerEntryRectangles[i].X + SPACE_BETWEEN_ELEMENTS, ServerEntryRectangles[i].Y + SPACE_BETWEEN_ELEMENTS, ServerEntryImageTexture.Width, ServerEntryImageTexture.Height);
                ServerEntryImageRectangles[i] = new Rectangle(ServerEntryRectangles[i].X + SPACE_BETWEEN_ELEMENTS, ServerEntryRectangles[i].Y + SPACE_BETWEEN_ELEMENTS, ServerEntryImageTexture.Width, ServerEntryImageTexture.Height);

                WhiteFrameTopRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y, (int)ServerEntrySize.X, (int)FrameSize.Y);
                WhiteFrameBottomRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, (int)(ServerEntryRectangles[i].Y + ServerEntrySize.Y - FrameSize.Y), (int)ServerEntrySize.X, (int)FrameSize.Y);
                WhiteFrameLeftRectangles[i] = new Rectangle(ServerEntryRectangles[i].X, ServerEntryRectangles[i].Y, (int)FrameSize.X, (int)ServerEntrySize.Y);
                WhiteFrameRightRectangles[i] = new Rectangle((int)(ServerEntryRectangles[i].X + ServerEntrySize.X - FrameSize.X), ServerEntryRectangles[i].Y, (int)FrameSize.X, (int)ServerEntrySize.Y);

                ServerNameRectangles[i] = new Rectangle(ServerEntryImageRectangles[i].X + ServerEntryImageRectangles[i].Width + SPACE_BETWEEN_ELEMENTS, ServerEntryImageRectangles[i].Y,
                    200, 50);
                ServerPlayersVectors[i] = new Rectangle(ServerEntryRectangles[i].X + ServerEntryRectangles[i].Width - SPACE_FOR_DATA, ServerNameRectangles[i].Y,
                    SPACE_FOR_DATA, 50);
                ServerAddressVectors[i] = new Rectangle(ServerNameRectangles[i].X, ServerNameRectangles[i].Y + SECOND_LINE,
                    200, 50);
                ServerPingVectors[i] = new Rectangle(ServerEntryRectangles[i].X + ServerEntryRectangles[i].Width - SPACE_FOR_DATA, ServerAddressVectors[i].Y,
                    SPACE_FOR_DATA, 50);
            }
        }


        public override void Update(GameTime gameTime)
        {
            #region Mouse selection handling

            var mouse = InputManager.CurrentMouseState;

            for (int i = 0; i < ServerEntryRectangles.Length; i++)
            {
                var mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);
                if (AvailableScreenRectangle.Intersects(mouseRectangle) &&
                    ServerEntryRectangles[i].Intersects(mouseRectangle) &&
                InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released)
                {
                    // Reinitiate and make others bools false
                    ServerSelected = new bool[Servers.Count];

                    ServerSelected[i] = true;
                    if (OnClickedPressed != null)
                        OnClickedPressed(i);

                    if (OnEntryClicked != null)
                        OnEntryClicked();
                }
            }

            #endregion

            #region Keyboard and Gamepad

            if (Array.IndexOf(ServerSelected, true) == -1 && (InputManager.IsOncePressed(Keys.Down) || InputManager.IsOncePressed(Buttons.LeftTrigger, Buttons.DPadDown)))
            {
                ServerSelected[0] = true;
                if (OnClickedPressed != null)
                    OnClickedPressed(0);

                if (OnEntryClicked != null)
                    OnEntryClicked();

                return;
            }

            // Server entry was selected
            if (Array.IndexOf(ServerSelected, true) != -1)
            {
                if (InputManager.IsOncePressed(Keys.Up) || InputManager.IsOncePressed(Buttons.LeftTrigger, Buttons.DPadUp))
                    MenuUp();

                if (InputManager.IsOncePressed(Keys.Down) || InputManager.IsOncePressed(Buttons.LeftTrigger, Buttons.DPadDown))
                    MenuDown();
            }

            #endregion

            #region Mouse Scroll

            if (InputManager.MouseScrollUp)
                MouseScrollUp();

            if (InputManager.MouseScrollDown)
                MouseScrollDown();

            #endregion
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            SpriteBatch.Draw(BackgroundTexture, BackgroundVector, ScreenRectangle, Screen.MainBackgroundColor, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0f);

            SpriteBatch.Draw(BlackTexture, SliderRectangle, Rectangle.Empty, Color.White);

            for (int i = 0; i < Servers.Count; i++)
            {

                #region WhiteFrames

                if (ServerSelected[i])
                {
                    SpriteBatch.Draw(BlackTexture, ServerEntryRectangles[i], Rectangle.Empty, Color.White);

                    SpriteBatch.Draw(WhiteFrameTexture, WhiteFrameTopRectangles[i], Rectangle.Empty, Color.White);
                    SpriteBatch.Draw(WhiteFrameTexture, WhiteFrameBottomRectangles[i], Rectangle.Empty, Color.White);
                    SpriteBatch.Draw(WhiteFrameTexture, WhiteFrameLeftRectangles[i], Rectangle.Empty, Color.White);
                    SpriteBatch.Draw(WhiteFrameTexture, WhiteFrameRightRectangles[i], Rectangle.Empty, Color.White);
                }

                #endregion

                #region Favicon

                // If server have a Favicon, use it.
                if (ServerEntryImageTextures[i] == null && Servers[i].ServerResponse.Info.Favicon != null)
                    ServerEntryImageTextures[i] = Texture2D.FromStream(GraphicsDevice, new MemoryStream(Servers[i].ServerResponse.Info.Favicon));

                if (ServerEntryImageTextures[i] != null)
                    SpriteBatch.Draw(ServerEntryImageTextures[i], ServerEntryImageRectangles[i], new Rectangle(0, 0, ServerEntryImageTextures[i].Width, ServerEntryImageTextures[i].Height), Color.White);

                #endregion

                MainTextRenderer.DrawText(SpriteBatch, Servers[i].Name, ServerNameRectangles[i], Color.White);


                MainTextRenderer.DrawText(SpriteBatch, string.Format("{0}/{1}", Servers[i].ServerResponse.Info.Players.Online, Servers[i].ServerResponse.Info.Players.Max), ServerPlayersVectors[i], Color.White);
                

                if (Servers[i].ServerResponse.Ping == long.MaxValue)
                    MainTextRenderer.DrawText(SpriteBatch, string.Format("Ping: {0}", "None"), ServerPingVectors[i], Color.White);
                else
                    MainTextRenderer.DrawText(SpriteBatch, string.Format("Ping: {0}", Servers[i].ServerResponse.Ping), ServerPingVectors[i], Color.White);
                

                if (Servers[i].ServerResponse.Ping == 0 && Servers[i].ServerResponse.Info.Players.Max == 0)
                    MainTextRenderer.DrawText(SpriteBatch, "Connecting...", ServerAddressVectors[i], Color.White);
                else if (Servers[i].ServerResponse.Ping == long.MaxValue)
                    MainTextRenderer.DrawText(SpriteBatch, "Can't connect to server.", ServerAddressVectors[i], Color.DarkRed);
                else if (Servers[i].ServerResponse.Info.Version.Protocol != CurrentProtocol)
                    MainTextRenderer.DrawText(SpriteBatch, "Unsupported protocol.", ServerAddressVectors[i], Color.DarkRed);
                else
                    MainTextRenderer.DrawText(SpriteBatch, Servers[i].ServerResponse.Info.Description, ServerAddressVectors[i], Color.DarkRed);
            }

            SpriteBatch.End();
        }

        public override void Dispose()
        {
        }

        Rectangle GetServerEntryRectangle(int index)
        {
            return new Rectangle((int)(ScreenRectangle.Center.X - ServerEntrySize.X * 0.5f), (int)(64 + 8 + ServerEntrySize.Y * index), (int)ServerEntrySize.X, (int)ServerEntrySize.Y);
        }
    }
}
