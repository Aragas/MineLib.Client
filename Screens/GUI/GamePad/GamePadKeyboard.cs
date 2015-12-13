using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MineLib.PGL.BMFont;

namespace MineLib.PGL.Screens.GUI.GamePad
{
    struct GamePadKeyboardEntry
    {
        public Rectangle ButtonRectangle;

        public char[] Chars;

        public bool Selected;
    }

    public delegate void GamePadKeyboardEventHandler(char character);

     /// <summary>
     /// Daisywheel implementation from Steam Big Picture
     /// </summary>
    public sealed class PadDaisywheel : MineLibComponent
    {
        public event GamePadKeyboardEventHandler OnCharReceived;

        public event Action OnCharDeleted;

        public bool IsHidden { get; set; }


        Rectangle ScreenRectangle => Game.GraphicsDevice.Viewport.Bounds;

         SpriteBatch SpriteBatch { get; }

        FontRenderer MainTextRenderer { get; }


        Texture2D MainCircleTexture { get; }
        Rectangle MainCircleRectangle { get; }

        Texture2D GamePadButtonsTexture { get; }

        float CharSize => 700 * Scale;

         GamePadKeyboardEntry[] ButtonEntries { get; }

        int ButtonSize { get; set; }
        float Scale { get; }

        Color _color = new Color(255, 255, 255, 200);

        public PadDaisywheel(Client game) : base(game)
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice); 
            IsHidden = true;


            if (MainTextRenderer == null)
                MainTextRenderer = new FontRenderer(GraphicsDevice, "PixelUnicode");

            MainCircleTexture = CreateCircle(GraphicsDevice, (int)(Math.Min(ScreenRectangle.Width, ScreenRectangle.Height) * 0.5f), new Color(25, 45, 60, 255));
            MainCircleRectangle = new Rectangle(
                (int)(ScreenRectangle.Center.X - MainCircleTexture.Width * 0.5f),
                (int)(ScreenRectangle.Center.Y - MainCircleTexture.Height * 0.5f),
                MainCircleTexture.Width,
                MainCircleTexture.Height);

            ButtonEntries = SteamDefault();

            GamePadButtonsTexture = Game.Content.Load<Texture2D>("ControllerButtons");
            //var t = GamePadButtonsTexture.Bounds;
            //GamePadButtonsTexture = new Texture2D(Game.GraphicsDevice, t.Width, t.Height);
            //GamePadButtonsTexture.SetData(new[] { new Color(150, 150, 150, 255) });

            #region Scale
            
            //var size = MainTextRenderer.MeasureText(" ");
            var size = new Vector2(1f);
            var xScale = 1.0f / ((ButtonSize * 0.5f) / size.X);
            var yScale = 1.0f / ((ButtonSize * 0.5f) / size.Y);
            Scale = Math.Min(xScale, yScale);

