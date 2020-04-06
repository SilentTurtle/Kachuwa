using System;
using System.Threading.Tasks;

namespace Kachuwa.Plugin
{
    
    public interface IPlugin
    {
        string SystemName { get; }
        Task<bool> Install();
        Task<bool> UnInstall();
        PluginConfig Configuration { get; set; }

    }

}