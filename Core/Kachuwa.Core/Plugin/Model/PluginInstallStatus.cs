namespace Kachuwa.Plugin
{
    public class PluginInstallStatus
    {
        public bool IsInstalled { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}