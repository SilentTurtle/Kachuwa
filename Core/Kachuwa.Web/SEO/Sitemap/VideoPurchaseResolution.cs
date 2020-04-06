using System.Xml.Serialization;

namespace Kachuwa.Web
{
    public enum VideoPurchaseResolution
    {
        None,
        
        [XmlEnum("hd")]
        Hd,

        [XmlEnum("sd")]
        Sd
    }
}