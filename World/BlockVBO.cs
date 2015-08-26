using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.Components;

namespace MineLib.PGL.World
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
	    private static readonly Vector3[] CubeNormals;

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

            CubeNormals = new[]
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0)
            };
        }

	    public static IVertexType[] CreateQuadSide(BlockRenderInfo block)
        {
            var quad = new IVertexType[4];

            var unit = CubeMesh[(int)block.Face];
            var texture = CubeTexture[6];
            
	        switch (WorldRendererComponent.ShaderType)
	        {
                case ShaderType.VertexPositionColorTexture:
	                for (var i = 0; i < 4; i++)
	                {
	                    var t = new Color(Color.White.ToVector3() * block.Block.SkyLight * SkyLight);
	                    quad[i] = new VertexPositionColorTexture(block.Position + unit[i], t, texture[i] + block.Texture);
	                }
	                break;

                case ShaderType.VertexPositionTexture:
                    for (var i = 0; i < 4; i++)
	                    quad[i] = new VertexPositionTexture(block.Position + unit[i], texture[i] + block.Texture);
	                break;

	            case ShaderType.VertexPositionTextureLight:
	                for (var i = 0; i < 4; i++)
	                    quad[i] = new VertexPositionTextureLight(block.Position + unit[i], texture[i] + block.Texture, block.Block.SkyLight * SkyLight);
	                break;

                case ShaderType.VertexPositionNormalTextureLight:
                    for (var i = 0; i < 4; i++)
                        quad[i] = new VertexPositionNormalTextureLight(block.Position + unit[i], CubeNormals[(int)block.Face],  texture[i] + block.Texture, block.Block.SkyLight * SkyLight);
                    break;

                case ShaderType.VertexPositionNormalTextureTangentBinormal:
                case ShaderType.Deferred:
                    for (var i = 0; i < 4; i++)
                        quad[i] = new VertexPositionNormalTextureTangentBinormal(block.Position + unit[i], CubeNormals[(int)block.Face], texture[i] + block.Texture);
                    break;

                case ShaderType.VertexPositionNormalTextureTangentBinormalLight:
                    for (var i = 0; i < 4; i++)
                        quad[i] = new VertexPositionNormalTextureTangentBinormalLight(block.Position + unit[i], CubeNormals[(int)block.Face], texture[i] + block.Texture, block.Block.SkyLight * SkyLight);
                    break;

                default:
                    throw new Exception("CreateQuadSide: " + WorldRendererComponent.ShaderType + "not implemented");
	        }

	        return quad;
        }

        public static IVertexType[] CreateQuadSideTriagled(BlockRenderInfo block)
        {
            var returnVertices = new IVertexType[6];
            var vertices = CreateQuadSide(block);

            returnVertices[0] = vertices[0];
            returnVertices[1] = vertices[1];
            returnVertices[2] = vertices[3];

            returnVertices[3] = vertices[1];
            returnVertices[4] = vertices[2];
            returnVertices[5] = vertices[3];

            if (WorldRendererComponent.ShaderType == ShaderType.VertexPositionNormalTextureTangentBinormal || WorldRendererComponent.ShaderType == ShaderType.Deferred)
            {
                var triangle1 = CalculateTangentBinormal(block.Face,
                    new[] { (IVertexTypeTangentBinormal) returnVertices[0],
                            (IVertexTypeTangentBinormal) returnVertices[1],
                            (IVertexTypeTangentBinormal) returnVertices[2] });

                returnVertices[0] = (VertexPositionNormalTextureTangentBinormal)triangle1[0];
                returnVertices[1] = (VertexPositionNormalTextureTangentBinormal)triangle1[1];
                returnVertices[2] = (VertexPositionNormalTextureTangentBinormal)triangle1[2];


                var triangle2 = CalculateTangentBinormal(block.Face,
                    new[] { (IVertexTypeTangentBinormal) returnVertices[3],
                            (IVertexTypeTangentBinormal) returnVertices[4],
                            (IVertexTypeTangentBinormal) returnVertices[5] });

                returnVertices[3] = (VertexPositionNormalTextureTangentBinormal)triangle2[0];
                returnVertices[4] = (VertexPositionNormalTextureTangentBinormal)triangle2[1];
                returnVertices[5] = (VertexPositionNormalTextureTangentBinormal)triangle2[2];
            }

            if (WorldRendererComponent.ShaderType == ShaderType.VertexPositionNormalTextureTangentBinormalLight)
            {
                var triangle1 = CalculateTangentBinormal(block.Face,
                    new[] { (IVertexTypeTangentBinormal) returnVertices[0],
                            (IVertexTypeTangentBinormal) returnVertices[1],
                            (IVertexTypeTangentBinormal) returnVertices[2] });

                returnVertices[0] = (VertexPositionNormalTextureTangentBinormalLight) triangle1[0];
                returnVertices[1] = (VertexPositionNormalTextureTangentBinormalLight) triangle1[1];
                returnVertices[2] = (VertexPositionNormalTextureTangentBinormalLight) triangle1[2];


                var triangle2 = CalculateTangentBinormal(block.Face, 
                    new[] { (IVertexTypeTangentBinormal) returnVertices[3],
                            (IVertexTypeTangentBinormal) returnVertices[4],
                            (IVertexTypeTangentBinormal) returnVertices[5] });

                returnVertices[3] = (VertexPositionNormalTextureTangentBinormalLight) triangle2[0];
                returnVertices[4] = (VertexPositionNormalTextureTangentBinormalLight) triangle2[1];
                returnVertices[5] = (VertexPositionNormalTextureTangentBinormalLight) triangle2[2];
            }

            return returnVertices;
        }

	    private static IVertexTypeTangentBinormal[] CalculateTangentBinormal(BlockFace face, IVertexTypeTangentBinormal[] vertices)
	    {
            var returnVertices = new IVertexTypeTangentBinormal[3];

            var vert1 = vertices[0];
            var vert2 = vertices[1];
            var vert3 = vertices[2];

            var normal = CubeNormals[(int) face];

            // This is a triangle from your vertices  
            var v1 = vert1.Position;
            var v2 = vert2.Position;
            var v3 = vert3.Position;

            // These are the texture coordinate of the triangle  
            var w1 = vert1.TextureCoordinate;
            var w2 = vert2.TextureCoordinate;
            var w3 = vert3.TextureCoordinate;

            var x1 = v2.X - v1.X;
            var x2 = v3.X - v1.X;
            var y1 = v2.Y - v1.Y;
            var y2 = v3.Y - v1.Y;
            var z1 = v2.Z - v1.Z;
            var z2 = v3.Z - v1.Z;

            var s1 = w2.X - w1.X;
            var s2 = w3.X - w1.X;
            var t1 = w2.Y - w1.Y;
            var t2 = w3.Y - w1.Y;

            var r = 1.0f / (s1 * t2 - s2 * t1);
            var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            // Gram-Schmidt orthogonalize  
            var tangent = sdir - normal * Vector3.Dot(normal, sdir);
            tangent.Normalize();

            // Calculate handedness (here maybe you need to switch >= with <= depend on the geometry winding order)  
            var tangentdir = (Vector3.Dot(Vector3.Cross(normal, sdir), tdir) >= 0.0f) ? 1.0f : -1.0f;
            var binormal = Vector3.Cross(normal, tangent) * tangentdir;

            vert1.Tangent = tangent;
            vert2.Tangent = tangent;
            vert3.Tangent = tangent;

            vert1.Binormal = binormal;
            vert2.Binormal = binormal;
            vert3.Binormal = binormal;

            returnVertices[0] = vert1;
            returnVertices[1] = vert2;
            returnVertices[2] = vert3;

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