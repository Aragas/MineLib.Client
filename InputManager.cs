using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Aragas.Core.Wrappers;

namespace MineLib.PGL
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


        private static MouseState CurrentMouseState;
        private static KeyboardState CurrentKeyboardState;
        private static GamePadState CurrentGamePadState;
        public static TouchCollection CurrentTouchCollection;
        public static List<Keys> CurrentOtherKeys;

        private static MouseState LastMouseState;
        private static KeyboardState LastKeyboardState;
        private static GamePadState LastGamePadState;
        public static TouchCollection LastTouchCollection;
        public static List<Keys> LastOtherKeys;

        #endregion Fields

        #region Properties

        public static List<GestureSample> TouchGestures = new List<GestureSample>();


        public static Point MousePosition => CurrentMouseState.Position;

        public static bool MouseLeftClicked => CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released;
        public static bool MouseRightClicked => CurrentMouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released;

        public static bool MoveForward { get { return IsCurrentKeyPressed(Keys.W) || IsCurrentButtonPressed(Buttons.LeftThumbstickUp) || moveForwardTouch; } }
        private static bool moveForwardTouch;

        public static bool MoveBackward { get { return IsCurrentKeyPressed(Keys.S) || IsCurrentButtonPressed(Buttons.LeftThumbstickDown) || moveBackwardTouch; } }
        private static bool moveBackwardTouch;

        public static bool MoveLeft { get { return IsCurrentKeyPressed(Keys.A) || IsCurrentButtonPressed(Buttons.LeftThumbstickLeft) || moveLeftTouch; } }
        private static bool moveLeftTouch;

        public static bool MoveRight { get { return IsCurrentKeyPressed(Keys.D) || IsCurrentButtonPressed(Buttons.LeftThumbstickRight) || moveRightTouch; } }
        private static bool moveRightTouch;

        public static bool MoveJump { get { return IsCurrentKeyPressed(Keys.Space) || IsCurrentButtonPressed(Buttons.B) || moveJumpTouch; } }
        private static bool moveJumpTouch;

        public static bool MoveCrouch { get { return IsCurrentKeyPressed(Keys.LeftShift) || IsCurrentButtonPressed(Buttons.A) || moveCrouchTouch; } }
        private static bool moveCrouchTouch;



        public static bool ButtonCameraUp => IsCurrentKeyPressed(Keys.Up) || IsCurrentButtonPressed(Buttons.RightThumbstickUp);
        public static bool ButtonCameraDown => IsCurrentKeyPressed(Keys.Down) || IsCurrentButtonPressed(Buttons.RightThumbstickDown);
        public static bool ButtonCameraLeft => IsCurrentKeyPressed(Keys.Left) || IsCurrentButtonPressed(Buttons.RightThumbstickLeft);
        public static bool ButtonCameraRight => IsCurrentKeyPressed(Keys.Right) || IsCurrentButtonPressed(Buttons.RightThumbstickRight);
        public static bool ButtonCameraSwitch { get { return IsOncePressed(Keys.O); } }



        public static bool MouseScrollUp => CurrentMouseState.ScrollWheelValue > LastMouseState.ScrollWheelValue;
        public static bool MouseScrollDown => CurrentMouseState.ScrollWheelValue < LastMouseState.ScrollWheelValue;

        public static bool MenuUIUp => (CurrentGamePadState.DPad.Up == ButtonState.Pressed && LastGamePadState.DPad.Up == ButtonState.Released && CurrentGamePadState.IsButtonUp(Buttons.LeftTrigger)) || IsOncePressed(Keys.Up);
        public static bool MenuUIDown => (CurrentGamePadState.DPad.Down == ButtonState.Pressed && LastGamePadState.DPad.Down == ButtonState.Released && CurrentGamePadState.IsButtonUp(Buttons.LeftTrigger)) || IsOncePressed(Keys.Down);
        public static bool MenuUIPressed => IsOncePressed(Buttons.A) || IsOncePressed(Keys.Enter);

        public static bool GameUILeft => CurrentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && LastGamePadState.Buttons.LeftShoulder == ButtonState.Released;
        public static bool GameUIRight => CurrentGamePadState.Buttons.RightShoulder == ButtonState.Pressed && LastGamePadState.Buttons.RightShoulder == ButtonState.Released;

        public static Keys[] CurrentKeys { get { var list = new List<Keys>(CurrentOtherKeys); list.AddRange(CurrentKeyboardState.GetPressedKeys()); return list.ToArray(); } }
        public static Keys[] LastKeys { get { var list = new List<Keys>(LastOtherKeys); list.AddRange(LastKeyboardState.GetPressedKeys()); return list.ToArray(); } }

        #endregion Properties

        static InputManager()
        {
            CurrentOtherKeys = new List<Keys>();
            LastOtherKeys = new List<Keys>();

            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Hold | GestureType.DoubleTap | GestureType.Tap;
            //TouchPanel.EnableMouseGestures = true;
            //TouchPanel.EnableMouseTouchPoint = true;


            InputWrapper.OnKey += KeyOnKey;
        }

        private static void KeyOnKey(object sender, KeyPressedEventArgs e)
        {
        //    CurrentKeys.Add((Keys)e.Key);
        }

        public static void Update(GameTime time)
        {
            LastOtherKeys = new List<Keys>(CurrentOtherKeys);
            CurrentOtherKeys.Clear();

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
                            if (TouchCameraRectangle.Contains(gesture.Position))
                                TouchCameraDelta = gesture.Delta;

                            if (TouchMoveRectangle.Contains(gesture.Position))
                                TouchMoveDelta = gesture.Delta;
                            break;
                    }
                }

            }

        }

        #region Methods

        public static bool IsCurrentKeyPressed(Keys key) { return CurrentKeys.Any(pressedKey => pressedKey == key); }
        public static bool IsLastKeyPressed(Keys key) { return LastKeys.Any(pressedKey => pressedKey == key); }
        public static bool IsCurrentButtonPressed(Buttons button) { return CurrentGamePadState.IsButtonDown(button); }

        public static bool IsOncePressed(Keys key) { return IsCurrentKeyPressed(key) && !IsLastKeyPressed(key); }
        public static bool IsOncePressed(Buttons button) { return CurrentGamePadState.IsButtonDown(button) && LastGamePadState.IsButtonUp(button); }
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
            var angle = (float) Math.Atan2(gamepadThumbStick.Y, gamepadThumbStick.X);
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

        public static AnalogStickDirection GetAnalogStickLeftDirection()
        {
            var gamepadThumbStick = CurrentGamePadState.ThumbSticks.Left;

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
        public static AnalogStickDirection GetAnalogStickRightDirection()
        {
            var gamepadThumbStick = CurrentGamePadState.ThumbSticks.Right;

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

        public static Vector2 AnalogStickLeft => CurrentGamePadState.ThumbSticks.Left;
        public static Vector2 AnalogStickRight => CurrentGamePadState.ThumbSticks.Right;

        #endregion
    }
}