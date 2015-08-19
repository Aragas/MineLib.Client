using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using MineLib.PGL.Screens.GUI.GamePad;

namespace MineLib.PGL.Screens.GUI.InputBox
{
    public delegate void InputBoxEventHandler(string text);

    public abstract class GUIInputBox : GUIItem
    {
        public event InputBoxEventHandler OnEnterPressed;
        public event Action<GUIInputBox> OnFocused;
        public event Action<GUIInputBox> OnUnFocused;


        public Color UsingColor { get; set; }
        public bool ShowInput { get; protected internal set; }

        protected Rectangle InputBoxRectangle { get; set; }


        public string Text { get { return _text; } private set { _text = value; } }
        private string _text = "";

        protected Color TextColor = Color.White;
        protected Color TextShadowColor = Color.Gray;
       
        protected Texture2D BlackTexture { get; set; }


        private PadDaisywheel PadDaisywheel { get; set; }


        protected GUIInputBox(Client game, Screen screen, InputBoxEventHandler onEnterPressed, Color style) : base(game, screen)
        {
            UsingColor = style;
            OnEnterPressed += onEnterPressed;

            PadDaisywheel = Screen.Daisywheel;
        }

        public override void Update(GameTime gameTime)
        {
            #region Mouse handling

            if (!IsNonPressable && InputBoxRectangle.Intersects(new Rectangle(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y, 1, 1)))
            {
                if (InputManager.MouseLeftClicked)
                {
                    if (OnFocused != null)
                        OnFocused(this);

                    ToSelected();
                }
            }
            else if (!IsNonPressable)
            {
                if (InputManager.MouseLeftClicked)
                {
                    if (OnUnFocused != null)
                        OnUnFocused(this);

                    ToActive();
                }

                foreach (var gesture in InputManager.TouchGestures)
                    if (gesture.GestureType == GestureType.Tap && InputBoxRectangle.Contains(gesture.Position))
                    {
                        if (OnFocused != null)
                            OnFocused(this);

                        ToSelected();

                        InputManager.ShowKeyboard();

                        break;
                    }
                    else if(gesture.GestureType == GestureType.Tap && !InputBoxRectangle.Contains(gesture.Position))
                    {
                        if (OnUnFocused != null)
                            OnUnFocused(this);
                    
                        ToActive();
                    
                        InputManager.HideKeyboard();
                    
                        break;
                    }
            }
            /*
            else if (!IsNonPressable)
            {


                foreach (var gesture in InputManager.Gestures)
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        if (OnUnFocused != null)
                            OnUnFocused(this);

                        ToActive();

                        InputManager.HideKeyboard();

                        break;
                    }
            }
            */

            /*
            if (!IsNonPressable && !InputBoxRectangle.Intersects(new Rectangle(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y, 1, 1)))
            {
                if (InputManager.MouseLeftClicked)
                {
                    if (OnUnFocused != null)
                        OnUnFocused(this);

                    ToActive();
                }
            }
            else if (!IsNonPressable && !InputManager.CurrentTouchCollection.Any(location => InputBoxRectangle.Contains(location.Position)))
            {
                foreach (var gesture in InputManager.Gestures)
                    if (gesture.GestureType == GestureType.Tap && !InputBoxRectangle.Contains(gesture.Position))
                    {
                        if (OnUnFocused != null)
                            OnUnFocused(this);

                        ToActive();

                        InputManager.HideKeyboard();

                        break;
                    }
            }
            */
            #endregion

            #region Keyboard handling

            if (IsSelected)
            {
                var keys = new List<Keys>();
                keys.AddRange(InputManager.CurrentKeyboardState.GetPressedKeys());
                keys.AddRange(InputManager.CurrentKeys);

                foreach (var key in keys)
                    if (InputManager.LastKeyboardState.IsKeyUp(key) || InputManager.IsLastKeyPressed(key))
                        switch (key)
                        {
                            case Keys.Back:
                                if (Text.Length == 0)
                                    continue;

                                Text = Text.Remove(Text.Length - 1, 1);
                                break;

                            case Keys.Enter:
                                if(OnEnterPressed != null)
                                    OnEnterPressed(Text);
                                break;

                            default:
                                Text += ConvertKeyboardInput(InputManager.CurrentKeyboardState, key);
                                break;
                        }
            }


            #endregion

            if (IsSelected && InputManager.IsOncePressed(Buttons.LeftTrigger, Buttons.Start))
            {
                if (PadDaisywheel.IsHidden)
                {
                    PadDaisywheel.ResetEvents();
                    PadDaisywheel.OnCharReceived += GamePadDaisywheelOnOnCharReceived;
                    PadDaisywheel.OnCharDeleted += GamePadDaisywheelOnOnCharDeleted;

                    PadDaisywheel.IsHidden = false;
                }
                else
                    PadDaisywheel.IsHidden = true;
            }

            //if (InputBoxText.Length == 0 && OnEmpty != null)
            //    OnEmpty();
            //else if (OnNonEmpty != null)
            //    OnNonEmpty();
        }

