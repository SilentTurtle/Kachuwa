namespace Kachuwa.Storage
{
    public interface IKeyGenerator
    {
        string GetKey();
        string GetKey(int size);
    }
}