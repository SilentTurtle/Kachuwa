namespace Kachuwa.Localization
{
    public interface IResourceProvider
    {
        object GetResource(string name, string culture);
    }
}