//using System.Collections.Generic;
//using System.IO;

//namespace Kachuwa.Storage
//{
//    public interface IFileRepository
//    {
//        void Persist(string id, int chunkNumber, byte[] buffer);
//        byte[] Read(string id, int chunkNumber);
//        void WriteToStream(Stream stream, FileSession session);
//        Stream GetFileStream(FileSession session);
//    }

//    public abstract partial class FileRepository : IFileRepository
//    {
//        public abstract void Persist(string id, int chunkNumber, byte[] buffer);

//        public abstract byte[] Read(string id, int chunkNumber);

//        public virtual void WriteToStream(Stream stream, FileSession session)
//        {
//            using (var sw = new BinaryWriter(stream))
//            {
//                for (int i = 1; i <= session.FileInfo.TotalNumberOfChunks; i++)
//                {
//                    sw.Write(Read(session.Id, i));
//                }
//            }

//            stream.Flush();
//        }

//        public virtual Stream GetFileStream(FileSession session)
//        {
//            return new ChunkedFileStream(this, session);
//        }
//    }

//    public class MemoryRepository : FileRepository
//    {
//        private Dictionary<string, Dictionary<int, byte[]>> internalStorage;

//        public MemoryRepository()
//        {
//            internalStorage = new Dictionary<string, Dictionary<int, byte[]>>();
//        }

//        public override void Persist(string id, int chunkNumber, byte[] buffer)
//        {
//            if (!internalStorage.ContainsKey(id))
//            {
//                internalStorage.Add(id, new Dictionary<int, byte[]>());
//            }

//            Dictionary<int, byte[]> blocks = internalStorage[id];
//            blocks.Add(chunkNumber, buffer);
//        }

//        public override byte[] Read(string id, int chunkNumber)
//        {
//            if (!internalStorage.ContainsKey(id))
//            {
//                throw new System.Exception("Session not found on internalStorage");
//            }

//            return internalStorage[id][chunkNumber];
//        }
//    }
//    public class LocalFileSystemRepository : FileRepository
//    {
//        string ROOT = "./files_store";
//        public async override void Persist(string id, int chunkNumber, byte[] buffer)
//        {
//            string chunkDestinationPath = Path.Combine(ROOT, id);

//            if (!Directory.Exists(chunkDestinationPath))
//            {
//                Directory.CreateDirectory(chunkDestinationPath);
//            }

//            string path = Path.Combine(ROOT, id, chunkNumber.ToString());
//            await File.WriteAllBytesAsync(path, buffer);
//        }

//        public override byte[] Read(string id, int chunkNumber)
//        {
//            string targetPath = Path.Combine(ROOT, id, chunkNumber.ToString());
//            return File.ReadAllBytes(targetPath);
//        }

//    }
//}