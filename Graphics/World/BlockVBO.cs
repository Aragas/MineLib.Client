using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PCL.Graphics.Data;

namespace MineLib.PCL.Graphics.World
{
    public enum BlockFace
    {
        PositiveZ = 0,
        NegativeZ = 1,
        PositiveX = 2,
        NegativeX = 3,
        PositiveY = 4,
        NegativeY = 5
    }

	public static class BlockVBO
	{
        public static bool BuildWithLight = true;

		//public const float Size             = 1f;

	    public const float TextureScale	    = 1f / 16f;
		//private const float ColorScale		= 1f / 32f;
        private const float SkyLight        = 1f / 16f;

        private static readonly Vector3[][] CubeMesh;
        private static readonly Vector2[][] CubeTexture;
        static BlockVBO()
        {
            #region Vector3
            CubeMesh = new Vector3[6][];

            CubeMesh[0] = new[] // Positive Z face
            {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };

            CubeMesh[1] = new[] // Negative Z face
            {
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f)
            };

            CubeMesh[2] = new[] // Positive X face
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 1.0f)
            };

            CubeMesh[3] = new[] // Negative X face
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 0.0f)
            };

            CubeMesh[4] = new[] // Positive Y face
            {
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f)
            };

            CubeMesh[5] = new[] // Negative Y face
            {
                new Vector3(0.0f, 1.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 1.0f)
            };
            #endregion

            #region Vector2
            CubeTexture = new Vector2[7][];
            /*
            CubeTexture[0] = new[] // Positive Z face   // Front
            {
                new Vector2(1.0f, 1.0f) * TextureScale,
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale,
                new Vector2(1.0f, 0.0f) * TextureScale
            };

            CubeTexture[1] = new[] // Negative Z face   // Back
            {
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 0.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale
            };

            CubeTexture[2] = new[] // Positive X face
            {
                new Vector2(1.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 0.0f) * TextureScale,
                new Vector2(1.0f, 0.0f) * TextureScale
            };

            CubeTexture[3] = new[] // Negative X face
            {
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale
            };

            CubeTexture[4] = new[] // Positive Y face
            {
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 1.0f) * TextureScale
            };

            CubeTexture[5] = new[] // Negative Y face
            {
                new Vector2(1.0f, 0.0f) * TextureScale,
                new Vector2(1.0f, 0.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale
            };
            */

            CubeTexture[6] = new[]
            {
                new Vector2(0.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 1.0f) * TextureScale,
                new Vector2(1.0f, 0.0f) * TextureScale,
                new Vector2(0.0f, 0.0f) * TextureScale
            };

            #endregion
        }

	    public static T[] CreateQuadSide<T>(BlockRenderInfo block) where T : IVertexType
        {
            var shaderType = default(T);

            var unit = CubeMesh[(int)block.Face];
            var texture = CubeTexture[6];

            if (shaderType is VertexPositionTextureLight)
            {
                var quad = new VertexPositionTextureLight[4];
                for (var i = 0; i < 4; i++)
                    quad[i] = new VertexPositionTextureLight(block.Position + unit[i], texture[i] + block.Texture, block.Block.SkyLight * SkyLight, new Vector3(10));

                return quad as T[];
            }

            if (shaderType is VertexPositionTexture)
                {
                    var quad = new VertexPositionTexture[4];
                    for (var i = 0; i < 4; i++)
                        quad[i] = new VertexPositionTexture(block.Position + unit[i], texture[i] + block.Texture);

                    return quad as T[];
                }

            return null;
        }

        public static T[] CreateQuadSideTriagled<T>(BlockRenderInfo block) where T : IVertexType
        {
            var returnVertices = new T[6];
            var vertices = CreateQuadSide<T>(block);

            returnVertices[0] = vertices[0];
            returnVertices[1] = vertices[1];
            returnVertices[2] = vertices[3];

            returnVertices[3] = vertices[1];
            returnVertices[4] = vertices[2];
            returnVertices[5] = vertices[3];

            return returnVertices;
        }

        /*
        public static VertexPositionTextureLight[] CreateQuadSide(BlockRenderInfo block)
        {
            var unit = CubeMesh[(int) block.Face];
            var texture = CubeTexture[6];
            var quad = new VertexPositionTextureLight[4];
            for (var i = 0; i < 4; i++)
                quad[i] = new VertexPositionTextureLight(block.Position + unit[i], texture[i] + block.Texture, block.Block.SkyLight * SkyLight, new Vector3(10));

            return quad;
        }

        public static VertexPositionTextureLight[] CreateQuadSide(BlockRenderInfo block, int indiciesOffset, out int[] indicies)
        {
            indicies = new[] { 0, 1, 3, 1, 2, 3 };
            for (var i = 0; i < indicies.Length; i++)
                indicies[i] += ((int) block.Face * 4) + indiciesOffset;

            var unit = CubeMesh[(int) block.Face];
            var texture = CubeTexture[6];
            var quad = new VertexPositionTextureLight[4];
            for (var i = 0; i < 4; i++)
                quad[i] = new VertexPositionTextureLight(block.Position + unit[i], texture[i] + block.Texture, block.Block.SkyLight * SkyLight, new Vector3(10));
            
            return quad;
        }
        

        public static VertexPositionTextureLight[] CreateQuadSideTriagled(BlockRenderInfo block)
        {
            var returnVertices = new VertexPositionTextureLight[6];
            var vertices = CreateQuadSide(block);

            returnVertices[0] = vertices[0];
            returnVertices[1] = vertices[1];
            returnVertices[2] = vertices[3];

            returnVertices[3] = vertices[1];
            returnVertices[4] = vertices[2];
            returnVertices[5] = vertices[3];

            return returnVertices;
        }

        public static VertexPositionTextureLight[] CreateQuadSideTriagled(BlockRenderInfo block, int indiciesOffset, out int[] indicies)
        {
            var vertices = CreateQuadSide(block, indiciesOffset, out indicies);

            var returnVertices = new VertexPositionTextureLight[6];
            //returnVertices[0] = vertices[0];
            //returnVertices[1] = vertices[1];
            //returnVertices[2] = vertices[2];

            //returnVertices[3] = vertices[2];
            //returnVertices[4] = vertices[3];
            //returnVertices[5] = vertices[0];

            returnVertices[0] = vertices[0];
            returnVertices[1] = vertices[1];
            returnVertices[2] = vertices[3];

            returnVertices[3] = vertices[1];
            returnVertices[4] = vertices[2];
            returnVertices[5] = vertices[3];

            return returnVertices;
        }
        */

#if OLD

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
			var verticles = new List<Vector3>();
            var textcoords = new List<Vector2>();
            var skyLights = new List<float>();
            var lights = new List<Color>();


            #region Verticies
            var topLeftFront    = renderInfo.Position + new Vector3(0.0f, 1.0f, 1.0f) * Size;    // b
            var topRightFront   = renderInfo.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftFront    = renderInfo.Position + new Vector3(0.0f, 0.0f, 1.0f) * Size;    // b
            var btmRightFront   = renderInfo.Position + new Vector3(1.0f, 0.0f, 1.0f) * Size;    // b

            verticles.Add(topLeftFront);
            verticles.Add(topRightFront);
            verticles.Add(btmLeftFront);

            verticles.Add(btmLeftFront);
            verticles.Add(topRightFront);
            verticles.Add(btmRightFront);
            #endregion Verticies


            #region Textures
            var texture = Texture[renderInfo.Block.ID];
            if(texture == default(Vector2))
                texture = Texture[0];

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
#endif
	}
}