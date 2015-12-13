//#define DUMMY_RENDER

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Aragas.Core.Wrappers;

using PCLStorage;

namespace MineLib.PGL.BMFont
{
#if DUMMY_RENDER
    public class FontRender
    {
        private const string FontXmlDataFileExtension = "fnt";

        public int Size { get; }

        private FontFile FontFile { get; }
        private Texture2D[] Textures { get; }

        private Dictionary<char, FontChar> GlyphMap { get; } = new Dictionary<char, FontChar>();

        public FontRender(GraphicsDevice graphicsDevice, string fileName, int size)
        {
            Size = size;


            #region File

            var deserializer = new XmlSerializer(typeof(FontFile));

            var content = FileSystemWrapper.ContentFolder;
            if (content.CheckExistsAsync("Fonts").Result != ExistenceCheckResult.FolderExists)
                throw new IOException(Path.Combine(content.Path, "Font"));

            var fontFolder = content.GetFolderAsync("Fonts").Result;

            if (fontFolder.CheckExistsAsync($"{fileName}{Size}.{FontXmlDataFileExtension}").Result == ExistenceCheckResult.FileExists)
                using (var stream = fontFolder.GetFileAsync($"{fileName}{Size}.{FontXmlDataFileExtension}").Result.OpenAsync(FileAccess.Read).Result)
                using (var textReader = new StreamReader(stream))
                    FontFile = (FontFile) deserializer.Deserialize(textReader);

            #endregion File


            #region Texture

            Textures = new Texture2D[FontFile.Pages.Count];
            foreach (var fontPage in FontFile.Pages)
            {
                if (fontFolder.CheckExistsAsync(fontPage.File).Result != ExistenceCheckResult.FileExists)
                    throw new IOException(Path.Combine(fontFolder.Path, fontPage.File));

                using (var stream = content.GetFolderAsync("Fonts").Result.GetFileAsync(fontPage.File).Result.OpenAsync(FileAccess.Read).Result)
                    Textures[fontPage.ID] = Texture2D.FromStream(graphicsDevice, stream);
            }

            #endregion Texture


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


        private static Texture2D _pointTexture;
        private static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }


        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var dx = borders.X;
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color);
                    dx += fc.XAdvance;
                }
            }
        }
        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            //DrawRectangle(spriteBatch, borders, Color.Red, 1);

            var size = MeasureText(text);

            var dx = borders.Center.X - (int) (size.X * 0.5f) - text.Length;
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color);
                    dx += fc.XAdvance;
                }
            }
        }

        public void Dispose()
        {
            GlyphMap?.Clear();

            FontFile?.Dispose();

            if(Textures != null)
                foreach (var texture in Textures)
                    texture.Dispose();
        }
    }
    public class FontRenderer1 : IDisposable
    {
        private FontRender[] FontRenders { get; }

        public FontRenderer1(GraphicsDevice graphicsDevice, string fileName, params int[] sizes)
        {
            FontRenders = new FontRender[sizes.Length];

            for (int i = 0; i < sizes.Length; i++)
                FontRenders[i] = new FontRender(graphicsDevice, fileName, sizes[i]);
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var mins = new float[FontRenders.Length];
            for (int i = 0; i < FontRenders.Length; i++)
            {
                var size = FontRenders[i].MeasureText(text);
                mins[i] = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            }

            var index = Array.IndexOf(mins, mins.OrderBy(item => Math.Abs(1.0f - item)).First());

            FontRenders[index].DrawText(spriteBatch, text, borders, color);
        }
        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var mins = new float[FontRenders.Length];
            for (int i = 0; i < FontRenders.Length; i++)
            {
                var size = FontRenders[i].MeasureText(text);
                mins[i] = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            }

            var index = Array.IndexOf(mins, mins.OrderBy(item => Math.Abs(1.0f - item)).First());

            FontRenders[index].DrawTextCentered(spriteBatch, text, borders, color);
        }

        public void Dispose()
        {
            foreach (var fontRender in FontRenders)
                fontRender?.Dispose();
        }
    }
#else
    public class FontRenderer : IDisposable
    {
        private const string FontXmlDataFileExtension = "fnt";

        private FontFile FontFile { get; }
        private Texture2D[] Textures { get; }

        private Dictionary<char, FontChar> GlyphMap { get; } = new Dictionary<char, FontChar>();
        private float FontScale { get; }

        public FontRenderer(GraphicsDevice graphicsDevice, string fileName, params int[] sizes)
        {

            #region File

            var deserializer = new XmlSerializer(typeof(FontFile));

            var content = FileSystemWrapper.ContentFolder;
            if (content.CheckExistsAsync("Fonts").Result != ExistenceCheckResult.FolderExists)
                throw new IOException(Path.Combine(content.Path, "Font"));

            var fontFolder = content.GetFolderAsync("Fonts").Result;

            if (fontFolder.CheckExistsAsync($"{fileName}.{FontXmlDataFileExtension}").Result == ExistenceCheckResult.FileExists)
                using (var stream = fontFolder.GetFileAsync($"{fileName}.{FontXmlDataFileExtension}").Result.OpenAsync(FileAccess.Read).Result)
                using (var textReader = new StreamReader(stream))
                    FontFile = (FontFile) deserializer.Deserialize(textReader);

            #endregion File


            #region Texture

            Textures = new Texture2D[FontFile.Pages.Count];
            foreach (var fontPage in FontFile.Pages)
            {
                if(fontFolder.CheckExistsAsync(fontPage.File).Result != ExistenceCheckResult.FileExists)
                    throw new IOException(Path.Combine(fontFolder.Path, fontPage.File));

                using (var stream = content.GetFolderAsync("Fonts").Result.GetFileAsync(fontPage.File).Result.OpenAsync(FileAccess.Read).Result)
                    Textures[fontPage.ID] = Texture2D.FromStream(graphicsDevice, stream);
            }

            #endregion Texture


            foreach (var glyph in FontFile.Chars)
                GlyphMap.Add((char) glyph.ID, glyph);

            FontScale = FontFile.Info.Size / 16f;
            //FontScale = FontFile.Info.Size / 8f;
            if (FontScale < 0)
                FontScale *= -1f;
        }

        private Vector2 MeasureText(string text)
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
        private float ScaleToMin(float value)
        {
            return (float) (Math.Round(value * FontScale, MidpointRounding.ToEven) / FontScale);
        }

        private static Texture2D _pointTexture;
        private static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            //DrawRectangle(spriteBatch, borders, Color.Red, 1);

            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = ScaleToMin(min);

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
                    dx += (int) (fc.XAdvance * scale);// + FontScale * scale);
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
                    dx += (int)(fc.XAdvance * scale + FontScale * scale);
                }
            }
        }

        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            //DrawRectangle(spriteBatch, borders, Color.Red, 1);

            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = ScaleToMin(min);
            //scale = min;

            var dx = borders.Center.X - (int) (size.X * scale * 0.5f) - text.Length;
            var dy = borders.Y;

            foreach (var c in text)
            {
                FontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    spriteBatch.Draw(Textures[fc.Page], position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    dx += (int) (fc.XAdvance * scale);// + FontScale * scale);
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
                    dx += (int) (fc.XAdvance * scale + FontScale * scale);
                }
            }
        }

        public void Dispose()
        {
            GlyphMap?.Clear();

            FontFile?.Dispose();

            if(Textures != null)
                foreach (var texture in Textures)
                    texture.Dispose();
        }
    }
#endif
}
