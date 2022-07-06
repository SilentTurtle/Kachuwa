using System.IO;

namespace Kachuwa.Storage
{
    public interface IFile
    {
        string ContentType { get; set; }
        Stream Stream { get; set; }
    }

    public class KachuwaFile: IFile
    {
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }
}