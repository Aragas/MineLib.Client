using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MineLib.PGL.Screens;

namespace MineLib.PGL.Components
{
    public class ScreenManagerComponent : DrawableGameComponent
    {
        private List<Screen> Screens { get; } = new List<Screen>();
        private List<Screen> ScreensToUpdate { get; } = new List<Screen>();
        private List<Screen> ScreensToDraw { get; } = new List<Screen>();

        private bool NeedsResize { get; set; }

        public ScreenManagerComponent(Game game) : base(game) { }


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

        public void OnResize()
        {
            NeedsResize = true;
        }

        public override void Update(GameTime gameTime)
        {
            ScreensToUpdate.Clear();

            foreach (var screen in Screens)
                ScreensToUpdate.Add(screen);

            if (NeedsResize)
            {
                foreach (var screen in Screens)
                    screen.OnResize();

                NeedsResize = false;
                return;
            }

            foreach (var screen in ScreensToUpdate)
            {
                if (screen.IsHidden || screen.IsBackground)
                    continue;

                screen.Update(gameTime);

                if (screen.IsJustNowActive)
                {
                    // Skip one HandleInput, now we won't get a ESC button loop
                    screen.ToActive();
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
                if (screen.IsHidden)
                    continue;

                screen.Draw(gameTime);
            }
        }
    }
}
