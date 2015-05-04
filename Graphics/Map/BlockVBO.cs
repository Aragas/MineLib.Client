using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MineLib.PCL.Graphics.Data;

namespace MineLib.PCL.Graphics.Map
{
	public static class BlockVBO
	{
        public static bool BuildWithLight = true;

		public const float Size = 1f;

	    private const float TextureScale	= 1f / 16f;
		private const float ColorScale		= 1f / 32f;
        private const float SkyLight        = 1f / 16f;


        public static List<VertexPositionTextureLight> CubeFull(BlockRenderInfo renderInfo)
	    {
            var list = new List<VertexPositionTextureLight>();
            list.AddRange(CubeFaceFront(renderInfo));
            list.AddRange(CubeFaceBack(renderInfo));
            list.AddRange(CubeFaceTop(renderInfo));
            list.AddRange(CubeFaceBottom(renderInfo));
            list.AddRange(CubeFaceLeft(renderInfo));
            list.AddRange(CubeFaceRight(renderInfo));

	        return list;
	    }


        public static List<VertexPositionTextureLight> CubeFaceFront(BlockRenderInfo renderInfo)
        {
			var indicies = new List<int>();
			var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var topLeftFront    = renderInfo.Position + new Vector3(0.0f, 1.0f, 1.0f) * Size;    // b
            var topRightFront   = renderInfo.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftFront    = renderInfo.Position + new Vector3(0.0f, 0.0f, 1.0f) * Size;    // b
            var btmRightFront   = renderInfo.Position + new Vector3(1.0f, 0.0f, 1.0f) * Size;    // b

            verticles.Add(topLeftFront);	// v0-v1-v2
            verticles.Add(topRightFront);
            verticles.Add(btmLeftFront);
            //indicies.Add(0);
            //indicies.Add(1);
            //indicies.Add(2);

            verticles.Add(btmLeftFront);	// v2-v3-v0
            verticles.Add(topRightFront);
            verticles.Add(btmRightFront);
            //indicies.Add(2);
            //indicies.Add(3);
            //indicies.Add(0);
            #endregion Verticies


            #region Textures

            var texture = new Vector2();
            switch (renderInfo.Block.ID)
            {
                case 1:
                    texture = new Vector2(1, 0);
                    break;

                case 2:
                    texture = new Vector2(3, 0);
                    break;

                case 3:
                    texture = new Vector2(2, 0);
                    break;

                case 8:
                case 9:
                    texture = new Vector2(14, 0);
                    break;

                case 12:
                    texture = new Vector2(2, 1);
                    break;

                case 17:
                    texture = new Vector2(4, 1);
                    break;

                case 18:
                case 161:
                    texture = new Vector2(4, 3);
                    break;

                default:
                    texture = new Vector2(0, 1);
                    break;
            }

            var textureTopLeft      = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight     = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft   = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight  = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;

            textcoords.Add(textureTopRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            #endregion Textures

            
			#region Light
            var skyLight = renderInfo.Block.SkyLight * SkyLight;
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);

            var light = new Color(renderInfo.Block.Light, renderInfo.Block.Light, renderInfo.Block.Light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);

            #endregion Light


            var list = new List<VertexPositionTextureLight>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionTextureLight(verticles[i], textcoords[i], skyLights[i], lights[i]));

            return list;
	    }

