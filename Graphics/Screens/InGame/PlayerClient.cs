using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core;
using MineLib.Core.Data;
using MineLib.Core.Wrappers;

using MineLib.PCL.Graphics.Components;
using MineLib.PCL.Graphics.Components.NotUsed;

namespace MineLib.PCL.Graphics.Screens.InGame
{
    public enum PlayerIndex
    {
        PlayerOne,

        PlayerOneHalf,
        PlayerTwoHalf,

        PlayerOneQuad,
        PlayerTwoQuad,
        PlayerThreeQuad,
        PlayerFourQuad
    }

    public sealed class PlayerClient<T> : MineLibGUIGameComponent where T : struct, IVertexType
    {
        public RenderTarget2D ColorRT { get; private set; }
        public RenderTarget2D DepthRT { get; private set; }
        public Rectangle PlayerRectangle { get; private set; }
        
        private bool HandleInput { get; set; }
        private PlayerIndex PlayerIndex { get; set; }

        private CameraComponentTC<T> Camera { get; set; }
        private WorldComponent<T> World { get; set; }
        private Minecraft<T> Minecraft { get; set; }

        private SpriteBatch SpriteBatch { get; set; }
        private readonly CancellationTokenSource _cancellationToken;

        private const string DefaulConnectSettings = "DefaultConnect.json";

        public PlayerClient(Client game, Screen screen, PlayerIndex player, Server entry, Minecraft<T> minecraft = null) : base(game, screen)
        {
            HandleInput = true;

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Minecraft = minecraft;
            PlayerIndex = player;

            FillPlayerRenderTarget();

            _cancellationToken = new CancellationTokenSource();


            Clear = Game.Content.Load<Effect>("Effects/Clear");
            FsQ = new FullscreenQuad(GraphicsDevice);


            Minecraft = new Minecraft<T>(Game).Initialize(PlayerIndex.ToString(), "", ProtocolType.Module, false, null) as Minecraft<T>;
            Minecraft.ChoseModule += modules =>
            {
                foreach (var protocol in modules)
                    if (Game.DefaultModule.FileName == protocol.FileName)
                        return protocol;
                
                return null;
            };

            var server = FileSystemWrapper.LoadSettings<Server>(DefaulConnectSettings, new Server { Address = new Address { IP = "192.168.1.53", Port = 25565 } });
            ThreadWrapper.StartThread(async () =>
            {
                await Minecraft.ConnectAsync(server.Address.IP, server.Address.Port);
                Minecraft.ConnectToServer(server.Address.IP, server.Address.Port, Minecraft.ClientUsername, 47);
            }, false, "MinecraftStartThread");

            Camera = new CameraComponentTC<T>(Game, Microsoft.Xna.Framework.Vector3.Zero, Microsoft.Xna.Framework.Vector3.Zero, 25f);
            World = new WorldComponent<T>(Game, Camera, Minecraft);

            Camera.World = Minecraft.World;
        }

        private void FillPlayerRenderTarget()
        {
            var fullWidth =     (int)(ScreenRectangle.Width     * 1.0f);
            var fullHeight =    (int)(ScreenRectangle.Height    * 1.0f);
            var halfWidth =     (int)(ScreenRectangle.Width     * 0.5f);
            var halfHeight =    (int)(ScreenRectangle.Height    * 0.5f);

            switch (PlayerIndex)
            {
                case PlayerIndex.PlayerOne:
                    PlayerRectangle = new Rectangle(0, 0, fullWidth, fullHeight);
                    break;


                    case PlayerIndex.PlayerOneHalf:
                    PlayerRectangle = new Rectangle(0, 0, fullWidth, halfHeight);
                    break;

                    case PlayerIndex.PlayerTwoHalf:
                    PlayerRectangle = new Rectangle(0, halfHeight, fullWidth, halfHeight);
                    break;


                    case PlayerIndex.PlayerOneQuad:
                    PlayerRectangle = new Rectangle(0, 0, halfWidth, halfHeight);
                    break;

                    case PlayerIndex.PlayerTwoQuad:
                    PlayerRectangle = new Rectangle(halfWidth, 0, halfWidth, halfHeight);
                    break;

                    case PlayerIndex.PlayerThreeQuad:
                    PlayerRectangle = new Rectangle(0, halfHeight, halfWidth, halfHeight);
                    break;

                    case PlayerIndex.PlayerFourQuad:
                    PlayerRectangle = new Rectangle(halfWidth, halfHeight, halfWidth, halfHeight);
                    break;
            }

            ColorRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, GraphicsDevice.Adapter.CurrentDisplayMode.Format, DepthFormat.Depth24);

            DepthRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, GraphicsDevice.Adapter.CurrentDisplayMode.Format, DepthFormat.None);

            //DepthRT = new RenderTarget2D(GraphicsDevice, fullWidth, fullHeight,
            //    false, SurfaceFormat.Vector2, DepthFormat.None);
        }

        public override void Update(GameTime gameTime)
        {
            if (PlayerIndex == PlayerIndex.PlayerOneHalf && InputManager.IsOncePressed(Keys.D2))
                HandleInput = !HandleInput;
            if (PlayerIndex == PlayerIndex.PlayerTwoHalf && InputManager.IsOncePressed(Keys.D3))
                HandleInput = !HandleInput;
            

            if (PlayerIndex == PlayerIndex.PlayerOneQuad && InputManager.IsOncePressed(Keys.D2))
                HandleInput = !HandleInput;
            if (PlayerIndex == PlayerIndex.PlayerTwoQuad && InputManager.IsOncePressed(Keys.D3))
                HandleInput = !HandleInput;
            if (PlayerIndex == PlayerIndex.PlayerThreeQuad && InputManager.IsOncePressed(Keys.D4))
                HandleInput = !HandleInput;
            if (PlayerIndex == PlayerIndex.PlayerFourQuad && InputManager.IsOncePressed(Keys.D5))
                HandleInput = !HandleInput;
            

            if(HandleInput)
                Camera.Update(gameTime);

            World.Update(gameTime);
        }

        Effect Clear { get; set; }
        FullscreenQuad FsQ { get; set; }

        private void ClearGBuffer()
        {
            GraphicsDevice.Clear(Color.Transparent);
            foreach (var pass in Clear.CurrentTechnique.Passes)
            {
                pass.Apply();

                FsQ.Draw(GraphicsDevice);
            }
        }

        private void SetGBuffer()
        {
            GraphicsDevice.SetRenderTargets(ColorRT, DepthRT);
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        private void ResolveGBuffer()
        {
            GraphicsDevice.SetRenderTargets(null);
        }

        public override void Draw(GameTime gameTime)
        {
            var preDepthStencilState = GraphicsDevice.DepthStencilState;

            ClearGBuffer();
            SetGBuffer();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            World.Draw(gameTime);
            ResolveGBuffer();

            GraphicsDevice.DepthStencilState = preDepthStencilState;
        }

        public override void Dispose()
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();

            if (World != null)
                World.Dispose();

            if (Camera != null)
                Camera.Dispose();

            if (Minecraft != null)
                Minecraft.Dispose();
        }
    }
}
