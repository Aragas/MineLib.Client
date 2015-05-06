using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MineLib.PCL.Graphics
{
    public class Camera : GameComponent
    {
        Vector3 _cameraPosition;
        Vector3 _cameraRotation;
	    readonly float _cameraSpeed;
        Vector3 _cameraLookAt;
        Vector3 _mouseRotationBuffer;
        MouseState _currentMouseState;
        MouseState _prevMouseState;

        
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


	    public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game)
        {
            _cameraSpeed = speed;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 5000.0f);

            MoveTo(position, rotation);
			
            _prevMouseState = Mouse.GetState();
        }

        public void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }

        private void UpdateLookAt()
        {
            //Build a rotation matrix
            Matrix rotationMatrix = Matrix.CreateRotationX(_cameraRotation.X) * Matrix.CreateRotationY(_cameraRotation.Y);
            //Build look at offset vector
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            //Update our camera's look at vector
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
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentMouseState = Mouse.GetState();

            var ks = Keyboard.GetState();

            //Handle basic key movement

            var moveVector = Vector3.Zero;

            if (ks.IsKeyDown(Keys.W))
                moveVector.Z = 1f;
            if (ks.IsKeyDown(Keys.S))
                moveVector.Z = -1f;
            if (ks.IsKeyDown(Keys.A))
                moveVector.X = 1f;
            if (ks.IsKeyDown(Keys.D))
                moveVector.X = -1f;
            if (ks.IsKeyDown(Keys.Space) || ks.IsKeyDown(Keys.Q))
                moveVector.Y = 1f;
            if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.E))
                moveVector.Y = -1f;

            if (ks.IsKeyDown(Keys.Up))
            {
                _mouseRotationBuffer.Y += 1f * dt;
                Rotation = new Vector3(-MathHelper.Clamp(_mouseRotationBuffer.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)), MathHelper.WrapAngle(_mouseRotationBuffer.X), 0f);
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                _mouseRotationBuffer.Y -= 1f * dt;
                Rotation = new Vector3(-MathHelper.Clamp(_mouseRotationBuffer.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)), MathHelper.WrapAngle(_mouseRotationBuffer.X), 0f);
            }
            if (ks.IsKeyDown(Keys.Left))
            {
                _mouseRotationBuffer.X += 1f * dt;
                Rotation = new Vector3(-MathHelper.Clamp(_mouseRotationBuffer.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)), MathHelper.WrapAngle(_mouseRotationBuffer.X), 0f);
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                _mouseRotationBuffer.X -= 1f * dt;
                Rotation = new Vector3(-MathHelper.Clamp(_mouseRotationBuffer.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)), MathHelper.WrapAngle(_mouseRotationBuffer.X), 0f);
            }
            //if (ks.IsKeyDown(Keys.O))
            //{
            //    mouseOn = !mouseOn;
            //}

            if (moveVector != Vector3.Zero)
            {
                //normalize that vector
                //so that we don't move faster diagonally
                moveVector.Normalize();
                //Now we add in smooth and speed
                moveVector *= dt * _cameraSpeed;

                //Move camera
                Move(moveVector);
            }

            //Handle mouse movement

	        if (_currentMouseState != _prevMouseState)
            {
                //Cache mouse location
                float deltaX = _currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                float deltaY = _currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                //Calculate rotation from mouse movement
                _mouseRotationBuffer.X -= 0.01f * deltaX * dt;
                _mouseRotationBuffer.Y -= 0.01f * deltaY * dt;

                //Clamp the rotational movement
                if (_mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    _mouseRotationBuffer.Y = _mouseRotationBuffer.Y - (_mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (_mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    _mouseRotationBuffer.Y = _mouseRotationBuffer.Y - (_mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                //Finally add that rotation to our rotation vector clamping as needed
                Rotation = new Vector3(-MathHelper.Clamp(_mouseRotationBuffer.Y,
                                    MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)),
                                    MathHelper.WrapAngle(_mouseRotationBuffer.X), 0);
            }

            //if (mouseOn) //Set mouse cursor to center of screen
                Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            //Set prev state to current state
            _prevMouseState = _currentMouseState;

            base.Update(gameTime);
        }
    }
}
