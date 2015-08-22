using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MineLib.PGL.Components
{
    public sealed class CameraComponent : GameComponent
    {
        float fovAngle;

        float near = 0.01f;
        float far = 5000.0f;


        Vector3 _cameraPosition;
        Vector3 _cameraRotation;
	    Vector3 _cameraLookAt;

        readonly float _cameraSpeed;

        Vector2 _cameraRotationBuffer;
        Vector2 _cameraPositionBuffer;

        bool _recalculate;

        
        public Vector3 Position { get { return _cameraPosition; } set { _cameraPosition = value; _recalculate = true; } }

        public Vector3 Rotation { get { return _cameraRotation; } set { _cameraRotation = value; _recalculate = true; } }

        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }
        public BoundingFrustum BoundingFrustum { get; private set; }


        internal Data.World World { get; set; }


        public CameraComponent(Game game, Vector3 position, Vector3 rotation, float speed) : base(game)
        {
            fovAngle = MathHelper.PiOver4; 

            _cameraSpeed = speed;

            MoveTo(position, rotation);

            InputManager.TouchCameraRectangle = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2, 0, 
                Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height);
            InputManager.TouchMoveRectangle = new Rectangle(0, 0, 
                Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height);

            _recalculate = true;
        }


        public void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            var rotate = Matrix.CreateRotationY(_cameraRotation.Y);

            var movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);

            return _cameraPosition + movement;
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

        public override void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

#if DEBUG
            if (InputManager.IsOncePressed(Keys.U))
                MoveTo(DebugComponent.PlayerPos, Vector3.Zero);
#endif


            #region Moving
            var moveVector = Vector3.Zero;

            if (InputManager.MoveForward)
                moveVector.Z = 1f;
            if (InputManager.MoveBackward)
                moveVector.Z = -1f;
            if (InputManager.MoveLeft)
                moveVector.X = 1f;
            if (InputManager.MoveRight)
                moveVector.X = -1f;
            if (InputManager.MoveJump)
                moveVector.Y = 1f;
            if (InputManager.MoveCrouch)
                moveVector.Y = -1f;


            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
                moveVector *= dt * _cameraSpeed;
            }

            if (InputManager.TouchEnabled)
            {
                moveVector.X -= InputManager.TouchMoveDelta.X * dt;
                moveVector.Z -= InputManager.TouchMoveDelta.Y * dt;
            }

            Move(moveVector);
            #endregion Moving


            #region Camera
            if (InputManager.ButtonCameraSwitch)
                InputManager.MouseEnabled = !InputManager.MouseEnabled;

            if (InputManager.MouseEnabled)
            {
                var center = new Vector2(Game.GraphicsDevice.Viewport.Width * 0.5f, Game.GraphicsDevice.Viewport.Height * 0.5f);
                var mouseDelta = InputManager.MouseCamera - center;
   

                // Android set mouse to (0, 0)
                if (center != -mouseDelta)
                {
                    Mouse.SetPosition((int)center.X, (int)center.Y);

                    _cameraRotationBuffer -= 0.01f * mouseDelta * dt;
                }
            }

            if (InputManager.TouchEnabled)
                _cameraRotationBuffer -= 0.1f * InputManager.TouchCameraDelta * dt;
            

            if (_cameraRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
            if (_cameraRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(75.0f));

            Rotation = new Vector3(-MathHelper.Clamp(_cameraRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_cameraRotationBuffer.X), 0);
            //Position = new Vector3(-MathHelper.Clamp(_cameraPositionBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_cameraPositionBuffer.X), 0);
            #endregion Camera


            base.Update(gameTime);
        }


        public void ApplyTo(Effect effect)
        {
            if (_recalculate)
                Recalculate();

            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["World"].SetValue(Matrix.Identity);

#if DEBUG
            DebugComponent.CameraPos = Position;
#endif
        }

        public void ApplyTo(BasicEffect effect)
        {
            if (_recalculate)
                Recalculate();

            effect.View = View;
            effect.Projection = Projection;
            effect.World = Matrix.Identity;

#if DEBUG
            DebugComponent.CameraPos = Position;
#endif
        }

        public void ApplyTo(AlphaTestEffect effect)
        {
            if (_recalculate)
                Recalculate();

            effect.View = View;
            effect.Projection = Projection;
            effect.World = Matrix.Identity;

#if DEBUG
            DebugComponent.CameraPos = Position;
#endif
        }

        private void Recalculate()
        {
            var rotationMatrix = Matrix.CreateRotationX(_cameraRotation.X) * Matrix.CreateRotationY(_cameraRotation.Y);
            var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            _cameraLookAt = _cameraPosition + lookAtOffset;

            View = Matrix.CreateLookAt(_cameraPosition, _cameraLookAt, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, Game.GraphicsDevice.Viewport.AspectRatio, near, far);
            
            BoundingFrustum = new BoundingFrustum(View * Projection);

            _recalculate = false;
        }
    }
}
