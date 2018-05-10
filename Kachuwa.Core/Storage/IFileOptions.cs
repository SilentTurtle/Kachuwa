namespace Kachuwa.Storage
{
    public interface IFileOptions
    {
        string Path { get; set; }

        FileType[] AllowedTypes { get; set; }

    }

    public class DefaultFileOptions : IFileOptions
    {
        public string Path { get; set; }="Uploads";

        //TODO:: list all posible file uploads
        public FileType[] AllowedTypes { get; set; }=new FileType[]
        {
            new FileType(){ContentType = "",FileExtension = ""}, 
        };
    }
}