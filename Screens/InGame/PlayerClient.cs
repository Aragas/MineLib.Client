using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core;
using MineLib.Core.Data;

using Aragas.Core.Wrappers;

using MineLib.PGL.Components;
using MineLib.PGL.Data;
using MineLib.PGL.Extensions;
using MineLib.PGL.Screens.InGame.Light;

namespace MineLib.PGL.Screens.InGame
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

    public sealed class PlayerClient : ScreenGUIComponent
    {
        public RenderTarget2D ColorRT { get; private set; }
        public RenderTarget2D NormalRT { get; private set; }
        public RenderTarget2D DepthRT { get; private set; }
        RenderTarget2D LocalLightRT { get; set; }
        RenderTarget2D SkyLightRT { get; set; }
        Rectangle PlayerRectangle { get; set; }

        Vector2 HalfPixel { get; }

        Effect Clear { get; }
        Effect Combine { get; }
        QuadRenderer QuadRenderer { get; }


        bool HandleInput { get; set; }
        PlayerIndex PlayerIndex { get; }

        CameraComponent Camera { get; }
        WorldRendererComponent WorldRenderer { get; }
        Minecraft Minecraft { get; }

        SpriteBatch SpriteBatch { get; }
        readonly CancellationTokenSource _cancellationToken;

        const string DefaulConnectSettings = "DefaultConnect.json";

        public PlayerClient(Client game, Screen screen, PlayerIndex player, Server entry, Minecraft minecraft = null) : base(game, screen)
        {
            HandleInput = true;

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Minecraft = minecraft;
            PlayerIndex = player;

            // Calculate half of a pixel to correctly align texture and screen space coordinates.
            HalfPixel = new Vector2(0.5f / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, 0.5f / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);


            FillPlayerRenderTarget();

            _cancellationToken = new CancellationTokenSource();


            Clear = Game.Content.Load<Effect>("Effects/AlbedoDepth");
            Combine = Game.Content.Load<Effect>("Effects/Combine");
            Combine.Parameters.TrySet("SunColor", Color.White.ToVector4());
            SpecularTexture = Game.Content.Load<Texture2D>("terrain_s");
            QuadRenderer = new QuadRenderer(Game);


            Minecraft = new Minecraft(PlayerIndex.ToString(), "", ProtocolType.Module, false, null);
            Minecraft.ChoseModule += (sender, args) =>
            {
                foreach (var protocol in args.Modules)
                    if (Game.DefaultModule.FileName == protocol.FileName)
                        args.SetModule(protocol);
            };

            var server = new Server { Address = new Address { IP = "127.0.0.1", Port = 25565 } };
            FileSystemWrapper.LoadSettings(DefaulConnectSettings, server);
            //var thread = ThreadWrapper.CreateThread(() =>
            //{
            //    Minecraft.Connect(server.Address.IP, server.Address.Port);
            //    Minecraft.ConnectToServer(server.Address.IP, server.Address.Port, Minecraft.ClientUsername, 47);
            //});
            //thread.Name = "MinecraftStartThread";
            //thread.IsBackground = false;
            //thread.Start();

            Minecraft.Connect(server.Address.IP, server.Address.Port);
            Minecraft.ConnectToServer(server.Address.IP, server.Address.Port, Minecraft.ClientUsername ?? Minecraft.ClientLogin, 47);

            Camera = new CameraComponent(Game, Vector3.Zero, Vector3.Zero, 25f);
            WorldRenderer = new WorldRendererComponent(Game, Camera, Minecraft);

            Camera.World = Minecraft.World;


            WorldRenderer.Lights.Add(new PointLight(Game, lightPosition, 1000, Color.White, 2f));
        }

        private Vector3 lightPosition = new Vector3(160, 160, 550);

        private void FillPlayerRenderTarget()
        {
            var fullWidth =     (int)(ScreenRectangle.Width * 1.0f);
            var fullHeight =    (int)(ScreenRectangle.Height * 1.0f);
            var halfWidth =     (int)(ScreenRectangle.Width * 0.5f);
            var halfHeight =    (int)(ScreenRectangle.Height * 0.5f);

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

            NormalRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, GraphicsDevice.Adapter.CurrentDisplayMode.Format, DepthFormat.None);

            DepthRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, GraphicsDevice.Adapter.CurrentDisplayMode.Format, DepthFormat.None);

            LocalLightRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, SurfaceFormat.HalfVector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            SkyLightRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, SurfaceFormat.HalfVector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

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

            if (InputManager.IsCurrentKeyPressed(Keys.NumPad8))
                lightPosition.Z += 10f;

            if (InputManager.IsCurrentKeyPressed(Keys.NumPad2))
                lightPosition.Z -= 10f;

            if (InputManager.IsCurrentKeyPressed(Keys.NumPad4))
                lightPosition.X += 10f;

            if (InputManager.IsCurrentKeyPressed(Keys.NumPad6))
                lightPosition.X -= 10f;

            if (InputManager.IsCurrentKeyPressed(Keys.OemPlus))
                lightPosition.Y += 10f;

            if (InputManager.IsCurrentKeyPressed(Keys.OemMinus))
                lightPosition.Y -= 10f;

            if (HandleInput)
                Camera.Update(gameTime);

            if (Minecraft?.World != null)
                Combine.Parameters.TrySet("TimeOfDay", Minecraft.World.RealTime.Hours);
            
            WorldRenderer.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            var preDepthStencilState = GraphicsDevice.DepthStencilState;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            WorldRenderer.Draw(gameTime);

            GraphicsDevice.DepthStencilState = preDepthStencilState;
        }

        public Texture2D SpecularTexture;
        public void DrawDeferred(GameTime gameTime)
        {
            var preDepthStencilState = GraphicsDevice.DepthStencilState;
            var preBlendState = GraphicsDevice.BlendState;


            ClearGBuffer();
            SetGBuffer();
            
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            WorldRenderer.Draw(gameTime);

            ResolveGBuffer();


            ClearLightTarget();
            SetLightTarget();
            foreach (var light in WorldRenderer.Lights)
            {
                light.Position = lightPosition;
                light.DrawLight(this, Camera, QuadRenderer, HalfPixel);
            }
            ResolveLightTarget();


            CombineRT();


            GraphicsDevice.DepthStencilState = preDepthStencilState;
            GraphicsDevice.BlendState = preBlendState;
        }

        public void DrawDebug(GameTime gameTime)
        {
            DrawDeferred(gameTime);

            var screenWidth = (int)(ScreenRectangle.Width * 1.0f);
            var screenHeight = (int)(ScreenRectangle.Height * 1.0f);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            SpriteBatch.Draw(ColorRT, new Rectangle(0, 0, screenWidth / 2, screenHeight / 2), Color.White);
            SpriteBatch.Draw(NormalRT, new Rectangle(screenWidth / 2, 0, screenWidth / 2, screenHeight / 2), Color.White);
            SpriteBatch.Draw(DepthRT, new Rectangle(0, screenHeight / 2, screenWidth / 2, screenHeight / 2), Color.White);
            SpriteBatch.Draw(LocalLightRT, new Rectangle(screenWidth / 2, screenHeight / 2, screenWidth / 2, screenHeight / 2), Color.White);

            SpriteBatch.End();
        }


        private void ClearGBuffer()
        {
            GraphicsDevice.SetRenderTargets(ColorRT, NormalRT, DepthRT, SkyLightRT);
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        private void SetGBuffer()
        {
            GraphicsDevice.SetRenderTargets(ColorRT, NormalRT, DepthRT, SkyLightRT);

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            QuadRenderer.RenderFullScreenQuad(Clear);
        }

        private void ResolveGBuffer()
        {
            GraphicsDevice.SetRenderTarget(null);
        }


        private void SetLightTarget()
        {
            GraphicsDevice.SetRenderTarget(LocalLightRT);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true, DepthBufferWriteEnable = false };
        }

        private void ClearLightTarget()
        {
            // Clear the light target to transparent black (with alpha = 0, because the alpha channel is used for the lightmap).
            GraphicsDevice.SetRenderTarget(LocalLightRT);
            GraphicsDevice.Clear(Color.Transparent);
        }

        private void ResolveLightTarget()
        {
            GraphicsDevice.SetRenderTarget(null);
        }


        private void CombineRT()
        {
            Combine.Parameters.TrySet("ColorMap", ColorRT);
            Combine.Parameters.TrySet("NormalMap", NormalRT);
            Combine.Parameters.TrySet("LocalLightMap", LocalLightRT);
            Combine.Parameters.TrySet("SkyLightMap", SkyLightRT);
            
            Combine.Parameters.TrySet("HalfPixel", HalfPixel);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            QuadRenderer.RenderFullScreenQuad(Combine);
        }


        public override void Dispose()
        {
            _cancellationToken?.Cancel();

            Camera?.Dispose();

            WorldRenderer?.Dispose();

            Minecraft?.Dispose();
        }
    }
}
