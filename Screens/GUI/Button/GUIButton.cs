using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MineLib.PGL.Screens.GUI.Button
{
    public abstract class GUIButton : GUIItem
    {
        public event Action OnButtonPressed;

        protected Color TextureColor { get; set; }

        protected string ButtonText { get; set; }
        public Rectangle ButtonRectangle { get; set; }
        protected Rectangle ButtonRectangleShadow { get { return new Rectangle(ButtonRectangle.X + 2, ButtonRectangle.Y + 2, ButtonRectangle.Width, ButtonRectangle.Height); } }
        
        protected readonly int WidgetSize = 256;

        #region Button

        protected Rectangle ButtonPosition = new Rectangle(0, 66, 200, 20);
        protected Rectangle ButtonPressedPosition = new Rectangle(0, 86, 200, 20);
        protected Rectangle ButtonUnavailablePosition = new Rectangle(0, 46, 200, 20);

        #endregion Button

        #region HalfButton

        protected Rectangle ButtonFirstHalfPosition = new Rectangle(0, 66, 49, 20);
        protected Rectangle ButtonSecondHalfPosition = new Rectangle(151, 66, 49, 20);

        protected Rectangle ButtonPressedFirstHalfPosition = new Rectangle(0, 86, 49, 20);
        protected Rectangle ButtonPressedSecondHalfPosition = new Rectangle(151, 86, 49, 20);

        protected Rectangle ButtonUnavailableFirstHalfPosition = new Rectangle(0, 46, 49, 20);
        protected Rectangle ButtonUnavailableSecondHalfPosition = new Rectangle(151, 46, 49, 20);

        protected Rectangle ButtonRectangleFirstHalf;
        protected Rectangle ButtonRectangleSecondHalf;

        #endregion HalfButton

        protected Color ButtonColor = Color.LightGray;
        protected Color ButtonShadowColor = Color.Black;

        protected Color ButtonPressedColor = Color.Gold;
        protected Color ButtonPressedShadowColor = Color.Black;

        protected Color ButtonUnavailableColor = Color.Gray;


        protected Texture2D WidgetsTexture { get { return TextureStorage.GUITextures.Widgets; } }


        private SoundEffect ButtonEffect { get; set; }


        protected GUIButton(Client game, Screen screen, string text, Action action, Color textureColor) : base(game, screen)
        {
            #region Texture scaling

            int widgetSize = WidgetsTexture.Width;
            if (WidgetSize != widgetSize)
            {
                var scale = (float)widgetSize / (float)WidgetSize;

                ButtonPosition.X = (int)(ButtonPosition.X * scale);
                ButtonPosition.Y = (int)(ButtonPosition.Y * scale);
                ButtonPosition.Width = (int)(ButtonPosition.Width * scale);
                ButtonPosition.Height = (int)(ButtonPosition.Height * scale);

                ButtonPressedPosition.X = (int)(ButtonPressedPosition.X * scale);
                ButtonPressedPosition.Y = (int)(ButtonPressedPosition.Y * scale);
                ButtonPressedPosition.Width = (int)(ButtonPressedPosition.Width * scale);
                ButtonPressedPosition.Height = (int)(ButtonPressedPosition.Height * scale);

                ButtonUnavailablePosition.X = (int)(ButtonUnavailablePosition.X * scale);
                ButtonUnavailablePosition.Y = (int)(ButtonUnavailablePosition.Y * scale);
                ButtonUnavailablePosition.Width = (int)(ButtonUnavailablePosition.Width * scale);
                ButtonUnavailablePosition.Height = (int)(ButtonUnavailablePosition.Height * scale);

                #region HalfButton

                ButtonFirstHalfPosition.X = (int)(ButtonFirstHalfPosition.X * scale);
                ButtonFirstHalfPosition.Y = (int)(ButtonFirstHalfPosition.Y * scale);
                ButtonFirstHalfPosition.Width = (int)(ButtonFirstHalfPosition.Width * scale);
                ButtonFirstHalfPosition.Height = (int)(ButtonFirstHalfPosition.Height * scale);

                ButtonSecondHalfPosition.X = (int)(ButtonSecondHalfPosition.X * scale);
                ButtonSecondHalfPosition.Y = (int)(ButtonSecondHalfPosition.Y * scale);
                ButtonSecondHalfPosition.Width = (int)(ButtonSecondHalfPosition.Width * scale);
                ButtonSecondHalfPosition.Height = (int)(ButtonSecondHalfPosition.Height * scale);

                ButtonPressedFirstHalfPosition.X = (int)(ButtonPressedFirstHalfPosition.X * scale);
                ButtonPressedFirstHalfPosition.Y = (int)(ButtonPressedFirstHalfPosition.Y * scale);
                ButtonPressedFirstHalfPosition.Width = (int)(ButtonPressedFirstHalfPosition.Width * scale);
                ButtonPressedFirstHalfPosition.Height = (int)(ButtonPressedFirstHalfPosition.Height * scale);

                ButtonPressedSecondHalfPosition.X = (int)(ButtonPressedSecondHalfPosition.X * scale);
                ButtonPressedSecondHalfPosition.Y = (int)(ButtonPressedSecondHalfPosition.Y * scale);
                ButtonPressedSecondHalfPosition.Width = (int)(ButtonPressedSecondHalfPosition.Width * scale);
                ButtonPressedSecondHalfPosition.Height = (int)(ButtonPressedSecondHalfPosition.Height * scale);

                ButtonUnavailableFirstHalfPosition.X = (int)(ButtonUnavailableFirstHalfPosition.X * scale);
                ButtonUnavailableFirstHalfPosition.Y = (int)(ButtonUnavailableFirstHalfPosition.Y * scale);
                ButtonUnavailableFirstHalfPosition.Width = (int)(ButtonUnavailableFirstHalfPosition.Width * scale);
                ButtonUnavailableFirstHalfPosition.Height = (int)(ButtonUnavailableFirstHalfPosition.Height * scale);

                ButtonUnavailableSecondHalfPosition.X = (int)(ButtonUnavailableSecondHalfPosition.X * scale);
                ButtonUnavailableSecondHalfPosition.Y = (int)(ButtonUnavailableSecondHalfPosition.Y * scale);
                ButtonUnavailableSecondHalfPosition.Width = (int)(ButtonUnavailableSecondHalfPosition.Width * scale);
                ButtonUnavailableSecondHalfPosition.Height = (int)(ButtonUnavailableSecondHalfPosition.Height * scale);

                #endregion HalfButton
            }

            #endregion Texture scaling

            TextureColor = textureColor;
            ButtonText = text;
            OnButtonPressed += action;

            ButtonEffect = Game.Content.Load<SoundEffect>(@"Sounds\menu_button");
        }

        public override void Draw(GameTime gameTime) { }

        public override void Update(GameTime gameTime)
        {
            #region Mouse handling

            if (ButtonRectangle.Intersects(new Rectangle(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y, 1, 1)) && !IsNonPressable)
            {
                ToSelectedMouseHover();

                if (InputManager.MouseLeftClicked)
                    PressButton();
            }
            else if (InputManager.CurrentTouchCollection.Any(location => ButtonRectangle.Contains(location.Position)) && !IsNonPressable)
            {
                ToSelectedMouseHover();


                foreach (var gesture in InputManager.TouchGestures)
                    if (gesture.GestureType == GestureType.Tap && ButtonRectangle.Contains(gesture.Position))
                    {
                        PressButton();

                        break;
                    }
            }
            else if (!IsSelected && !IsNonPressable)
                ToActive();

            #endregion
        }

        public override void Dispose()
        {
            base.Dispose();

            if(OnButtonPressed != null)
                foreach (var @delegate in OnButtonPressed.GetInvocationList())
                    OnButtonPressed -= (Action) @delegate;
            
            if (ButtonEffect != null)
                ButtonEffect.Dispose();
        }



        public void PressButton()
        {
            ButtonEffect.Play();

            if (OnButtonPressed != null)
                OnButtonPressed();
        }
    }
}
