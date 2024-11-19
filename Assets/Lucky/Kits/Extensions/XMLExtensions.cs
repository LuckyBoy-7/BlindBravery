using System;
using System.Xml;

namespace Lucky.Kits.Extensions
{
    public static class XMLExtensions
    {
        public static float ToFloatByAttr(this XmlElement orig, string attr)
        {
            string s = orig.GetAttribute(attr);
            if (s == "")
                return 0;
            return (float)Convert.ToDouble(s);
        }
    }
}