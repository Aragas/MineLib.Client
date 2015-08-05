using System;
using System.Xml.Serialization;

namespace MineLib.PCL.Graphics.BMFont
{
    public class FontPage
    {
        [XmlAttribute("id")]
        public Int32 ID { get; set; }

        [XmlAttribute("file")]
        public String File { get; set; }
    }
}
