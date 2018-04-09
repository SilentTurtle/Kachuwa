namespace Kachuwa.Web.Security
{
    public interface ITokenGenerator
    {
        string Generate();
        string RequestAntiforgeryToken();
    }
}