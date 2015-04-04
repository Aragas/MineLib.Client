using System;
using System.IO;
using System.Threading;

using Ionic.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Client.Graphics.Components;
using MineLib.Client.Graphics.Data;
using MineLib.Client.Graphics.Map;

using MineLib.Network;

namespace MineLib.Client.Graphics
{
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Client : Game
    {
		public static MinecraftTexturesStorage MinecraftTexturesStorage;
        public static Texture2D Blocks;
		
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Minecraft _minecraft;

		Camera _camera;
		WorldVBO _world;

		FPSCounterComponent _fps;

        public Client()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
			_graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = true;
			_graphics.ApplyChanges();


			_camera = new Camera(this, Vector3.Zero, new Vector3(0), 25f);
            _camera.Initialize();

            Thread thread = new Thread(() =>
            {
	            _minecraft = new Minecraft().Initialize("TestBot", "", NetworkMode.Module, true) as Minecraft;

	            if (_minecraft != null)
                    _minecraft.BeginConnect("127.0.0.1", 25565, OnConnected, null);
            });
				thread.Start();

            base.Initialize();
        }

        private void OnConnected(IAsyncResult ar)
        {
            _minecraft.BeginConnectToServer(OnJoinedServer, null);
        }

        private void OnJoinedServer(IAsyncResult ar)
        {
            while (_minecraft.ConnectionState != ConnectionState.JoinedServer) { Thread.Sleep(250); }

            _minecraft.BeginSendClientInfo(null, null);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

			if (File.Exists(Path.Combine(Content.RootDirectory, "Texture.xnb")))
				Blocks = Content.Load<Texture2D>("Texture");

            #region Load resources from minecraft.jar
            if (File.Exists(Path.Combine(Content.RootDirectory, "minecraft.jar")))
            {
                var minecraftFiles = new ZipFile(Path.Combine(Content.RootDirectory, "minecraft.jar"));
                MinecraftTexturesStorage = new MinecraftTexturesStorage(_graphics.GraphicsDevice, minecraftFiles);

                MinecraftTexturesStorage.ParseGUITextures();
            }
            #endregion

            _fps = new FPSCounterComponent(this, _spriteBatch, Content.Load<SpriteFont>("VolterGoldfish"));
            Components.Add(_fps);
        }

        protected override void UnloadContent()
        {
        }

		KeyboardState oldState;
		protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

			var newState = Keyboard.GetState();
			if (newState.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
			{
				// the player just pressed down
			}
			else if (newState.IsKeyDown(Keys.Down) && oldState.IsKeyDown(Keys.Down))
			{
				// the player is holding the key down
			}
			else if (!newState.IsKeyDown(Keys.K) && oldState.IsKeyDown(Keys.K))
			{
				// the player was holding the key down, but has just let it go
				if (_minecraft != null)
					if (_world == null)
						_world = new WorldVBO(_graphics.GraphicsDevice, _minecraft.World.Chunks);
			}

			if (!newState.IsKeyDown(Keys.J) && oldState.IsKeyDown(Keys.J))
			{
				if (_minecraft != null)
				{
					_world = new WorldVBO(_graphics.GraphicsDevice, _minecraft.World.Chunks);
					//_camera.Position = _minecraft.Player.Position.Vector3.ToXNAVector3();
                    //_camera.MoveTo(_minecraft.Player.Position.Vector3.ToXNAVector3(), new Vector3(0));
				}
			}
			oldState = newState;


	        _camera.Update(gameTime);
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);


	        if (_world != null)
                _world.Draw(_camera);
                //_world.Draw(_camera, ray);


	        base.Draw(gameTime);
        }
    }
}
