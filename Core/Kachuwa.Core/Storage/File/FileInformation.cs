using System;
using System.Collections.Generic;

namespace Kachuwa.Storage
{
    public class FileInformation
    {
        public virtual ISet<int> AlreadyPersistedChunks { get; private set; } = new HashSet<int>();

        public long FileSize { get; set; }

        public string FileName { get; set; }

        public int ChunkSize { get; set; }

        public FileInformation(long fileSize, String fileName, int chunkSize)
        {
            this.FileSize = fileSize;
            this.FileName = fileName;
            this.ChunkSize = chunkSize;
        }

        public virtual int TotalNumberOfChunks
        {
            get
            {
                return (int)Math.Ceiling(FileSize / (ChunkSize * 1F));
            }
        }

        public virtual void MarkChunkAsPersisted(int chunkNumber)
        {
            AlreadyPersistedChunks.Add(chunkNumber);
        }
    }
}