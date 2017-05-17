using System.Threading.Tasks;

namespace Kachuwa.Web.Module
{
    public interface IScriptRunner
    {
        Task<bool> Run(string scripts);
    }
}