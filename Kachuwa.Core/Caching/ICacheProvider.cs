namespace Kachuwa.Caching
{
    public interface ICacheProvider
    {
        ICache Get(string name);
    }
}