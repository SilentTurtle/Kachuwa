using System.Xml.Serialization;

namespace Kachuwa.Web
{
    public enum VideoPurchaseOption
    {
        None,
        
        [XmlEnum("rent")]
        Rent,

        [XmlEnum("own")]
        Own
    }
}