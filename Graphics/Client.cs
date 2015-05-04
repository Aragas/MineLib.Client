using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Network;
using MineLib.Network.IO;
using MineLib.Network.Module;

using MineLib.PCL;
using MineLib.PCL.Graphics.Components;
using MineLib.PCL.Graphics.Map;
using MineLib.PCL.Graphics.Screens;

using PCLStorage;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace MineLib.PCL.Graphics
{
    public sealed class Client : Game
    {
        public static ContentManager ContentManager { get; private set; }

        public Rectangle Windows { get { return _graphics.GraphicsDevice.Viewport.Bounds; } }

        Camera _camera;
        GraphicsDeviceManager _graphics;
        Minecraft _minecraft;
        SpriteBatch _spriteBatch;
        INetworkTCP _tcp;
        WorldVBO _world;
        public static Texture2D Blocks;
        static int Chunks;
        int count = 0;
        FPSCounterComponent FPS;
        
        
        ScreenManagerComponent ScreenManager;

        public event Storage GetStorage;

        public event MineLib.Network.Module.LoadAssembly LoadAssembly;

        public Client(INetworkTCP tcp, bool fullscreen = false)
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = fullscreen;
            ContentManager = Content;
            ContentManager.RootDirectory = "Content";
            _tcp = tcp;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Blocks = ContentManager.Load<Texture2D>("Texture");
            FPS = new FPSCounterComponent(this, _spriteBatch, ContentManager.Load<SpriteFont>("VolterGoldfish"));
            Components.Add(FPS);

            string ip;
            ushort port;
            if (GetStorage != null && GetStorage(this).CheckExistsAsync("settings.txt").Result == ExistenceCheckResult.FileExists)
            {
                using (StreamReader reader = new StreamReader(GetStorage(this).GetFileAsync("settings.txt").Result.OpenAsync(FileAccess.Read).Result))
                {
                    ip = reader.ReadLine();
                    port = ushort.Parse(reader.ReadLine());
                }
            }
            _minecraft = new Minecraft();
            _minecraft.LoadAssembly += LoadAssembly;
            _minecraft.GetStorage += GetStorage;
            _minecraft.Initialize("TestBot", "", ProtocolType.Module, _tcp, false, null);
            _minecraft.BeginConnect("192.168.1.53", 25565, new AsyncCallback(OnConnected), null);
            _camera = new Camera(this, Vector3.Zero, Vector3.Zero, 25f);
        }

        private void OnConnected(IAsyncResult ar)
        {
            _minecraft.BeginConnectToServer(OnJoinedServer, null);
        }

        private void OnJoinedServer(IAsyncResult ar)
        {
            while (_minecraft.ConnectionState != ConnectionState.Joined) { }
            _minecraft.BeginSendClientInfo(null, null);
            _world = new WorldVBO(_graphics.GraphicsDevice);
            while (_minecraft.World.Chunks.Count < 100)
            {
            }
            count = _minecraft.World.Chunks.Count;
            _world.World = _minecraft.World;
            _world.Build();
            lastbuild = DateTime.UtcNow;
        }

        protected override void UnloadContent()
        {
        }

        DateTime lastbuild = DateTime.MinValue;
        KeyboardState oldState;
        protected override void Update(GameTime gameTime)
        {
            _camera.Update(gameTime);

            if (_minecraft != null && _minecraft.World != null)
            {
                Chunks = _minecraft.World.Chunks.Count;
                if (Chunks > count && (DateTime.UtcNow- lastbuild > new TimeSpan(0, 0, 20)))
                {
                    _world.World = _minecraft.World;
                    _world.Build();
                    count = _minecraft.World.Chunks.Count;
                }
            }
            var state = Keyboard.GetState();
            if (((!state.IsKeyDown(Keys.J) || oldState.IsKeyDown(Keys.J)) && (!state.IsKeyDown(Keys.J) || !oldState.IsKeyDown(Keys.J))) && ((!state.IsKeyDown(Keys.J) && oldState.IsKeyDown(Keys.J)) && (_minecraft != null)))
            {
                _world.World = _minecraft.World;
                _world.Build();
            }
            oldState = state;
            if (_world != null)
            {
                _world.Update();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (_world != null)
                _world.Draw(_camera);
            
            base.Draw(gameTime);
        }
}
