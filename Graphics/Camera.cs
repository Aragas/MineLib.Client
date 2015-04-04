using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MineLib.Client.Graphics
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

	    public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game)
        {
            _cameraSpeed = speed;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000.0f);

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

        private bool mouseOn;
        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentMouseState = Mouse.GetState();

            var ks = Keyboard.GetState();

            //Handle basic key movement

            var moveVector = Vector3.Zero;

            if (ks.IsKeyDown(Keys.W))
                moveVector.Z = 1;
            if (ks.IsKeyDown(Keys.S))
                moveVector.Z = -1;
            if (ks.IsKeyDown(Keys.A))
                moveVector.X = 1;
            if (ks.IsKeyDown(Keys.D))
                moveVector.X = -1;
            if (ks.IsKeyDown(Keys.Space))
                moveVector.Y = 1;
            if (ks.IsKeyDown(Keys.LeftShift))
                moveVector.Y = -1;
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

        public Ray GetMouseRay(GraphicsDevice gd)
        {
            var MousePosition = Mouse.GetState().Position;

            var nearsource = new Vector3(MousePosition.X, MousePosition.Y, 0.0f);
            var farsource = new Vector3(MousePosition.X, MousePosition.Y, 1.0f);

            var world = Matrix.CreateTranslation(0, 0, 0);

            var nearPoint = gd.Viewport.Unproject(nearsource, Projection, View, world);
            var farPoint = gd.Viewport.Unproject(farsource, Projection, View, world);

            var direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        public DRay GetMouseRays(GraphicsDevice gd, int w, int h)
        {
            var rays = new List<Ray>();
            var MousePosition = Mouse.GetState().Position;

            var width = gd.Viewport.Width;
            var height = gd.Viewport.Height;
            for (int i = w; i > 0; i--)
            {
                for (int j = h; j > 0; j--)
                {
                    var nearsource = new Vector3(width / i, height / j, 0.0f);
                    var farsource = new Vector3(width / i, height / j, 1.0f);
                    var world = Matrix.CreateTranslation(0, 0, 0);
                    var nearPoint = gd.Viewport.Unproject(nearsource, Projection, View, world);
                    var farPoint = gd.Viewport.Unproject(farsource, Projection, View, world);
                    var direction = farPoint - nearPoint;
                    direction.Normalize();
                    rays.Add(new Ray(nearPoint, direction)); 
                }
            }
            return new DRay(rays, Position);
        }

        public List<Ray> GetMouseRays1(GraphicsDevice gd, float wPres, float hPres)
        {
            List<Ray> rays = new List<Ray>();
            var MousePosition = Mouse.GetState().Position;

            var width = gd.Viewport.Width;
            var height = gd.Viewport.Height;
            var wcount = width / wPres;
            var hcount = height / hPres;
            for (int i = (int)-wcount; i < (int)wcount; i++)
                for (int j = (int)-hcount; j < (int)hcount; j++)
                {
                    Vector3 nearsource = new Vector3(MousePosition.X, MousePosition.Y, 0.0f) + new Vector3(i * 50f, j * 50, 0f);
                    Vector3 farsource = new Vector3(MousePosition.X, MousePosition.Y, 1.0f) + new Vector3(i * 50f, j * 50, 0f);
                    Matrix world = Matrix.CreateTranslation(0, 0, 0);
                    Vector3 nearPoint = gd.Viewport.Unproject(nearsource, Projection, View, world);
                    Vector3 farPoint = gd.Viewport.Unproject(farsource, Projection, View, world);
                    Vector3 direction = farPoint - nearPoint;
                    direction.Normalize();
                    rays.Add(new Ray(nearPoint, direction));
                }


            return rays;
        }

    }

    public struct DRay
    {
        public List<Ray> Rays;
        public Vector3 CamPosition;

        public DRay(List<Ray> rays, Vector3 camPosition)
        {
            Rays = rays;
            CamPosition = camPosition;
        }
    }
}
