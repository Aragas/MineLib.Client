using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Extensions
{
    public static class Texture2DExtension
    {
        public static Texture2D Copy(this Texture2D texture)
        {
            var text = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);

            for (var i = 0; i < texture.LevelCount; i++)
            {
                var rawMipWidth = texture.Width / Math.Pow(2, i);
                var rawMipHeight = texture.Height / Math.Pow(2, i);

                // make sure that mipmap dimensions are always > 0.
                var mipWidth = (rawMipWidth < 1) ? 1 : (int) rawMipWidth;
                var mipHeight = (rawMipHeight < 1) ? 1 : (int) rawMipHeight;

                var mipData = new Color[mipWidth * mipHeight];
                texture.GetData(i, null, mipData, 0, mipData.Length);
                text.SetData(i, null, mipData, 0, mipData.Length);
            }

            return text;
        }
    }
}
