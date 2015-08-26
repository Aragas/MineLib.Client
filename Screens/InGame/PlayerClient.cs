using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core;
using MineLib.Core.Data;
using MineLib.Core.Wrappers;

using MineLib.PGL.Components;
using MineLib.PGL.Data;
using MineLib.PGL.Screens.InGame.Light;

using Vector3 = Microsoft.Xna.Framework.Vector3;

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
        RenderTarget2D LightRT { get; set; }
        Rectangle PlayerRectangle { get; set; }

        Vector2 HalfPixel { get; set; }

        Effect Clear { get; set; }
        Effect Combine { get; set; }
        QuadRenderer QuadRenderer { get; set; }


        bool HandleInput { get; set; }
        PlayerIndex PlayerIndex { get; set; }

        CameraComponent Camera { get; set; }
        WorldRendererComponent WorldRenderer { get; set; }
        Minecraft Minecraft { get; set; }

        SpriteBatch SpriteBatch { get; set; }
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
            QuadRenderer = new QuadRenderer(Game);
    

            Minecraft = new Minecraft(Game).Initialize(PlayerIndex.ToString(), "", ProtocolType.Module, false, null) as Minecraft;
            Minecraft.ChoseModule += modules =>
            {
                foreach (var protocol in modules)
                    if (Game.DefaultModule.FileName == protocol.FileName)
                        return protocol;
                
                return null;
            };

            var server = FileSystemWrapper.LoadSettings<Server>(DefaulConnectSettings, new Server { Address = new Address { IP = "95.24.192.107", Port = 25565 } });
            ThreadWrapper.StartThread(async () =>
            {
                await Minecraft.ConnectAsync(server.Address.IP, server.Address.Port);
                Minecraft.ConnectToServer(server.Address.IP, server.Address.Port, Minecraft.ClientUsername, 47);
            }, false, "MinecraftStartThread");

            Camera = new CameraComponent(Game, Vector3.Zero, Vector3.Zero, 25f);
            WorldRenderer = new WorldRendererComponent(Game, Camera, Minecraft);

            Camera.World = Minecraft.World;


            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(0, 100, 0), 1000, Color.White, 10f));
            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(1000, 100, 1000), 1000, Color.White, 10f));
            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(-1000, 100, -1000), 1000, Color.White, 10f));

            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(-1000, 100, 0), 1000, Color.White, 10f));
            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(0, 100, -1000), 1000, Color.White, 10f));

            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(1000, 100, 0), 1000, Color.White, 10f));
            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(0, 100, 1000), 1000, Color.White, 10f));

            WorldRenderer.Lights.Add(new PointLight(Game, new Vector3(1000, 100, 0), 1000, Color.White, 10f));
        }

        private void FillPlayerRenderTarget()
        {
            var width = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            var height = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            var fullWidth =     (int)(width * 1.0f);
            var fullHeight =    (int)(height * 1.0f);
            var halfWidth =     (int)(width * 0.5f);
            var halfHeight =    (int)(height * 0.5f);

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

            LightRT = new RenderTarget2D(GraphicsDevice, PlayerRectangle.Width, PlayerRectangle.Height,
                false, GraphicsDevice.Adapter.CurrentDisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

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

            WorldRenderer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var preDepthStencilState = GraphicsDevice.DepthStencilState;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            WorldRenderer.Draw(gameTime);

            GraphicsDevice.DepthStencilState = preDepthStencilState;
        }

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
                light.DrawLight(this, Camera, QuadRenderer, HalfPixel);
            ResolveLightTarget();


            CombineRT();


            GraphicsDevice.DepthStencilState = preDepthStencilState;
            GraphicsDevice.BlendState = preBlendState;
        }

        public void DrawDebug(GameTime gameTime)
        {
            Draw(gameTime);

            var screenWidth = (int)(ScreenRectangle.Width * 1.0f);
            var screenHeight = (int)(ScreenRectangle.Height * 1.0f);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            SpriteBatch.Draw(ColorRT, new Rectangle(0, 0, screenWidth / 2, screenHeight / 2), Color.White);
            SpriteBatch.Draw(NormalRT, new Rectangle(screenWidth / 2, 0, screenWidth / 2, screenHeight / 2), Color.White);
            SpriteBatch.Draw(DepthRT, new Rectangle(0, screenHeight / 2, screenWidth / 2, screenHeight / 2), Color.White);
            SpriteBatch.Draw(LightRT, new Rectangle(screenWidth / 2, screenHeight / 2, screenWidth / 2, screenHeight / 2), Color.White);

            SpriteBatch.End();
        }


        private void SetGBuffer()
        {
            GraphicsDevice.SetRenderTargets(ColorRT, NormalRT, DepthRT);

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            QuadRenderer.RenderFullScreenQuad(Clear);
        }

        private void ClearGBuffer()
        {
            // Clear the light target to transparent black (with alpha = 0, because the alpha channel is used for the lightmap).
            GraphicsDevice.SetRenderTargets(ColorRT, NormalRT, DepthRT);
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        private void ResolveGBuffer()
        {
            GraphicsDevice.SetRenderTarget(null);
        }


        private void SetLightTarget()
        {
            GraphicsDevice.SetRenderTarget(LightRT);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true, DepthBufferWriteEnable = false };
        }

        private void ClearLightTarget()
        {
            // Clear the light target to transparent black (with alpha = 0, because the alpha channel is used for the lightmap).
            GraphicsDevice.SetRenderTarget(LightRT);
            GraphicsDevice.Clear(Color.Transparent);
        }

        private void ResolveLightTarget()
        {
            GraphicsDevice.SetRenderTarget(null);
        }


        private void CombineRT()
        {
            Combine.Parameters["colorMap"].SetValue(ColorRT);
            Combine.Parameters["normalMap"].SetValue(NormalRT);
            Combine.Parameters["lightMap"].SetValue(LightRT);

            Combine.Parameters["halfPixel"].SetValue(HalfPixel);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            QuadRenderer.RenderFullScreenQuad(Combine);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


        public override void Dispose()
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();

            if (WorldRenderer != null)
                WorldRenderer.Dispose();

            if (Camera != null)
                Camera.Dispose();

            if (Minecraft != null)
                Minecraft.Dispose();
        }
    }
}
