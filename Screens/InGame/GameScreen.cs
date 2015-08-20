using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.Core.Data;

using MineLib.PGL.Screens.InMenu;

namespace MineLib.PGL.Screens.InGame
{
    public sealed class GameScreen : Screen
    {
        private readonly List<PlayerClient> _clients = new List<PlayerClient>();

        public GameScreen(Client game, Server entry) : base(game, "GameScreen")
        {
            Game.IsMouseVisible = false;

            _clients.Add(new PlayerClient(Game, this, PlayerIndex.PlayerOne, entry));

            //_clients.Add(new PlayerClient<T>(Game, this, PlayerIndex.PlayerOneHalf, entry));
            //_clients.Add(new PlayerClient<T>(Game, this, PlayerIndex.PlayerTwoHalf, entry));

            //_clients.Add(new PlayerClient<T>(Game, this, PlayerIndex.PlayerOneQuad, entry));
            //_clients.Add(new PlayerClient<T>(Game, this, PlayerIndex.PlayerTwoQuad, entry));
            //_clients.Add(new PlayerClient<T>(Game, this, PlayerIndex.PlayerThreeQuad, entry));
            //_clients.Add(new PlayerClient<T>(Game, this, PlayerIndex.PlayerFourQuad, entry));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(InputManager.IsOncePressed(Keys.F1))
                AddScreenAndCloseThis(new MainMenuScreen(Game));

            foreach (var playerClient in _clients)
                playerClient.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (var playerClient in _clients)
                playerClient.DrawRaw(gameTime);

            //SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            //foreach (var playerClient in _clients)
            //{
            //    playerClient.Draw(gameTime);
            //
            //    SpriteBatch.Draw(playerClient.ColorRT, playerClient.PlayerRectangle, Color.White);
            //    SpriteBatch.Draw(playerClient.DepthRT, playerClient.PlayerRectangle, Color.White);
            //}
            //SpriteBatch.End();
        }

        public override void Dispose()
        {
            if (_clients != null)
            {
                foreach (var playerClient in _clients)
                    playerClient.Dispose();

                _clients.Clear();
            }
        }
    }
}
