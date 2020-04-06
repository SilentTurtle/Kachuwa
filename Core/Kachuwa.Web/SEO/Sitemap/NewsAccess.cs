using System.Xml.Serialization;

namespace Kachuwa.Web
{
    public enum NewsAccess
    {
        [XmlEnum]
        Subscription,

        [XmlEnum]
        Registration
    }
}