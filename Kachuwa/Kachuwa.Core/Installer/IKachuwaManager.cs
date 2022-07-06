using System.Threading.Tasks;
using Kachuwa.Installer;

namespace Kachuwa.Installer
{
    public interface IKachuwaConfigurationManager
    {
        Task<bool> Install(string connectionString);
        Task<bool> Install(InstallerViewModel model);
        Task<bool> Unintall(string connectionString);
        Task<string> BackUpDb(string connectionString);
        Task<string> BackUpSystem();
    }
}