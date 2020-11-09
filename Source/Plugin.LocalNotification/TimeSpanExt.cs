using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Plugin.LocalNotification
{
    public struct TimeSpanExt : IXmlSerializable
    {
        private System.TimeSpan _value;

        public static implicit operator TimeSpanExt(System.TimeSpan value)
        {
            return new TimeSpanExt { _value = value };
        }

        public static implicit operator System.TimeSpan(TimeSpanExt value)
        {
            return value._value;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            _value = System.TimeSpan.Parse(reader.ReadInnerXml());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(_value.ToString());
        }
    }
}