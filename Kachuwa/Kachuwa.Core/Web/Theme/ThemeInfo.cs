using System.Collections.Generic;

namespace Kachuwa.Web.Theme
{
    public class ThemeInfo
    {
        public string ThemeName { get; set; }
        public bool IsAdminTheme { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Framework { get; set; }
        public string FrameworkVersion { get; set; }
        public Dictionary<string, IEnumerable<string>> BundleJs { get; set; }
        public Dictionary<string, IEnumerable<string>> BundleCss { get; set; }
        public string ScssVariableFile { get; set; }
        public IEnumerable<string> Fonts { get; set; }
        public string Signature { get; set; }
       

    }
}