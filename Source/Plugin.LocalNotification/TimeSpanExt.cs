using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public struct TimeSpanExt : IXmlSerializable, IEquatable<TimeSpanExt>
    {
        private TimeSpan _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TimeSpan(TimeSpanExt value)
        {
            return ToTimeSpan(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TimeSpanExt(TimeSpan value)
        {
            return ToTimeSpanExt(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(TimeSpanExt left, TimeSpanExt right)
        {
            return !(left == right);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(TimeSpanExt left, TimeSpanExt right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// CA2225: Operator overloads have named alternates
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(TimeSpanExt value)
        {
            return value._value;
        }

        /// <summary>
        /// CA2225: Operator overloads have named alternates
        /// </summary>
        /// <returns></returns>
        public static TimeSpanExt ToTimeSpanExt(TimeSpan value)
        {
            return new TimeSpanExt() { _value = value };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is TimeSpanExt other && Equals(other);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TimeSpanExt other)
        {
            return other._value.Equals(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var xmlValue = reader.ReadInnerXml();
            if (string.IsNullOrWhiteSpace(xmlValue))
            {
                throw new ArgumentNullException(nameof(reader));
            }

            _value = TimeSpan.Parse(xmlValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteValue(_value.ToString());
        }
    }
}