using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MineLib.PGL.Screens;

namespace MineLib.PGL.Components
{
    public class ScreenManagerComponent : DrawableGameComponent
    {
        private List<Screen> Screens { get; set; }
        private List<Screen> ScreensToUpdate { get; set; }
        private List<Screen> ScreensToDraw { get; set; }


        public ScreenManagerComponent(Game game) : base(game)
        {
            Screens = new List<Screen>();
            ScreensToUpdate = new List<Screen>();
            ScreensToDraw = new List<Screen>();
        }


        public void AddScreen(Screen screen)
        {
            Screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            Screens.Remove(screen);

            screen.Dispose();
        }

        public void CloseOtherScreens(Screen currentScreen)
        {
            foreach (var screen in Screens)
                if (screen != currentScreen)
                    RemoveScreen(screen);
        }


        public override void Update(GameTime gameTime)
        {
            ScreensToUpdate.Clear();

            foreach (var screen in Screens)
                ScreensToUpdate.Add(screen);

            foreach (var screen in ScreensToUpdate)
            {
                if (screen.ScreenState == ScreenState.Hidden)// || screen.ScreenState == ScreenState.Background)
                    continue;

                screen.Update(gameTime);

                if (screen.ScreenState == ScreenState.JustNowActive)
                {
                    // Skip one HandleInput, now we won't get a ESC button loop
                    screen.ScreenState = ScreenState.Active;
                    break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreensToDraw.Clear();

            foreach (var screen in Screens)
                ScreensToDraw.Add(screen);

            foreach (var screen in ScreensToDraw)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }
    }
}