            #endregion

        }

        private GamePadKeyboardEntry[] SteamDefault()
         {
             int offset = 10;

             var scaleX = MainCircleRectangle.Width * 0.3f;
             var scaleY = MainCircleRectangle.Height * 0.3f;
             ButtonSize = (int)Math.Min(scaleX, scaleY);


             var buttonEntries = new GamePadKeyboardEntry[8];

             // Top
             buttonEntries[0] = new GamePadKeyboardEntry();
             buttonEntries[0].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X - ButtonSize * 0.5f),
                 (int)(MainCircleRectangle.Center.Y - MainCircleTexture.Height * 0.5f + offset),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[0].Chars = new[] {'a', 'b', 'c', 'd'};

             // Right Top
             buttonEntries[1] = new GamePadKeyboardEntry();
             buttonEntries[1].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X + MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 (int)(MainCircleRectangle.Center.Y - MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[1].Chars = new[] {'e', 'f', 'g', 'h'};

             // Right
             buttonEntries[2] = new GamePadKeyboardEntry();
             buttonEntries[2].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X + MainCircleTexture.Width * 0.5f - ButtonSize - offset),
                 (int)(MainCircleRectangle.Center.Y - ButtonSize * 0.5f),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[2].Chars = new[] {'i', 'j', 'k', 'l'};

             // Right Bottom
             buttonEntries[3] = new GamePadKeyboardEntry();
             buttonEntries[3].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X + MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 (int)(MainCircleRectangle.Center.Y + MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[3].Chars = new[] {'m', 'n', 'o', 'p'};

             // Bottom
             buttonEntries[4] = new GamePadKeyboardEntry();
             buttonEntries[4].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X - ButtonSize * 0.5f),
                 (int)(MainCircleRectangle.Center.Y + MainCircleTexture.Height * 0.5f - ButtonSize - offset),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[4].Chars = new[] {'q', 'r', 's', 't'};

             // Left Bottom
             buttonEntries[5] = new GamePadKeyboardEntry();
             buttonEntries[5].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X - MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 (int)(MainCircleRectangle.Center.Y + MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[5].Chars = new[] {'u', 'v', 'w', 'x'};

             // Left
             buttonEntries[6] = new GamePadKeyboardEntry();
             buttonEntries[6].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X - MainCircleTexture.Width * 0.5f + offset),
                 (int)(MainCircleRectangle.Center.Y - ButtonSize * 0.5f),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[6].Chars = new[] {'y', 'z', ',', '.'};

             // Left Top
             buttonEntries[7] = new GamePadKeyboardEntry();
             buttonEntries[7].ButtonRectangle = new Rectangle(
                 (int)(MainCircleRectangle.Center.X - MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 (int)(MainCircleRectangle.Center.Y - MainCircleTexture.Height * 0.25f - ButtonSize * 0.5f),
                 ButtonSize,
                 ButtonSize);
             buttonEntries[7].Chars = new[] {':', '/', '@', '-'};

             return buttonEntries;
         }

        public void ResetEvents()
        {
            OnCharDeleted = delegate { };
            OnCharReceived = delegate { };
        }


        public override void Update(GameTime gameTime)
        {
            if (IsHidden)
                return;

            #region AnalogThumbStick

            ButtonEntries[0].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.Up;
            ButtonEntries[1].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.UpRight;
            ButtonEntries[2].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.Right;
            ButtonEntries[3].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.DownRight;
            ButtonEntries[4].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.Down;
            ButtonEntries[5].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.DownLeft;
            ButtonEntries[6].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.Left;
            ButtonEntries[7].Selected = InputManager.GetAnalogStickLeftDirection() == AnalogStickDirection.UpLeft;

            #endregion

            #region RightButtons

            if (InputManager.IsOncePressed(Buttons.X) && InputManager.AnalogStickLeft != Vector2.Zero)
                foreach (var buttonEntry in ButtonEntries)
                    if (buttonEntry.Selected && OnCharReceived != null)
                        OnCharReceived(buttonEntry.Chars[0]);
                

            if (InputManager.IsOncePressed(Buttons.Y) && InputManager.AnalogStickLeft != Vector2.Zero)
                foreach (var buttonEntry in ButtonEntries)
                    if (buttonEntry.Selected && OnCharReceived != null)
                        OnCharReceived(buttonEntry.Chars[1]);
                

            if (InputManager.IsOncePressed(Buttons.B) && InputManager.AnalogStickLeft != Vector2.Zero)
                foreach (var buttonEntry in ButtonEntries)
                    if (buttonEntry.Selected && OnCharReceived != null)
                        OnCharReceived(buttonEntry.Chars[2]);
                

            if (InputManager.IsOncePressed(Buttons.A) && InputManager.AnalogStickLeft != Vector2.Zero)
                foreach (var buttonEntry in ButtonEntries)
                    if (buttonEntry.Selected && OnCharReceived != null)
                        OnCharReceived(buttonEntry.Chars[3]);
                

            #endregion

            if (InputManager.IsOncePressed(Buttons.B) && InputManager.AnalogStickLeft == Vector2.Zero)
                OnCharReceived?.Invoke(' ');
            
            if (InputManager.IsOncePressed(Buttons.X) && InputManager.AnalogStickLeft == Vector2.Zero)
                OnCharDeleted?.Invoke();
        }
        public override void Draw(GameTime gameTime)
        {
            if (IsHidden)
                return;


            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearClamp);

            SpriteBatch.Draw(MainCircleTexture, MainCircleRectangle, _color);

            foreach (var buttonEntry in ButtonEntries)
            {
                if (buttonEntry.Selected)
                    SpriteBatch.Draw(GamePadButtonsTexture, buttonEntry.ButtonRectangle, _color);

                // Left
                var leftChar = new Rectangle(
                    (int)(buttonEntry.ButtonRectangle.X),
                    (int)(buttonEntry.ButtonRectangle.Center.Y - CharSize * 0.5f),
                    (int)(CharSize),
                    (int)(CharSize));
                MainTextRenderer.DrawText(SpriteBatch, buttonEntry.Chars[0].ToString(), leftChar, _color);
                
                // Top
                var topChar = new Rectangle(
                    (int)(buttonEntry.ButtonRectangle.Center.X - CharSize * 0.25f),
                    (int)(buttonEntry.ButtonRectangle.Y + CharSize * 0.25f),
                    (int)(CharSize),
                    (int)(CharSize));
                MainTextRenderer.DrawText(SpriteBatch, buttonEntry.Chars[1].ToString(), topChar, _color);
                
                // Right
                var rightChar = new Rectangle(
                    (int)(buttonEntry.ButtonRectangle.X + buttonEntry.ButtonRectangle.Width - CharSize * 0.5f),
                    (int)(buttonEntry.ButtonRectangle.Center.Y - CharSize * 0.5f),
                    (int)(CharSize),
                    (int)(CharSize));
                MainTextRenderer.DrawText(SpriteBatch, buttonEntry.Chars[2].ToString(), rightChar, _color);
                
                // Bottom
                var bottomChar = new Rectangle(
                    (int)(buttonEntry.ButtonRectangle.Center.X - CharSize * 0.25f),
                    (int)(buttonEntry.ButtonRectangle.Y + buttonEntry.ButtonRectangle.Height - CharSize * 0.75f - CharSize * 0.5f),
                    (int)(CharSize),
                    (int)(CharSize));
                MainTextRenderer.DrawText(SpriteBatch, buttonEntry.Chars[3].ToString(), bottomChar, _color);
            }

            SpriteBatch.End();
        }

        public override void Dispose()
        {
            SpriteBatch?.Dispose();

            MainTextRenderer?.Dispose();

            MainCircleTexture?.Dispose();

            GamePadButtonsTexture?.Dispose();
        }


        // http://stackoverflow.com/questions/5641579/xna-draw-a-filled-circle
        private static Texture2D CreateCircle(GraphicsDevice importedGraphicsDevice, int radius, Color color)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(importedGraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = color;
            }

            //width
            for (int i = 0; i < outerRadius; i++)
            {
                int yStart = -1;
                int yEnd = -1;


                //loop through height to find start and end to fill
                for (int j = 0; j < outerRadius; j++)
                {
                    if (yStart == -1)
                    {
                        if (j == outerRadius - 1)
                        {
                            //last row so there is no row below to compare to
                            break;
                        }

                        //start is indicated by Color followed by Transparent
                        if (data[i + (j * outerRadius)] == color && data[i + ((j + 1) * outerRadius)] == Color.Transparent)
                        {
                            yStart = j + 1;
                            continue;
                        }
                    }
                    else if (data[i + (j * outerRadius)] == color)
                    {
                        yEnd = j;
                        break;
                    }
                }

                //if we found a valid start and end position
                if (yStart != -1 && yEnd != -1)
                {
                    //height
                    for (int j = yStart; j < yEnd; j++)
                    {
                        data[i + (j * outerRadius)] = color;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }
    }
}
