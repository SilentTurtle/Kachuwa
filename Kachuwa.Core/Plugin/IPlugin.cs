namespace Kachuwa.Plugin
{
    public interface IPlugin
    {
        string SystemName { get; }
        bool Install();
        bool UnInstall();
        PluginConfig Configuration { get; set; }

    }
}