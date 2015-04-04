using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MineLib.Client.Graphics.Data;

namespace MineLib.Client.Graphics.Map
{
	public static class BlockVBO
	{
        /// <summary>
        /// Build visible blocks that don't have any light
        /// </summary>
	    public const bool BuildWithoutLight = false;

		public const float Size = 0.5f;

	    private const float TextureScale	= 1f / 16f;
		private const float ColorScale		= 1f / 32f;

	    public static List<VertexPositionColorHalfTexture> CubeFull(BlockRenderInfo renderSide)
	    {
            var list = new List<VertexPositionColorHalfTexture>();
            list.AddRange(CubeFaceFront(renderSide));
            list.AddRange(CubeFaceBack(renderSide));
            list.AddRange(CubeFaceTop(renderSide));
            list.AddRange(CubeFaceBottom(renderSide));
            list.AddRange(CubeFaceLeft(renderSide));
            list.AddRange(CubeFaceRight(renderSide));

	        return list;
	    }

		public static List<VertexPositionColorHalfTexture> CubeFaceFront(BlockRenderInfo renderSide)
        {
			var indicies = new List<int>();
			var verticles = new List<Vector3>();
            var normals = new List<Color>();
            var textcoords = new List<Vector2>();

            #region Text
            var texture = new Vector2();
            switch (renderSide.Block.ID)
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
            #endregion Text

            #region Verticies
            var topLeftBack = renderSide.Position + new Vector3(-1.0f, 1.0f, -1.0f) * Size;    // f
            var topLeftFront = renderSide.Position + new Vector3(-1.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack = renderSide.Position + new Vector3(1.0f, 1.0f, -1.0f) * Size;    // f
            var topRightFront = renderSide.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack = renderSide.Position + new Vector3(-1.0f, -1.0f, -1.0f) * Size;    // f
            var btmLeftFront = renderSide.Position + new Vector3(-1.0f, -1.0f, 1.0f) * Size;    // b
            var btmRightBack = renderSide.Position + new Vector3(1.0f, -1.0f, -1.0f) * Size;    // f
            var btmRightFront = renderSide.Position + new Vector3(1.0f, -1.0f, 1.0f) * Size;    // b
            #endregion Verticies

            #region Textures
            var textureTopLeft = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;
            #endregion Textures

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

			textcoords.Add(textureTopRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);

			#region Normals
			var t = renderSide.Block.Light * ColorScale + renderSide.Block.SkyLight * ColorScale;
	        if (t > 1f)
		        t = 1f;
            if (t == 0 && !BuildWithoutLight)
		        return new List<VertexPositionColorHalfTexture>();

			var normal = HsvToRgb(0, 0, t);

			//var normal1 = new Color(t, t, t, 255);
			//var normal = Color.(Color.White, t);
			//var normal = new Color(255, 255, 255, 255);

			//if (renderSide.Block.Light > 10)
			//    t = 1;

			normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            #endregion Normals

            var list = new List<VertexPositionColorHalfTexture>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionColorHalfTexture(verticles[i], normals[i], textcoords[i]));

            return list;
	    }

        public static List<VertexPositionColorHalfTexture> CubeFaceBack(BlockRenderInfo renderSide)
        {
			var indicies = new List<int>();
			var verticles = new List<Vector3>();
            var normals = new List<Color>();
            var textcoords = new List<Vector2>();

            #region Text
            var texture = new Vector2();
            switch (renderSide.Block.ID)
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
            #endregion Text

            #region Verticies
            var topLeftBack = renderSide.Position + new Vector3(-1.0f, 1.0f, -1.0f) * Size;    // f
            var topLeftFront = renderSide.Position + new Vector3(-1.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack = renderSide.Position + new Vector3(1.0f, 1.0f, -1.0f) * Size;    // f
            var topRightFront = renderSide.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack = renderSide.Position + new Vector3(-1.0f, -1.0f, -1.0f) * Size;    // f
            var btmLeftFront = renderSide.Position + new Vector3(-1.0f, -1.0f, 1.0f) * Size;    // b
            var btmRightBack = renderSide.Position + new Vector3(1.0f, -1.0f, -1.0f) * Size;    // f
            var btmRightFront = renderSide.Position + new Vector3(1.0f, -1.0f, 1.0f) * Size;    // b
            #endregion Verticies

            #region Textures
            var textureTopLeft = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;
            #endregion Textures

            verticles.Add(topLeftBack);
            verticles.Add(btmLeftBack);
            verticles.Add(topRightBack);

            verticles.Add(btmLeftBack);
            verticles.Add(btmRightBack);
            verticles.Add(topRightBack);

            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureTopRight);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);

