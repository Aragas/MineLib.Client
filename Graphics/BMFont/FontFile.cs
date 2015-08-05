using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MineLib.PCL.Graphics.BMFont
{
    [XmlRoot("font")]
    public class FontFile : IDisposable
    {
        [XmlElement("info")]
        public FontInfo Info { get; set; }

        [XmlElement("common")]
        public FontCommon Common { get; set; }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<FontPage> Pages { get; set; }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<FontChar> Chars { get; set; }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<FontKerning> Kernings { get; set; }

        public void Dispose()
        {
            if(Pages != null)
                Pages.Clear();

            if (Chars != null)
                Chars.Clear();

            if (Kernings != null)
                Kernings.Clear();
        }
    }
}
