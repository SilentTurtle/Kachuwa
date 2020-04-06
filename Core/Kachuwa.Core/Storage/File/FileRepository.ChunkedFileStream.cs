using System;
using System.Collections.Generic;
using System.IO;

namespace Kachuwa.Storage
{
    
        public class ChunkedFileStream : Stream
        {
            private readonly IStorageProvider _storageProvider;
            public override long Position { get; set; }
            public override long Length
            {
                get
                {
                    return Session.FileInfo.FileSize;
                }
            }
            public override bool CanWrite { get; } = false;

            public override bool CanSeek { get; }
            public override bool CanRead { get; } = true;

            private Dictionary<long, byte[]> ChunkCache { get; set; } = new Dictionary<long, byte[]>();

            public ChunkedFileStream(IStorageProvider storageProvider, FileSession session)
            {
                _storageProvider = storageProvider;
                this.Session = session;

            }

            public FileSession Session { get; private set; }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var bytesRead = 0;

                for (int i = 0; i < count; i++)
                {
                    byte b;

                    if (TryReadByte(Position, out b))
                    {
                        buffer[i] = b;
                        Position++;
                        bytesRead++;
                    }
                    else
                    {
                        return bytesRead;
                    }
                }

                return bytesRead;
            }

            private bool TryReadByte(long byteIndex, out byte b)
            {
                b = 0;

                if (byteIndex >= Session.FileInfo.FileSize)
                    return false;

                // calculate chunk index by byte number
                long chunkNumber = (byteIndex / Session.FileInfo.ChunkSize) + 1;

                if (!ChunkCache.ContainsKey(chunkNumber))
                {
                    ChunkCache.Clear();
                    ChunkCache.Add(chunkNumber, _storageProvider.Read(Session.Id, (int)chunkNumber));
                }

                // get the i-th byte inside that chunk
                b = ChunkCache[chunkNumber][byteIndex % Session.FileInfo.ChunkSize];
                return true;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    
}