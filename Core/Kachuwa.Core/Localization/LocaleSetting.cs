namespace Kachuwa.Localization
{
    public class LocaleSetting
    {
        public string JsonResourceFileFormat = "locale-{0}.locale";//0=>culture
        public bool UseDbResources { get; set; } = true;
        public bool UseJsonResources { get; set; } = false;
        public bool UseXmlResources { get; set; } = false;
    }
}