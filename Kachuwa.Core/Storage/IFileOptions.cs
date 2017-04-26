namespace Kachuwa.Storage
{
    public interface IFileOptions
    {
        string Path { get; set; }

        FileType[] AllowedTypes { get; set; }

        IKeyGenerator KeyGenerator { get; set; }

        IStorageProvider StorageProvider { get; set; }
    }

    public class DefaultFileOptions : IFileOptions
    {
        public DefaultFileOptions()
        {
            StorageProvider=new LocalStorageProvider(this.Path,"");
        }
        public string Path { get; set; }="";
        public FileType[] AllowedTypes { get; set; }=new FileType[]
        {
            new FileType(){ContentType = "",FileExtension = ""}, 
        };
        public IKeyGenerator KeyGenerator { get; set; }=new KeyGenerator();
        public IStorageProvider StorageProvider { get; set; }
    }
}