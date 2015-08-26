using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;

namespace MineLib.PGL.Screens.InGame.Light
{
    public sealed class PointLight : BaseLight
    {
        private static Effect _effect;
        private static Model _model;
        
        public float LightIntensity { get; set; }

        public float Radius
        {
            get { return _radius; }
            set
            {
                if (_radius >= 0)
                    _radius = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
        private float _radius;

        public PointLight(Client game, Vector3 position, float radius, Color color, float intensity = 1.0f, bool castShadows = true, bool canFlicker = false)
            : base(game, position, color, castShadows, canFlicker)
        {
            _radius = radius;
            LightIntensity = intensity;

            if(_effect == null)
                _effect = Game.Content.Load<Effect>("Effects/PointLight");

            if (_model == null)
                _model = Game.Content.Load<Model>("Models/PointLight");
        }

        public PointLight(Client game, Vector3 position, float radius, float intensity = 1.0f, bool castShadows = true, bool canFlicker = false)
            : this(game, position, radius, Color.White, intensity, castShadows, canFlicker) { }


        public override void UpdateLight(GameTime gameTime) { }

        public override void DrawShadowMap(Action<Matrix, Matrix> shadowMapDraw) { }

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
                // Set up effect parameters.
                _effect.Parameters["colorMap"].SetValue(gBuffer.ColorRT);
                _effect.Parameters["normalMap"].SetValue(gBuffer.NormalRT);
                _effect.Parameters["depthMap"].SetValue(gBuffer.DepthRT);

                // Light parameters.
                _effect.Parameters["lightPos"].SetValue(Position);
                _effect.Parameters["color"].SetValue(LightColor.ToVector3());
                _effect.Parameters["radius"].SetValue(Radius);
                _effect.Parameters["lightIntensity"].SetValue(LightIntensity);

                _effect.Parameters["eyePosition"].SetValue(camera.Position);
                _effect.Parameters["farClip"].SetValue(1000f);
                _effect.Parameters["inverseView"].SetValue(Matrix.Invert(camera.View));

                _effect.Parameters["halfPixel"].SetValue(halfPixel);

                var sphereMatrix = Matrix.CreateScale(_radius) * Matrix.CreateTranslation(Position);
                _effect.Parameters["World"].SetValue(sphereMatrix);
                _effect.Parameters["View"].SetValue(camera.View);
                _effect.Parameters["Projection"].SetValue(camera.Projection);

                // If the camera is inside the light volume, draw with CullMode.CullClockwise, else draw with CullMode.CullCounterClockwise.

                // Calculate the distance between the camera and light center
                var cameraToCenter = Vector3.Distance(camera.Position, Position);

                var before = GraphicsDevice.RasterizerState;

                if (cameraToCenter < Radius)
                    GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                else
                    GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                foreach (var mesh in _model.Meshes)
                    foreach (var meshPart in mesh.MeshParts)
                        foreach (var pass in _effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                            GraphicsDevice.Indices = meshPart.IndexBuffer;

                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                        }

                GraphicsDevice.RasterizerState = before;
            }

        }

        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime) { }

        public override void Dispose() { }
    }
}