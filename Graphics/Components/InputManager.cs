using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using MineLib.Core.Wrappers;

namespace MineLib.PCL.Graphics.Components
{
    public enum AnalogStickDirection { None, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }

    public static class InputManager
    {
        public static bool TouchEnabled = true;
        public static bool MouseEnabled = true;
        public static bool KeyboardEnabled = true;
        public static bool GamepadEnabled = true;


        #region Fields

        public static Rectangle TouchCameraRectangle;
        public static Vector2   TouchCameraDelta;

        public static Rectangle TouchMoveRectangle;
        public static Vector2   TouchMoveDelta;
        public static Rectangle UpArrwow, DownArrow, LeftArrow, RightArrow, Jump, Crouch;


        public static MouseState CurrentMouseState;
        public static KeyboardState CurrentKeyboardState;
        public static GamePadState CurrentGamePadState;
        public static TouchCollection CurrentTouchCollection;
        public static List<Keys> CurrentKeys;

        public static MouseState LastMouseState;
        public static KeyboardState LastKeyboardState;
        public static GamePadState LastGamePadState;
        public static TouchCollection LastTouchCollection;
        public static List<Keys> LastKeys;

        #endregion Fields

        #region Properties

        public static List<GestureSample> TouchGestures = new List<GestureSample>();


        public static Vector2 MouseCamera { get { return new Vector2(CurrentMouseState.Position.X, CurrentMouseState.Position.Y); } }

        public static bool MouseLeftClicked { get { return CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released; } }
        public static bool MouseRightClicked { get { return CurrentMouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released; } }

        public static bool MoveForward { get { return IsPressed(Keys.W) || IsPressed(Buttons.LeftThumbstickUp) || moveForwardTouch; } }
        private static bool moveForwardTouch;

        public static bool MoveBackward { get { return IsPressed(Keys.S) || IsPressed(Buttons.LeftThumbstickDown) || moveBackwardTouch; } }
        private static bool moveBackwardTouch;

        public static bool MoveLeft { get { return IsPressed(Keys.A) || IsPressed(Buttons.LeftThumbstickLeft) || moveLeftTouch; } }
        private static bool moveLeftTouch;

        public static bool MoveRight { get { return IsPressed(Keys.D) || IsPressed(Buttons.LeftThumbstickRight) || moveRightTouch; } }
        private static bool moveRightTouch;

        public static bool MoveJump { get { return IsPressed(Keys.Space) || IsPressed(Buttons.B) || moveJumpTouch; } }
        private static bool moveJumpTouch;

        public static bool MoveCrouch { get { return IsPressed(Keys.LeftShift) || IsPressed(Buttons.A) || moveCrouchTouch; } }
        private static bool moveCrouchTouch;



        public static bool ButtonCameraUp { get { return IsPressed(Keys.Up) || IsPressed(Buttons.RightThumbstickUp); } }
        public static bool ButtonCameraDown { get { return IsPressed(Keys.Down) || IsPressed(Buttons.RightThumbstickDown); } }
        public static bool ButtonCameraLeft { get { return IsPressed(Keys.Left) || IsPressed(Buttons.RightThumbstickLeft); } }
        public static bool ButtonCameraRight { get { return IsPressed(Keys.Right) || IsPressed(Buttons.RightThumbstickRight); } }
        public static bool ButtonCameraSwitch { get { return IsOncePressed(Keys.O); } }



        public static bool MouseScrollUp { get { return CurrentMouseState.ScrollWheelValue > LastMouseState.ScrollWheelValue; } }
        public static bool MouseScrollDown { get { return CurrentMouseState.ScrollWheelValue < LastMouseState.ScrollWheelValue; } }

        public static bool MenuUp { get { return (CurrentGamePadState.DPad.Up == ButtonState.Pressed && LastGamePadState.DPad.Up == ButtonState.Released) || IsOncePressed(Keys.Up); } }
        public static bool MenuDown { get { return (CurrentGamePadState.DPad.Down == ButtonState.Pressed && LastGamePadState.DPad.Down == ButtonState.Released) || IsOncePressed(Keys.Down); } }

        public static bool GUIMenuLeft { get { return CurrentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && LastGamePadState.Buttons.LeftShoulder == ButtonState.Released; } }
        public static bool GUIMenuRight { get { return CurrentGamePadState.Buttons.RightShoulder == ButtonState.Pressed && LastGamePadState.Buttons.RightShoulder == ButtonState.Released; } }

        #endregion Properties

        static InputManager()
        {
            CurrentKeys = new List<Keys>();
            LastKeys = new List<Keys>();

            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Hold | GestureType.DoubleTap | GestureType.Tap;
            //TouchPanel.EnableMouseGestures = true;
            //TouchPanel.EnableMouseTouchPoint = true;


            InputWrapper.OnKey += KeyOnKey;
        }

        
        public static bool IsCurrentKeyPressed(Keys key)
        {
            foreach (var pressedKey in CurrentKeys)
                if (pressedKey == key)
                    return true;
            
            return false;
        }

