namespace Kachuwa.Caching
{
    public interface ICacheProvider
    {
        ICacheService Get(string name);
    }
}