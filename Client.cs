using System;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Aragas.Core.Wrappers;

using MineLib.Core.Loader;

using MineLib.PGL.Components;
using MineLib.PGL.Screens.InMenu;

using PCLStorage;

namespace MineLib.PGL
{
    public sealed class Client : Game
    {
        public static Point DefaultResolution => new Point(800, 600);

        public static Texture2D Blocks { get; private set; }

        public ScreenManagerComponent ScreenManager { get; private set; }

        public TextureStorageComponent TextureStorage { get; private set; }


        private GraphicsDeviceManager Graphics { get; }


        public ProtocolAssembly DefaultModule { get; private set; }
        private const string DefaultModuleSettings = "DefaultModule.json";

        public Client(Action<Game> platformCode, bool fullscreen = false)
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = fullscreen;
            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            platformCode?.Invoke(this);
        }

        protected override void LoadContent()
        {
            Blocks = Content.Load<Texture2D>("Texture");
            //Blocks = Content.Load<Texture2D>("Effects\\terrain");

            var contentFolder = FileSystemWrapper.ContentFolder;
            if (contentFolder != null && contentFolder.CheckExistsAsync("texturepack.zip").Result == ExistenceCheckResult.FileExists)
                using (var reader = new StreamReader(contentFolder.GetFileAsync("texturepack.zip").Result.OpenAsync(FileAccess.Read).Result))
                {
                    var minecraftFiles = new ZipFile(reader.BaseStream);
                    TextureStorage = new TextureStorageComponent(this, minecraftFiles);
                    TextureStorage.ParseGUITextures();
                }

            else if (contentFolder != null && contentFolder.CheckExistsAsync("minecraft.jar").Result == ExistenceCheckResult.FileExists)
                using (var reader = new StreamReader(contentFolder.GetFileAsync("minecraft.jar").Result.OpenAsync(FileAccess.Read).Result))
                {
                    var minecraftFiles = new ZipFile(reader.BaseStream);
                    TextureStorage = new TextureStorageComponent(this, minecraftFiles);
                    TextureStorage.ParseGUITextures();
                }


            var list = FileSystemWrapper.AssemblyFolder.GetFilesAsync().Result;
            DefaultModule = list.Count > 0 ? new ProtocolAssembly(list[0].Path) : default(ProtocolAssembly);
            FileSystemWrapper.LoadSettings(DefaultModuleSettings, DefaultModule);

            ScreenManager = new ScreenManagerComponent(this);
            Components.Add(ScreenManager);
            ScreenManager.AddScreen(new MainMenuScreen(this));

#if DEBUG
            Components.Add(new DebugComponent(this));
#endif
        }

        public void OnResize(object sender, EventArgs e)
        {
            if (Graphics.GraphicsDevice.Viewport.Width < DefaultResolution.X || Graphics.GraphicsDevice.Viewport.Height < DefaultResolution.Y)
            {
                Resize(DefaultResolution);
                return;
            }

            ScreenManager.OnResize();
        }
        public void Resize(Point size)
        {
            if (size.X < DefaultResolution.X || size.Y < DefaultResolution.Y)
                return;
            
            Graphics.PreferredBackBufferWidth = size.X;
            Graphics.PreferredBackBufferHeight = size.Y;

            Graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);

            if (InputManager.IsOncePressed(Keys.L))
            {
                Graphics.SynchronizeWithVerticalRetrace = !Graphics.SynchronizeWithVerticalRetrace;
                Graphics.ApplyChanges();
                IsFixedTimeStep = !IsFixedTimeStep;
            }

            if (InputManager.IsOncePressed(Keys.M))
                TargetElapsedTime = new TimeSpan((long) (1000f / 144f * TimeSpan.TicksPerMillisecond));
            
            if (InputManager.IsOncePressed(Keys.N))
                TargetElapsedTime = new TimeSpan((long) (1000f / 60f * TimeSpan.TicksPerMillisecond));
            
            if (InputManager.IsOncePressed(Keys.B))
                TargetElapsedTime = new TimeSpan((long) (1000f / 30f * TimeSpan.TicksPerMillisecond));

            if (InputManager.IsOncePressed(Keys.Y))
                Resize(new Point(800, 600));
            
            if (InputManager.IsOncePressed(Keys.U))
                Resize(new Point(1440, 900));

            if (InputManager.IsOncePressed(Keys.I))
                Resize(new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height));

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