        public override void Draw(GameTime gameTime)
        {
            PadDaisywheel.Draw(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (OnEnterPressed != null)
                foreach (var @delegate in OnEnterPressed.GetInvocationList())
                    OnEnterPressed -= (InputBoxEventHandler) @delegate;

            if (OnFocused != null)
                foreach (var @delegate in OnFocused.GetInvocationList())
                    OnFocused -= (Action<GUIInputBox>) @delegate;

            if (OnUnFocused != null)
                foreach (var @delegate in OnUnFocused.GetInvocationList())
                    OnUnFocused -= (Action<GUIInputBox>) @delegate;
            
            if (BlackTexture != null)
                BlackTexture.Dispose();
        }


        private void GamePadDaisywheelOnOnCharDeleted()
        {
            if(Text.Length > 0)
                Text = Text.Remove(Text.Length - 1, 1);
        }

        private void GamePadDaisywheelOnOnCharReceived(char character)
        {
            Text += character;
        }

        private static char? ConvertKeyboardInput(KeyboardState keyboard, Keys key)
        {
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            switch (key)
            {
                //Alphabet keys
                case Keys.A:                return shift ? 'A' : 'a';
                case Keys.B:                return shift ? 'B' : 'b';
                case Keys.C:                return shift ? 'C' : 'c';
                case Keys.D:                return shift ? 'D' : 'd';
                case Keys.E:                return shift ? 'E' : 'e';
                case Keys.F:                return shift ? 'F' : 'f';
                case Keys.G:                return shift ? 'G' : 'g';
                case Keys.H:                return shift ? 'H' : 'h';
                case Keys.I:                return shift ? 'I' : 'i';
                case Keys.J:                return shift ? 'J' : 'j';
                case Keys.K:                return shift ? 'K' : 'k';
                case Keys.L:                return shift ? 'L' : 'l';
                case Keys.M:                return shift ? 'M' : 'm';
                case Keys.N:                return shift ? 'N' : 'n';
                case Keys.O:                return shift ? 'O' : 'o';
                case Keys.P:                return shift ? 'P' : 'p';
                case Keys.Q:                return shift ? 'Q' : 'q';
                case Keys.R:                return shift ? 'R' : 'r';
                case Keys.S:                return shift ? 'S' : 's';
                case Keys.T:                return shift ? 'T' : 't';
                case Keys.U:                return shift ? 'U' : 'u';
                case Keys.V:                return shift ? 'V' : 'v';
                case Keys.W:                return shift ? 'W' : 'w';
                case Keys.X:                return shift ? 'X' : 'x';
                case Keys.Y:                return shift ? 'Y' : 'y';
                case Keys.Z:                return shift ? 'Z' : 'z';

                //Decimal keys
                case Keys.D0:               return shift ? ')' : '0';
                case Keys.D1:               return shift ? '!' : '1';
                case Keys.D2:               return shift ? '@' : '2';
                case Keys.D3:               return shift ? '#' : '3';
                case Keys.D4:               return shift ? '$' : '4';
                case Keys.D5:               return shift ? '%' : '5';
                case Keys.D6:               return shift ? '^' : '6';
                case Keys.D7:               return shift ? '&' : '7';
                case Keys.D8:               return shift ? '*' : '8';
                case Keys.D9:               return shift ? '(' : '9';

                //Decimal numpad keys
                case Keys.NumPad0:          return '0';
                case Keys.NumPad1:          return '1';
                case Keys.NumPad2:          return '2';
                case Keys.NumPad3:          return '3';
                case Keys.NumPad4:          return '4';
                case Keys.NumPad5:          return '5';
                case Keys.NumPad6:          return '6';
                case Keys.NumPad7:          return '7';
                case Keys.NumPad8:          return '8';
                case Keys.NumPad9:          return '9';

                //Special keys
                case Keys.OemTilde:         return shift ? '~' : '`';
                case Keys.OemSemicolon:     return shift ? ':' : ';';
                case Keys.OemQuotes:        return shift ? '"' : '\'';
                case Keys.OemQuestion:      return shift ? '?' : '/';
                case Keys.OemPlus:          return shift ? '+' : '=';
                case Keys.OemPipe:          return shift ? '|' : '\\';
                case Keys.OemPeriod:        return shift ? '>' : '.';
                case Keys.OemOpenBrackets:  return shift ? '{' : '[';
                case Keys.OemCloseBrackets: return shift ? '}' : ']';
                case Keys.OemMinus:         return shift ? '_' : '-';
                case Keys.OemComma:         return shift ? '<' : ',';
                case Keys.Space:            return ' ';

            }

            return null;
        }
    }
}
