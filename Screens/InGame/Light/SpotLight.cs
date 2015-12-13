using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;

namespace MineLib.PGL.Screens.InGame.Light
{
    public sealed class SpotLight : BaseLight
    {
        private static Effect _effect;
        private static Model _model;
        private static Texture2D _texture;


        /// <summary>
        /// Gets the projection matrix of the spot light.
        /// </summary>
        public Matrix Projection { get; protected set; }

        /// <summary>
        /// Sets the projection matrix of the spot light.
        /// </summary>
        public Matrix View { get; protected set; }

        /// <summary>
        /// Gets or sets the max distance that the spot light affects.
        /// </summary>
        public float MaxDistance { get { return _maxDistance; } set { _maxDistance = value; NeedUpdate = true; } }
        private float _maxDistance;

        /// <summary>
        /// Gets or sets the intensity of the spot light.
        /// </summary>
        public float LightIntensity { get; set; }

        /// <summary>
        /// Gets or sets the rate of decay of the spot light, which measures how the light intensity decreases from the center of the cone.
        /// </summary>
        public float DecayRate { get; set; }

        /// <summary>
        /// Gets or sets the angle of the spot light in radians. Must be positive and between 0 and 90 degrees.
        /// </summary>
        public float Angle
        {
            get { return (float)Math.Acos(_cosAngle); }
            set
            {
                if (MathHelper.ToDegrees(value) >= 0 && MathHelper.ToDegrees(value) <= 90)
                    _cosAngle = (float)Math.Cos(value);
                else throw new ArgumentOutOfRangeException();

                NeedUpdate = true;
            }
        }
        private float _cosAngle;

        /// <summary>
        /// Gets or sets the direction of the spot light.
        /// </summary>
        public Vector3 Direction { get { return Vector3.Normalize(_direction); } set { _direction = value; NeedUpdate = true; } }
        private Vector3 _direction;



        /// <summary>
        /// Creates a new spot light, specyfing position, direction, maximum distance, angle, diffuse color, intensity, rate of decay and wheter the light cast shadows and can flicker.
        /// </summary>
        /// <param name="position">Position of the spot light.</param>
        /// <param name="radius">Radius of the spot light.</param>
        /// <param name="color">Diffuse color of the spot light.</param>
        /// <param name="maxDistance">Maximum distance the spot light affects.</param>
        /// <param name="angle">Angle of the spot light in radians. Must be in the 0..Pi/2 range.</param>
        /// <param name="intensity">The intensity of the spot light. Default is 1.0f.</param>
        /// <param name="intensity">The decay rate of the spot light. Default is 1.0f.</param>
        /// <param name="castShadows">True if the spot light casts shadows, false otherwise. (default true)</param>
        /// <param name="canFlicker">True if the spot light can flicker, false otherwise. (default false)</param>
        public SpotLight(Client game, Vector3 position, Vector3 direction, float maxDistance, float angle, Color color, float intensity = 1.0f, float decayRate = 1f, bool castShadows = true, bool canFlicker = false)
            : base(game, position, color, castShadows, canFlicker)
        {
            if (_effect == null)
                _effect = Game.Content.Load<Effect>("Effects/SpotLight");

            if (_model == null)
                _model = Game.Content.Load<Model>("Models/PointLight");

            if (_texture == null)
                //texture = Game.Content.Load<Texture2D>("Textures/SpotRadialCookie");
                _texture = Game.Content.Load<Texture2D>("terrain");

            Projection = Matrix.Identity;
            View = Matrix.Identity;

            // If the angle is between 0 and 90 degrees, use it. Otherwise, set the angle at 90 degrees.
            if (MathHelper.ToDegrees(angle) <= 90 && MathHelper.ToDegrees(angle) >= 0)
                Angle = angle;
            else
                Angle = MathHelper.ToRadians(90f);

            _direction = direction;

            _maxDistance = maxDistance;

            DecayRate = decayRate;

            LightIntensity = intensity;

            UpdateMatrices();
        }

        /// <summary>
        /// Creates a new spot light, specyfing position, direction, angle, intensity, rate of decay and wheter the light cast shadows and can flicker.
        /// </summary>
        /// <param name="position">Position of the spot light.</param>
        /// <param name="radius">Radius of the spot light.</param>
        /// <param name="maxDistance">Maximum distance the spot light affects.</param>
        /// <param name="angle">Angle of the spot light in radians. Must be in the 0..Pi/2 range.</param>
        /// <param name="intensity">The intensity of the spot light. Default is 1.0f.</param>
        /// <param name="intensity">The decay rate of the spot light. Default is 1.0f.</param>
        /// <param name="castShadows">True if the spot light casts shadows, false otherwise. (default true)</param>
        /// <param name="canFlicker">True if the spot light can flicker, false otherwise. (default true)</param>
        public SpotLight(Client game, Vector3 position, Vector3 direction, float maxDistance, float angle, float intensity = 1.0f, float decayRate = 1f, bool castShadows = true, bool canFlicker = false)
            : this(game, position, direction, maxDistance, angle, Color.White, intensity, decayRate, castShadows, canFlicker) { }

        public override void UpdateLight(GameTime gameTime) { UpdateMatrices(); }

