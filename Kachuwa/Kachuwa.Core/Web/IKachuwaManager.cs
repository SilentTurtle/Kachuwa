using System.Threading.Tasks;

namespace Kachuwa.Web
{
    public interface IKachuwaConfigurationManager
    {
        Task<bool> Install(string connectionString, string dbProvider);
        Task<bool> Unintall(string connectionString);
        Task<string> BackUpDb(string connectionString);
        Task<string> BackUpSystem();
        Task<bool> CheckConnection(string connectionString,string dbProvider);
    }
}