using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core.Wrappers;

using PCLStorage;

namespace MineLib.PCL.Graphics.BMFont
{
    public class FontRenderer : IDisposable
    {
        private const string FontXmlDataFormat = ".fnt";
        private enum TextureFileFormats { png, dds, tga }

        private Dictionary<char, FontChar> GlyphMap { get; set; }
        private FontFile FontFile { get; set; }
        private Texture2D[] Textures { get; set; }

        public FontRenderer(GraphicsDevice graphicsDevice, string fileName)
        {

            #region File

            var deserializer = new XmlSerializer(typeof(FontFile));

            var content = FileSystemWrapper.ContentFolder;
            if (content.CheckExistsAsync("Fonts").Result != ExistenceCheckResult.FolderExists)
                throw new FileNotFoundException("Fonts folder missing");

            var fontFolder = content.GetFolderAsync("Fonts").Result;

            if (fontFolder.CheckExistsAsync(fileName + FontXmlDataFormat).Result == ExistenceCheckResult.FileExists)
                using (var stream = fontFolder.GetFileAsync(fileName + FontXmlDataFormat).Result.OpenAsync(FileAccess.Read).Result)
                using (var textReader = new StreamReader(stream))
                    FontFile = (FontFile) deserializer.Deserialize(textReader);

            #endregion File


            #region Texture

            var fileFormat = string.Empty;
            foreach (var format in Enum.GetNames(typeof(TextureFileFormats)))
                if (fontFolder.CheckExistsAsync(fileName + "_0" + "." + format).Result == ExistenceCheckResult.FileExists)
                { fileFormat = format; break; }

            var textures = new List<Texture2D>();
            var textureCount = 0;
            for (var i = 0; i < 10; i++)
                if (fontFolder.CheckExistsAsync(fileName + "_" + i + "." + fileFormat).Result != ExistenceCheckResult.FileExists)
                { textureCount = i; break; }
            

            for (var i = 0; i < textureCount; i++)
                using (var stream = content.GetFolderAsync("Fonts").Result.GetFileAsync(fileName + "_" + i + "." + fileFormat).Result.OpenAsync(FileAccess.Read).Result)
                    textures.Add(Texture2D.FromStream(graphicsDevice, stream));
            Textures = textures.ToArray();

            #endregion Texture

            
            GlyphMap = new Dictionary<char, FontChar>();

            foreach (var glyph in FontFile.Chars)
                GlyphMap.Add((char)glyph.ID, glyph);
        }

        public Vector2 MeasureText(string text)
        {
            var width = 0;
            var height = 0;
            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    width += fc.XAdvance + fc.XOffset;

                    if (fc.Height + fc.YOffset > height)
                        height = fc.Height + fc.YOffset;
                }
            }

            return new Vector2(width, height);
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale =  (float)Math.Floor(min * 2.0f) * 0.5f;

            var dx = borders.X;
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale + 2 * scale);
                }
            }
        }

        public void DrawTextStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.X;
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale + 2 * scale);
                }
            }
        }

        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = (float)Math.Floor(min * 2.0f) * 0.5f;

            var dx = borders.Center.X - (int)(size.X * scale * 0.5f) - (int)(2 * text.Length * 0.5f);
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int)(fc.XAdvance * scale + 2 * scale);
                }
            }
        }

        public void DrawTextCenteredStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.Center.X - (int)(size.X * scale * 0.5f) - (int)(2 * text.Length * 0.5f);
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int) (fc.XAdvance * scale + 2 * scale);
                }
            }
        }

        public void Dispose()
        {
            if(GlyphMap != null)
                GlyphMap.Clear();

            if(FontFile != null)
                FontFile.Dispose();

            if(Textures != null)
                foreach (var texture in Textures)
                    texture.Dispose();
        }
    }
}
