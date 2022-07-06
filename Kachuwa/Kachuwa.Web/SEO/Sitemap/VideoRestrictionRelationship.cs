using System.Xml.Serialization;

namespace Kachuwa.Web
{
    public enum VideoRestrictionRelationship
    {
        [XmlEnum("allow")]
        Allow,

        [XmlEnum("deny")]
        Deny
    }
}