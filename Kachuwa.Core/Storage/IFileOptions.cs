namespace Kachuwa.Storage
{
    public interface IFileOptions
    {
        string Path { get; set; }

        FileType[] AllowedTypes { get; set; }

    }
}