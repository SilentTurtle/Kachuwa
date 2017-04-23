namespace Kachuwa.Storage
{
    public interface IFileOptions
    {
        string Path { get; set; }

        FileType[] AllowedTypes { get; set; }

        IKeyGenerator KeyGenerator { get; set; }

        IStorageProvider StorageProvider { get; set; }
    }
}