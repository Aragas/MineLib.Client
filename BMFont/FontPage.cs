using System;
using System.Xml.Serialization;

namespace MineLib.PGL.BMFont
{
    public class FontPage
    {
        [XmlAttribute("id")]
        public Int32 ID { get; set; }

        [XmlAttribute("file")]
        public String File { get; set; }
    }
}