        public override void DrawShadowMap(Action<Matrix, Matrix> shadowMapDraw)
        {
            GraphicsDevice.SetRenderTarget(ShadowMap);

            // Set renderstates.
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            shadowMapDraw(View, Projection);
        }


        public void UpdateMatrices()
        {
            if (NeedUpdate)
            {
                // Target
                var target = (Position + Direction);
                if (target == Vector3.Zero)
                    target = -Vector3.Up;

                // Up
                var up = Vector3.Cross(_direction, Vector3.Up);
                up = up == Vector3.Zero ? Vector3.Right : Vector3.Up;

                // Create view and projection matrices.
                View = Matrix.CreateLookAt(Position, target, up);

                Projection = Matrix.CreatePerspectiveFieldOfView(Angle, 1.0f, 1.0f, MaxDistance);

                // Reset boolean.
                NeedUpdate = false;
            }
        }

        public override void DrawLight(PlayerClient gBuffer, CameraComponent camera, QuadRenderer quadRenderer, Vector2 halfPixel)
        {
            var drawLight = true;

            if (CanFlicker)
            {
                // Probability that the light won't be drawn.
                //drawLight = !RandomGen.CalculateProbability(LightGlobals.FlickerProbability);
                drawLight = true;
            }

            if (drawLight)
            {

                // Get the point light effect.
                _effect.CurrentTechnique = _effect.Techniques["SpotLight"];

                // Set up effect parameters.
                _effect.Parameters["colorMap"].SetValue(gBuffer.ColorRT);
                _effect.Parameters["normalMap"].SetValue(gBuffer.NormalRT);
                _effect.Parameters["depthMap"].SetValue(gBuffer.DepthRT);

                // Light parameters.
                _effect.Parameters["lightPos"].SetValue(Position);
                _effect.Parameters["color"].SetValue(LightColor.ToVector3());
                _effect.Parameters["lightDirection"].SetValue(Direction);
                _effect.Parameters["cosLightAngle"].SetValue(_cosAngle);
                _effect.Parameters["spotDecayExponent"].SetValue(DecayRate);
                _effect.Parameters["maxDistance"].SetValue(MaxDistance);
                _effect.Parameters["lightIntensity"].SetValue(LightIntensity);

                _effect.Parameters["lightViewProj"].SetValue(View * Projection);
                _effect.Parameters["cookieTexture"].SetValue(_texture);

                _effect.Parameters["eyePosition"].SetValue(camera.Position);
                _effect.Parameters["farClip"].SetValue(1000f);
                _effect.Parameters["inverseView"].SetValue(Matrix.Invert(camera.View));
                _effect.Parameters["inverseViewProj"].SetValue(Matrix.Invert(camera.View * camera.Projection));
                _effect.Parameters["halfPixel"].SetValue(halfPixel);

                _effect.Parameters["CastShadows"].SetValue(CastShadows);

                if (CastShadows)
                    _effect.Parameters["shadowMap"].SetValue(ShadowMap);
                

                Matrix coneMatrix;

                /*       
                Vector3 coneDirection = new Vector3(0,0,-1);
                float angle = (float)Math.Acos(Vector3.Dot(coneDirection,Direction));
                Vector3 axis = Vector3.Cross(coneDirection,Direction);

                float scale = (float)Math.Tan((double)Angle / 2.0) * 2 * MaxDistance;

                coneMatrix = Matrix.CreateScale(new Vector3(scale, scale, MaxDistance)) * Matrix.CreateFromAxisAngle(axis, angle) * Matrix.CreateTranslation(Position);  */


                //Make Scaling Factor
                float radial = (float)Math.Tan((double)Angle / 2.0) * 2 * MaxDistance;

                //Make Scaling Matrix
                Matrix Scaling = Matrix.CreateScale(radial, radial, MaxDistance);

                //Make Translation Matrix
                Matrix Translation = Matrix.CreateTranslation(Position.X, Position.Y, Position.Z);

                //Make Inverse View
                Matrix inverseView = Matrix.Invert(View);

                //Make Semi-Product
                Matrix semiProduct = Scaling * inverseView;

                //Decompose Semi-Product
                Vector3 S; Vector3 P; Quaternion Q;
                semiProduct.Decompose(out S, out Q, out P);

                //Make Rotation
                Matrix Rotation = Matrix.CreateFromQuaternion(Q);

                //Make World
                coneMatrix = Scaling * Rotation * Translation;


                _effect.Parameters["World"].SetValue(coneMatrix);
                _effect.Parameters["View"].SetValue(camera.View);
                _effect.Parameters["Projection"].SetValue(camera.Projection);


                //Calculate L
                Vector3 L = camera.Position - Position;

                //Calculate S.L
                float SL = Math.Abs(Vector3.Dot(L, Direction));

                //Check if SL is within the LightAngle, if so then draw the BackFaces, if not then draw the FrontFaces
                if (SL <= _cosAngle) GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                else GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            // Vertex and index buffers.
                            GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                            GraphicsDevice.Indices = meshPart.IndexBuffer;

                            // Draw the mesh part.
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                        }
                    }
                }

                // Reset the cull mode.
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            }
        }

        public override void Update(GameTime gameTime) { }
        public override void Draw(GameTime gameTime) { }

        public override void Dispose() { }
    }
}