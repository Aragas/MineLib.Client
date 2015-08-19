using System;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Loader;
using MineLib.Core.Wrappers;

using MineLib.PGL.Components;
using MineLib.PGL.Screens.InMenu;

using PCLStorage;

namespace MineLib.PGL
{
    public sealed class Client : Game
    {
        public int PreferredBackBufferWidth { get { return Graphics.PreferredBackBufferWidth; } set { Graphics.PreferredBackBufferWidth = value; Graphics.ApplyChanges(); } }
        public int PreferredBackBufferHeight { get { return Graphics.PreferredBackBufferHeight; } set { Graphics.PreferredBackBufferHeight = value; Graphics.ApplyChanges(); } }

        public static Texture2D Blocks { get; private set; }

        public ScreenManagerComponent ScreenManager { get; private set; }

        public TextureStorageComponent TextureStorage { get; private set; }


        private GraphicsDeviceManager Graphics { get; set; }


        public ProtocolAssembly DefaultModule { get; private set; }
        private const string DefaultModuleSettings = "DefaultModule.json";

        public Client(Action<Game> platformCode, bool fullscreen = false)
        {
            Graphics = new GraphicsDeviceManager(this);
            //Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.IsFullScreen = fullscreen;
            Graphics.ApplyChanges();

            //IsFixedTimeStep = false;
            //TargetElapsedTime = new TimeSpan((long)(1000f / 60f * TimeSpan.TicksPerMillisecond));

            Content.RootDirectory = "Content";

            if(platformCode != null)
                platformCode(this);
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


            var list = FileSystemWrapper.ProtocolsFolder.GetFilesAsync().Result;
            DefaultModule = FileSystemWrapper.LoadSettings<ProtocolAssembly>(DefaultModuleSettings, list.Count > 0 ? new ProtocolAssembly(list[0].Path) : null);

            // Android has some strange behaviour with custom shaders. The best way I found to specify really fast shader stuff
            AddComponent<VertexPositionTexture>();
        }

        private void AddComponent<T>() where T : struct, IVertexType
        {
            ScreenManager = new ScreenManagerComponent(this);
            Components.Add(ScreenManager);
            ScreenManager.AddScreen(new MainMenuScreen<T>(this));

#if DEBUG
            Components.Add(new DebugComponent<T>(this));
#endif
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

            if (InputManager.IsOncePressed(Keys.N))
                TargetElapsedTime = new TimeSpan((long)(1000f / 144f * TimeSpan.TicksPerMillisecond));
            
            if (InputManager.IsOncePressed(Keys.N))
                TargetElapsedTime = new TimeSpan((long)(1000f / 60f * TimeSpan.TicksPerMillisecond));
            
            if (InputManager.IsOncePressed(Keys.B))
                TargetElapsedTime = new TimeSpan((long)(1000f / 30f * TimeSpan.TicksPerMillisecond));
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
