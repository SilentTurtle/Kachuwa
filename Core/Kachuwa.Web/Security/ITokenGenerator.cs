using System.Threading.Tasks;

namespace Kachuwa.Web.Security
{
    public interface ITokenGenerator
    {
        Task<object> Generate() ;
        string RequestAntiforgeryToken();
    }
}