        public static bool IsLastKeyPressed(Keys key)
        {
            foreach (var pressedKey in LastKeys)
                if (pressedKey == key)
                    return true;

            return false;
        }

        private static Action KeyOnKey(int key)
        {
            CurrentKeys.Add((Keys) key);

            return null;
        }


        #region Methods

        public static void Update(GameTime time)
        {
            LastKeys = new List<Keys>(CurrentKeys);
            CurrentKeys.Clear();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            LastGamePadState = CurrentGamePadState;
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);

            LastTouchCollection = CurrentTouchCollection;
            CurrentTouchCollection = TouchPanel.GetState();


            if (TouchEnabled)
            {
                TouchGestures.Clear();
                while (TouchPanel.IsGestureAvailable)
                    TouchGestures.Add(TouchPanel.ReadGesture());


                TouchCameraDelta = Vector2.Zero;
                foreach (var gesture in TouchGestures)
                {
                    switch (gesture.GestureType)
                    {
                        case GestureType.Hold:
                            break;

                        case GestureType.Tap:
                            break;

                        case GestureType.DoubleTap:
                            break;

                        case GestureType.FreeDrag:
                            if(TouchCameraRectangle.Contains(gesture.Position))
                                TouchCameraDelta = gesture.Delta;

                            if (TouchMoveRectangle.Contains(gesture.Position))
                                TouchMoveDelta = gesture.Delta;
                            break;
                    }
                }

            }

        }


        public static bool IsPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) || IsCurrentKeyPressed(key);
        }

        public static bool IsPressed(Buttons button)
        {
            return CurrentGamePadState.IsButtonDown(button);
        }

        public static bool IsOncePressed(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key)) || (IsCurrentKeyPressed(key) && IsLastKeyPressed(key));
        }

        public static bool IsOncePressed(Buttons button)
        {
            return CurrentGamePadState.IsButtonDown(button) && LastGamePadState.IsButtonUp(button);
        }
        
        public static bool IsOncePressed(Buttons firstButton, Buttons secondButton)
        {
            if (LastGamePadState.IsButtonUp(firstButton) && LastGamePadState.IsButtonUp(secondButton))
                return false;

            return LastGamePadState.IsButtonDown(firstButton) && (LastGamePadState.IsButtonDown(secondButton) && CurrentGamePadState.IsButtonUp(secondButton));
        }


        public static void ShowKeyboard()
        {
            InputWrapper.ShowKeyboard();
        }

        public static void HideKeyboard()
        {
            InputWrapper.HideKeyboard();
        }

        #endregion

        #region AnalogThumbStick stuff

        const float PI = (float) Math.PI;

        // You could make this user-configurable.
        const float Deadzone = 0.8f;
        const float AliveAngle = PI / 4 - (PI / 16);

        // Remember PI = 180 degrees
        const float AliveAngle1Start = PI * 0;
        const float AliveAngle1End = PI * 0.5f - AliveAngle;

        const float AliveAngle2Start = PI * 0.5f + AliveAngle;
        const float AliveAngle2End = PI * 1f - AliveAngle;

        const float AliveAngle3Start = PI * 1f + AliveAngle;
        const float AliveAngle3End = PI * 1.5f - AliveAngle;

        const float AliveAngle4Start = PI * 1.5f + AliveAngle;
        const float AliveAngle4End = PI * 2f - AliveAngle;

        public static AnalogStickDirection GetAnalogStickDirection(Vector2 gamepadThumbStick)
        {
            // Get the length and prevent something from happening
            // if it's in our deadzone.
            var length = gamepadThumbStick.Length();
            if (length < Deadzone)
                return AnalogStickDirection.None;

            // Find the angle that the stick is at. see: http://en.wikipedia.org/wiki/File:Atan2_60.svg
            var angle = (float)Math.Atan2(gamepadThumbStick.Y, gamepadThumbStick.X);
            if (angle < 0)
                angle += PI * 2; // Simpify our checks.

            if (angle > AliveAngle4End)
                return AnalogStickDirection.Right;

            if (angle > AliveAngle4Start)
                return AnalogStickDirection.DownRight;

            if (angle > AliveAngle3End)
                return AnalogStickDirection.Down;

            if (angle > AliveAngle3Start)
                return AnalogStickDirection.DownLeft;

            if (angle > AliveAngle2End)
                return AnalogStickDirection.Left;

            if (angle > AliveAngle2Start)
                return AnalogStickDirection.UpLeft;

            if (angle > AliveAngle1End)
                return AnalogStickDirection.Up;

            if (angle > AliveAngle1Start)
                return AnalogStickDirection.UpRight;

            return AnalogStickDirection.Right;
        }

        #endregion
    }
}