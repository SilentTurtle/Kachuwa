namespace Kachuwa.Storage
{
    public class DefaultFileOptions : IFileOptions
    {
        public string Path { get; set; } = "Uploads";

        //TODO:: list all posible file uploads
        public FileType[] AllowedTypes { get; set; } = new FileType[]
        {
            new FileType(){ContentType = "image/png",FileExtension = "png"},
            new FileType(){ContentType = "image/x-citrix-png",FileExtension = "png"},
            new FileType(){ContentType = "image/x-png",FileExtension = "png"},
            new FileType(){ContentType = "image/jpeg",FileExtension = "jpeg"},
            new FileType(){ContentType = "image/jpeg",FileExtension = "jpg"},
            new FileType(){ContentType = "image/x-citrix-jpeg",FileExtension = "jpeg"},
            new FileType(){ContentType = "application/pdf",FileExtension = "pdf"},
            new FileType(){ContentType = "application/vnd.ms-excel",FileExtension = "xls"},
            new FileType(){ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation",FileExtension = "pptx"},
            new FileType(){ContentType = "application/zip",FileExtension = "zip"},
            new FileType(){ContentType = "application/octet-stream",FileExtension = "zip"},
            new FileType(){ContentType = "application/x-zip-compressed",FileExtension = "zip"}

            
        };
    }
}