			#region Normals
			var t = renderSide.Block.Light * ColorScale + renderSide.Block.SkyLight * ColorScale;
			if (t > 1f)
				t = 1f;
            if (t == 0 && !BuildWithoutLight)
                return new List<VertexPositionColorHalfTexture>();

			var normal = HsvToRgb(0, 0, t);

			normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            #endregion Normals

            var list = new List<VertexPositionColorHalfTexture>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionColorHalfTexture(verticles[i], normals[i], textcoords[i]));

            return list;
        }

        public static List<VertexPositionColorHalfTexture> CubeFaceTop(BlockRenderInfo renderSide)
        {
			var indicies = new List<int>();
			var verticles = new List<Vector3>();
            var normals = new List<Color>();
            var textcoords = new List<Vector2>();

            #region Text
            var texture = new Vector2();
            switch (renderSide.Block.ID)
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
            #endregion Text

            #region Verticies
            var topLeftBack = renderSide.Position + new Vector3(-1.0f, 1.0f, -1.0f) * Size;    // f
            var topLeftFront = renderSide.Position + new Vector3(-1.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack = renderSide.Position + new Vector3(1.0f, 1.0f, -1.0f) * Size;    // f
            var topRightFront = renderSide.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack = renderSide.Position + new Vector3(-1.0f, -1.0f, -1.0f) * Size;    // f
            var btmLeftFront = renderSide.Position + new Vector3(-1.0f, -1.0f, 1.0f) * Size;    // b
            var btmRightBack = renderSide.Position + new Vector3(1.0f, -1.0f, -1.0f) * Size;    // f
            var btmRightFront = renderSide.Position + new Vector3(1.0f, -1.0f, 1.0f) * Size;    // b
            #endregion Verticies

            #region Textures
            var textureTopLeft = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;
            #endregion Textures

            verticles.Add(topLeftBack);
            verticles.Add(topRightFront);
            verticles.Add(topLeftFront);
            verticles.Add(topLeftBack);
            verticles.Add(topRightBack);
            verticles.Add(topRightFront);

            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureTopRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);

			#region Normals
			var t = renderSide.Block.Light * ColorScale + renderSide.Block.SkyLight * ColorScale;
			if (t > 1f)
				t = 1f;
            if (t == 0 && !BuildWithoutLight)
                return new List<VertexPositionColorHalfTexture>();

			var normal = HsvToRgb(0, 0, t);

			normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            #endregion Normals

            var list = new List<VertexPositionColorHalfTexture>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionColorHalfTexture(verticles[i], normals[i], textcoords[i]));

            return list;
        }

        public static List<VertexPositionColorHalfTexture> CubeFaceBottom(BlockRenderInfo renderSide)
        {
			var indicies = new List<int>();
			var verticles = new List<Vector3>();
            var normals = new List<Color>();
            var textcoords = new List<Vector2>();

            #region Text
            var texture = new Vector2();
            switch (renderSide.Block.ID)
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
            #endregion Text

            #region Verticies
            var topLeftBack = renderSide.Position + new Vector3(-1.0f, 1.0f, -1.0f) * Size;    // f
            var topLeftFront = renderSide.Position + new Vector3(-1.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack = renderSide.Position + new Vector3(1.0f, 1.0f, -1.0f) * Size;    // f
            var topRightFront = renderSide.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack = renderSide.Position + new Vector3(-1.0f, -1.0f, -1.0f) * Size;    // f
            var btmLeftFront = renderSide.Position + new Vector3(-1.0f, -1.0f, 1.0f) * Size;    // b
            var btmRightBack = renderSide.Position + new Vector3(1.0f, -1.0f, -1.0f) * Size;    // f
            var btmRightFront = renderSide.Position + new Vector3(1.0f, -1.0f, 1.0f) * Size;    // b
            #endregion Verticies

            #region Textures
            var textureTopLeft = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;
            #endregion Textures

            verticles.Add(btmLeftBack);
            verticles.Add(btmLeftFront);
            verticles.Add(btmRightFront);
            verticles.Add(btmLeftBack);
            verticles.Add(btmRightFront);
            verticles.Add(btmRightBack);

            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);

			#region Normals
			var t = renderSide.Block.Light * ColorScale + renderSide.Block.SkyLight * ColorScale;
			if (t > 1f)
				t = 1f;
            if (t == 0 && !BuildWithoutLight)
                return new List<VertexPositionColorHalfTexture>();

			var normal = HsvToRgb(0, 0, t);

			normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            #endregion Normals

            var list = new List<VertexPositionColorHalfTexture>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionColorHalfTexture(verticles[i], normals[i], textcoords[i]));

            return list;
        }

        public static List<VertexPositionColorHalfTexture> CubeFaceLeft(BlockRenderInfo renderSide)
        {
			var indicies = new List<int>();
			var verticles = new List<Vector3>();
            var normals = new List<Color>();
            var textcoords = new List<Vector2>();

            #region Text
            var texture = new Vector2();
            switch (renderSide.Block.ID)
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
            #endregion Text

            #region Verticies
            var topLeftBack = renderSide.Position + new Vector3(-1.0f, 1.0f, -1.0f) * Size;    // f
            var topLeftFront = renderSide.Position + new Vector3(-1.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack = renderSide.Position + new Vector3(1.0f, 1.0f, -1.0f) * Size;    // f
            var topRightFront = renderSide.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack = renderSide.Position + new Vector3(-1.0f, -1.0f, -1.0f) * Size;    // f
            var btmLeftFront = renderSide.Position + new Vector3(-1.0f, -1.0f, 1.0f) * Size;    // b
            var btmRightBack = renderSide.Position + new Vector3(1.0f, -1.0f, -1.0f) * Size;    // f
            var btmRightFront = renderSide.Position + new Vector3(1.0f, -1.0f, 1.0f) * Size;    // b
            #endregion Verticies

            #region Textures
            var textureTopLeft = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;
            #endregion Textures

            verticles.Add(topLeftBack);
            verticles.Add(btmLeftFront);
            verticles.Add(btmLeftBack);
            verticles.Add(topLeftFront);
            verticles.Add(btmLeftFront);
            verticles.Add(topLeftBack);

            textcoords.Add(textureTopRight);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureTopRight);

			#region Normals
			var t = renderSide.Block.Light * ColorScale + renderSide.Block.SkyLight * ColorScale;
			if (t > 1f)
				t = 1f;
            if (t == 0 && !BuildWithoutLight)
                return new List<VertexPositionColorHalfTexture>();

			var normal = HsvToRgb(0, 0, t);

			normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            #endregion Normals

            var list = new List<VertexPositionColorHalfTexture>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionColorHalfTexture(verticles[i], normals[i], textcoords[i]));

            return list;
        }

        public static List<VertexPositionColorHalfTexture> CubeFaceRight(BlockRenderInfo renderSide)
        {
	        var indicies = new List<int>();
            var verticles = new List<Vector3>();
            var normals = new List<Color>();
            var textcoords = new List<Vector2>();

            #region Text
            var texture = new Vector2();
            switch (renderSide.Block.ID)
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
            #endregion Text

            #region Verticies
            var topLeftBack = renderSide.Position + new Vector3(-1.0f, 1.0f, -1.0f) * Size;    // f
            var topLeftFront = renderSide.Position + new Vector3(-1.0f, 1.0f, 1.0f) * Size;    // b
            var topRightBack = renderSide.Position + new Vector3(1.0f, 1.0f, -1.0f) * Size;    // f
            var topRightFront = renderSide.Position + new Vector3(1.0f, 1.0f, 1.0f) * Size;    // b
            var btmLeftBack = renderSide.Position + new Vector3(-1.0f, -1.0f, -1.0f) * Size;    // f
            var btmLeftFront = renderSide.Position + new Vector3(-1.0f, -1.0f, 1.0f) * Size;    // b
            var btmRightBack = renderSide.Position + new Vector3(1.0f, -1.0f, -1.0f) * Size;    // f
            var btmRightFront = renderSide.Position + new Vector3(1.0f, -1.0f, 1.0f) * Size;    // b
            #endregion Verticies

            #region Textures
            var textureTopLeft = new Vector2(1.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureTopRight = new Vector2(0.0f, 0.0f) * TextureScale + texture * TextureScale;
            var textureBottomLeft = new Vector2(1.0f, 1.0f) * TextureScale + texture * TextureScale;
            var textureBottomRight = new Vector2(0.0f, 1.0f) * TextureScale + texture * TextureScale;
            #endregion Textures

            verticles.Add(topRightBack);
            verticles.Add(btmRightBack);
            verticles.Add(btmRightFront);
            verticles.Add(topRightFront);
            verticles.Add(topRightBack);
            verticles.Add(btmRightFront);

            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomLeft);
            textcoords.Add(textureBottomRight);
            textcoords.Add(textureTopRight);
            textcoords.Add(textureTopLeft);
            textcoords.Add(textureBottomRight);

			#region Normals
			var t = renderSide.Block.Light * ColorScale + renderSide.Block.SkyLight * ColorScale;
			if (t > 1f)
				t = 1f;
            if (t == 0 && !BuildWithoutLight)
                return new List<VertexPositionColorHalfTexture>();

			var normal = HsvToRgb(0, 0, t);

			normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            #endregion Normals

            var list = new List<VertexPositionColorHalfTexture>();
            for (int i = 0; i < verticles.Count; i++)
                list.Add(new VertexPositionColorHalfTexture(verticles[i], normals[i], textcoords[i]));

            return list;
        }


        private static Color HsvToRgb(double h, double s, double v)
		{
			// ######################################################################
			// T. Nathan Mundhenk
			// mundhenk@usc.edu
			// C/C++ Macro HSV to RGB

			double H = h;
			while (H < 0) { H += 360; };
			while (H >= 360) { H -= 360; };
			double R, G, B;
			if (v <= 0)
			{ R = G = B = 0; }
			else if (s <= 0)
			{
				R = G = B = v;
			}
			else
			{
				double hf = H / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = v * (1 - s);
				double qv = v * (1 - s * f);
				double tv = v * (1 - s * (1 - f));
				switch (i)
				{

					// Red is the dominant color

					case 0:
						R = v;
						G = tv;
						B = pv;
						break;

					// Green is the dominant color

					case 1:
						R = qv;
						G = v;
						B = pv;
						break;
					case 2:
						R = pv;
						G = v;
						B = tv;
						break;

					// Blue is the dominant color

					case 3:
						R = pv;
						G = qv;
						B = v;
						break;
					case 4:
						R = tv;
						G = pv;
						B = v;
						break;

					// Red is the dominant color

					case 5:
						R = v;
						G = pv;
						B = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						R = v;
						G = tv;
						B = pv;
						break;
					case -1:
						R = v;
						G = pv;
						B = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						R = G = B = v; // Just pretend its black/white
						break;
				}
			}
			return new Color(Clamp((int)(R * 255.0)), Clamp((int)(G * 255.0)), Clamp((int)(B * 255.0)));
		}

		private static int Clamp(int i)
		{
			if (i < 0) return 0;
			if (i > 255) return 255;
			return i;
		}
	}
}