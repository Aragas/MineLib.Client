using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MineLib.PGL.Components.NotUsed
{
    public sealed class CameraComponent : GameComponent
    {
        Vector3 _cameraPosition;
        Vector3 _cameraRotation;
	    Vector3 _cameraLookAt;

        readonly float _cameraSpeed;

        Vector3 _mouseRotationBuffer;

        
        public Vector3 Position
        {
            get { return _cameraPosition; }
            set
            {
                _cameraPosition = value;
                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return _cameraRotation; }
            set
            {
                _cameraRotation = value;
                UpdateLookAt();
            }
        }

        public Matrix Projection { get; private set; }

        public Matrix View { get { return Matrix.CreateLookAt(_cameraPosition, _cameraLookAt, Vector3.Up); } }

        public BoundingFrustum BoundingFrustum { get { return new BoundingFrustum(View * Projection); } }


	    public CameraComponent(Game game, Vector3 position, Vector3 rotation, float speed) : base(game)
        {
            _cameraSpeed = speed;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Game.GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000.0f);

            MoveTo(position, rotation);

            InputManager.TouchCameraRectangle = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2,
                Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
        }


        public void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }

        private void UpdateLookAt()
        {
            var rotationMatrix = Matrix.CreateRotationX(_cameraRotation.X) * Matrix.CreateRotationY(_cameraRotation.Y);
            var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            _cameraLookAt = _cameraPosition + lookAtOffset;
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

                Move(moveVector);
            }
            #endregion Moving


            #region Camera
            if (InputManager.ButtonCameraUp)
                _mouseRotationBuffer.Y += dt;
            if (InputManager.ButtonCameraDown)
                _mouseRotationBuffer.Y -= dt;
            if (InputManager.ButtonCameraLeft)
                _mouseRotationBuffer.X += dt;
            if (InputManager.ButtonCameraRight)
                _mouseRotationBuffer.X -= dt;

            if (InputManager.ButtonCameraSwitch)
                InputManager.MouseEnabled = !InputManager.MouseEnabled;

            if (InputManager.MouseEnabled)
            {
                //Cache mouse location
                float deltaX = InputManager.MouseCamera.X - (Game.GraphicsDevice.Viewport.Width / 2);
                float deltaY = InputManager.MouseCamera.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                _mouseRotationBuffer.X -= 0.01f * deltaX * dt;
                _mouseRotationBuffer.Y -= 0.01f * deltaY * dt;

                if (_mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    _mouseRotationBuffer.Y = _mouseRotationBuffer.Y - (_mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (_mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    _mouseRotationBuffer.Y = _mouseRotationBuffer.Y - (_mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            }

            Rotation = new Vector3(-MathHelper.Clamp(_mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_mouseRotationBuffer.X), 0);
            #endregion Camera


            base.Update(gameTime);
        }
    }
}
