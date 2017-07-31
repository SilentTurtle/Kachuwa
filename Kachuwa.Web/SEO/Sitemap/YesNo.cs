using System.Xml.Serialization;

namespace Kachuwa.Web
{
    public enum YesNo
    {
        None,

        [XmlEnum("yes")]
        Yes,

        [XmlEnum("no")]
        No
    }
}