using System.Xml.Serialization;

namespace Kachuwa.Web
{
    public enum SitemapFrequency
    {
        [XmlEnum("always")]
        Always,

        [XmlEnum("hourly")]
        Hourly,

        [XmlEnum("daily")]
        Daily,

        [XmlEnum("weekly")]
        Weekly,

        [XmlEnum("monthly")]
        Monthly,

        [XmlEnum("yearly")]
        Yearly,

        /// <summary>
        /// The value "never" should be used to describe archived URLs.
        /// </summary>
        [XmlEnum("never")]
        Never
    }
}