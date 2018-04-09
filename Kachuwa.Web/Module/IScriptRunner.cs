using System.Threading.Tasks;
using Kachuwa.Data.Crud;

namespace Kachuwa.Web.Module
{
    public interface IScriptRunner
    {
        Task<bool> Run(string scripts);
        Task<bool> Run(Dialect dialect, string connectionString, string scripts);
        Task<bool> CheckConnection(Dialect dialect, string connectionString);
    }
}