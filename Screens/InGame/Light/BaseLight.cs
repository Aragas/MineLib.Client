using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;

namespace MineLib.PGL.Screens.InGame.Light
{
    abstract public class BaseLight : MineLibComponent
    {
        public static int NumberLights = 0;

        protected bool NeedUpdate;


        /// <summary>
        /// Gets or sets a value that indicates wheter the light cast shadows.
        /// </summary>
        public RenderTarget2D ShadowMap
        {
            get { return _shadowMap; }
            protected set { }
        }
        private static RenderTarget2D _shadowMap;

        /// <summary>
        /// Gets or sets a value that indicates wheter the light cast shadows.
        /// </summary>
        public bool CastShadows
        {
            get { return _castShadows; }
            set
            {
                _castShadows = value;
                if (value == false)
                {
                    _shadowMap.Dispose();
                    _shadowMap = null;
                }
            }
        }
        private bool _castShadows;

        /// <summary>
        /// Gets or sets a value that indicates wheter the light can flickers.
        /// </summary>
        public bool CanFlicker { get; set; }

        /// <summary>
        /// Gets or sets the position of the light.
        /// </summary>
        public Vector3 Position { get { return _position; } set { _position = value; NeedUpdate = true; } }
        private Vector3 _position;

        /// <summary>
        /// Gets or sets the color of the light.
        /// </summary>
        public Color LightColor { get; set; }


        public BaseLight(Client game, Vector3 pos, Color color, bool castShadows, bool canFlicker) : base(game)
        {
            _position = pos;
            _castShadows = castShadows;
            CanFlicker = canFlicker;
            LightColor = color;
            NumberLights++;

            if(_shadowMap == null)
                _shadowMap = new RenderTarget2D(GraphicsDevice, 1024, 1024, true, SurfaceFormat.HalfVector2, DepthFormat.Depth24);
        }


        public abstract void DrawLight(PlayerClient gBuffer, CameraComponent camera, QuadRenderer quadRenderer, Vector2 halfPixel);

        public abstract void UpdateLight(GameTime gameTime);

        public abstract void DrawShadowMap(Action<Matrix, Matrix> shadowMapDraw);
    }
}