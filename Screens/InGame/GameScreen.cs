using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Data;

using MineLib.PGL.Screens.InMenu;

namespace MineLib.PGL.Screens.InGame
{
    public sealed class GameScreen : Screen
    {
        private GameOptionScreen OptionScreen { get; set; }

        private readonly List<PlayerClient> _clients = new List<PlayerClient>();
        
        public GameScreen(Client game, Server entry) : base(game)
        {
            Game.IsMouseVisible = false;
            
            //_clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerOne, entry));

            _clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerOneHalf, entry));
            _clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerTwoHalf, entry));

            //_clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerOneQuad, entry));
            //_clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerTwoQuad, entry));
            //_clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerThreeQuad, entry));
            //_clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerFourQuad, entry));
        }

        private bool _isSet;
        private void Init()
        {
            OptionScreen = new GameOptionScreen(Game, this);
            OptionScreen.ToHidden();
            AddScreen(OptionScreen);
        }

        private bool _drawDebug;

        public override void OnResize()
        {
            base.OnResize();

            //foreach (var client in _clients)
            //    client.OnResize();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            base.Update(gameTime);

            if (!_isSet)
            {
                Init();
                _isSet = true;
            }

            if (InputManager.IsOncePressed(Keys.Escape))
                if (IsActive)
                {
                    Game.IsMouseVisible = true;

                    OptionScreen.ToActive();
                    ToBackground();
                }
            

            if (InputManager.IsOncePressed(Keys.F1))
                AddScreenAndCloseThis(new MainMenuScreen(Game));

            if (InputManager.IsOncePressed(Keys.F5))
                _drawDebug = !_drawDebug;

            foreach (var playerClient in _clients)
                playerClient.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (_drawDebug)
                foreach (var playerClient in _clients)
                    playerClient.DrawDebug(gameTime);
            else
                foreach (var playerClient in _clients)
                    playerClient.DrawDeferred(gameTime);


            //foreach (var playerClient in _clients)
            //    playerClient.Draw(gameTime);

            base.Draw(gameTime);
        }


        public override void Dispose()
        {
            base.Dispose();

            if (_clients != null)
            {
                foreach (var playerClient in _clients)
                    playerClient.Dispose();

                _clients.Clear();
            }
        }
    }
}