        public static List<VertexPositionTextureLight> CubeFaceBack(BlockRenderInfo renderInfo)
        {
            var indicies = new List<int>();
            var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var topLeftBack     = renderInfo.Position + new Vector3(0.0f, 1.0f, 0.0f) * Size;    // f
            var topRightBack    = renderInfo.Position + new Vector3(1.0f, 1.0f, 0.0f) * Size;    // f
            var btmLeftBack     = renderInfo.Position + new Vector3(0.0f, 0.0f, 0.0f) * Size;    // f
            var btmRightBack    = renderInfo.Position + new Vector3(1.0f, 0.0f, 0.0f) * Size;    // f

            verticles.Add(topLeftBack);
            verticles.Add(btmLeftBack);
            verticles.Add(topRightBack);

            verticles.Add(btmLeftBack);
            verticles.Add(btmRightBack);
            verticles.Add(topRightBack);
            #endregion Verticies


            #region Textures
            var texture = new Vector2();
            switch (renderInfo.Block.ID)
            {
                case 1:
                    texture = new Vector2(1, 0);
                    break;

                case 2:
                    texture = new Vector2(3, 0);
                    break;

                case 3:
                    texture = new Vector2(2, 0);
                    break;

                case 8:
                case 9:
                    texture = new Vector2(14, 0);
                    break;

                case 12:
                    texture = new Vector2(2, 1);
                    break;

                case 17:
                    texture = new Vector2(4, 1);
                    break;

                case 18:
                case 161:
                    texture = new Vector2(4, 3);
                    break;

                default:
                    texture = new Vector2(0, 1);
                    break;
            }
            
            var textureTopLeft      = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight     = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft   = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight  = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;

            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureTopRight);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);
            #endregion Textures


            #region Light
            var skyLight = renderInfo.Block.SkyLight * SkyLight;
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);

            var light = new Color(renderInfo.Block.Light, renderInfo.Block.Light, renderInfo.Block.Light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);

            #endregion Light


            var list = new List<VertexPositionTextureLight>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionTextureLight(verticles[i], textcoords[i], skyLights[i], lights[i]));

            return list;
        }

        public static List<VertexPositionTextureLight> CubeFaceTop(BlockRenderInfo renderInfo)
        {
            var indicies = new List<int>();
            var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var topLeftBack     = renderInfo.Position + new Vector3(0.0f, 1.0f, 0.0f) * Size;    // f
            var topLeftFront    = renderInfo.Position + new Vector3(0.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack    = renderInfo.Position + new Vector3(1.0f, 1.0f, 0.0f) * Size;    // f
            var topRightFront   = renderInfo.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b

            verticles.Add(topLeftBack);
            verticles.Add(topRightFront);
            verticles.Add(topLeftFront);
            verticles.Add(topLeftBack);
            verticles.Add(topRightBack);
            verticles.Add(topRightFront);
            #endregion Verticies


            #region Textures
            var texture = new Vector2();
            switch (renderInfo.Block.ID)
            {
                case 1:
                    texture = new Vector2(1, 0);
                    break;

                case 2:
                    texture = new Vector2(0, 0);
                    break;

                case 3:
                    texture = new Vector2(2, 0);
                    break;

                case 8:
                case 9:
                    texture = new Vector2(14, 0);
                    break;

                case 12:
                    texture = new Vector2(2, 1);
                    break;

                case 17:
                    texture = new Vector2(4, 1);
                    break;

                case 18:
                case 161:
                    texture = new Vector2(4, 3);
                    break;

                default:
                    texture = new Vector2(0, 1);
                    break;
            }

            var textureTopLeft      = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight     = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft   = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight  = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;

            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureTopRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);
            #endregion Textures


            #region Light
            var skyLight = renderInfo.Block.SkyLight * SkyLight;
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);

            var light = new Color(renderInfo.Block.Light, renderInfo.Block.Light, renderInfo.Block.Light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);

            #endregion Light


            var list = new List<VertexPositionTextureLight>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionTextureLight(verticles[i], textcoords[i], skyLights[i], lights[i]));

            return list;
        }

        public static List<VertexPositionTextureLight> CubeFaceBottom(BlockRenderInfo renderInfo)
        {
            var indicies = new List<int>();
            var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var btmLeftBack     = renderInfo.Position + new Vector3(0.0f, 0.0f, 0.0f) * Size;    // f
            var btmLeftFront    = renderInfo.Position + new Vector3(0.0f, 0.0f, 1.0f) * Size;    // b
            var btmRightBack    = renderInfo.Position + new Vector3(1.0f, 0.0f, 0.0f) * Size;    // f
            var btmRightFront   = renderInfo.Position + new Vector3(1.0f, 0.0f, 1.0f) * Size;    // b

            verticles.Add(btmLeftBack);
            verticles.Add(btmLeftFront);
            verticles.Add(btmRightFront);
            verticles.Add(btmLeftBack);
            verticles.Add(btmRightFront);
            verticles.Add(btmRightBack);
            #endregion Verticies


            #region Textures
            var texture = new Vector2();
            switch (renderInfo.Block.ID)
            {
                case 1:
                    texture = new Vector2(1, 0);
                    break;

                case 2:
                    texture = new Vector2(2, 0);
                    break;

                case 3:
                    texture = new Vector2(2, 0);
                    break;

                case 8:
                case 9:
                    texture = new Vector2(14, 0);
                    break;

                case 12:
                    texture = new Vector2(2, 1);
                    break;

                case 17:
                    texture = new Vector2(4, 1);
                    break;

                case 18:
                case 161:
                    texture = new Vector2(4, 3);
                    break;

                default:
                    texture = new Vector2(0, 1);
                    break;
            }

            var textureTopLeft      = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight     = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft   = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight  = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;

            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);
            #endregion Textures


            #region Light
            var skyLight = renderInfo.Block.SkyLight * SkyLight;
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);

            var light = new Color(renderInfo.Block.Light, renderInfo.Block.Light, renderInfo.Block.Light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);

            #endregion Light


            var list = new List<VertexPositionTextureLight>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionTextureLight(verticles[i], textcoords[i], skyLights[i], lights[i]));

            return list;
        }

        public static List<VertexPositionTextureLight> CubeFaceLeft(BlockRenderInfo renderInfo)
        {
            var indicies = new List<int>();
            var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var topLeftBack     = renderInfo.Position + new Vector3(0.0f, 1.0f, 0.0f) * Size;    // f
            var topLeftFront    = renderInfo.Position + new Vector3(0.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack     = renderInfo.Position + new Vector3(0.0f, 0.0f, 0.0f) * Size;    // f
            var btmLeftFront    = renderInfo.Position + new Vector3(0.0f, 0.0f, 1.0f) * Size;    // b

            verticles.Add(topLeftBack);
            verticles.Add(btmLeftFront);
            verticles.Add(btmLeftBack);
            verticles.Add(topLeftFront);
            verticles.Add(btmLeftFront);
            verticles.Add(topLeftBack);
            #endregion Verticies


            #region Textures
            var texture = new Vector2();
            switch (renderInfo.Block.ID)
            {
                case 1:
                    texture = new Vector2(1, 0);
                    break;

                case 2:
                    texture = new Vector2(3, 0);
                    break;

                case 3:
                    texture = new Vector2(2, 0);
                    break;

                case 8:
                case 9:
                    texture = new Vector2(14, 0);
                    break;

                case 12:
                    texture = new Vector2(2, 1);
                    break;

                case 17:
                    texture = new Vector2(4, 1);
                    break;

                case 18:
                case 161:
                    texture = new Vector2(4, 3);
                    break;

                default:
                    texture = new Vector2(0, 1);
                    break;
            }

            var textureTopLeft      = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight     = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft   = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight  = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;

            textcoords.Add(textureTopRight);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureTopRight);
            #endregion Textures


            #region Light
            var skyLight = renderInfo.Block.SkyLight * SkyLight;
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);

            var light = new Color(renderInfo.Block.Light, renderInfo.Block.Light, renderInfo.Block.Light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);

            #endregion Light


            var list = new List<VertexPositionTextureLight>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionTextureLight(verticles[i], textcoords[i], skyLights[i], lights[i]));

            return list;
        }

        public static List<VertexPositionTextureLight> CubeFaceRight(BlockRenderInfo renderInfo)
        {
            var indicies = new List<int>();
            var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var topRightBack    = renderInfo.Position + new Vector3(1.0f, 1.0f, 0.0f) * Size;    // f
            var topRightFront   = renderInfo.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmRightBack    = renderInfo.Position + new Vector3(1.0f, 0.0f, 0.0f) * Size;    // f
            var btmRightFront   = renderInfo.Position + new Vector3(1.0f, 0.0f, 1.0f) * Size;    // b

            verticles.Add(topRightBack);
            verticles.Add(btmRightBack);
            verticles.Add(btmRightFront);
            verticles.Add(topRightFront);
            verticles.Add(topRightBack);
            verticles.Add(btmRightFront);
            #endregion Verticies


            #region Textures
            var texture = new Vector2();
            switch (renderInfo.Block.ID)
            {
                case 1:
                    texture = new Vector2(1, 0);
                    break;

                case 2:
                    texture = new Vector2(3, 0);
                    break;

                case 3:
                    texture = new Vector2(2, 0);
                    break;

                case 8:
                case 9:
                    texture = new Vector2(14, 0);
                    break;

                case 12:
                    texture = new Vector2(2, 1);
                    break;

                case 17:
                    texture = new Vector2(4, 1);
                    break;

                case 18:
                case 161:
                    texture = new Vector2(4, 3);
                    break;

                default:
                    texture = new Vector2(0, 1);
                    break;
            }

            var textureTopLeft      = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight     = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft   = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight  = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;

            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomRight);
            #endregion Textures


            #region Light
            var skyLight = renderInfo.Block.SkyLight * SkyLight;
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);
            skyLights.Add(skyLight);

            var light = new Color(renderInfo.Block.Light, renderInfo.Block.Light, renderInfo.Block.Light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);
            lights.Add(light);

            #endregion Light


            var list = new List<VertexPositionTextureLight>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionTextureLight(verticles[i], textcoords[i], skyLights[i], lights[i]));

            return list;
        }


        private static Vector3 CalcNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var V1 = (p2 - p1);
            var V2 = (p3 - p1);
            var surfaceNormal = new Vector3
            {
                X =   (V1.Y * V2.Z) - (V1.Z - V2.Y),
                Y = -((V2.Z * V1.X) - (V2.X * V1.Z)),
                Z =   (V1.X - V2.Y) - (V1.Y - V2.X)
            };


            // Dont forget to colorize if needed
            return surfaceNormal;
        }
	}
}