using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Plugin
{
    public class KachuwaDirectoryContents : IDirectoryContents, IEnumerable<IFileInfo>, IEnumerable
    {
        private readonly IEnumerable<IFileInfo> _entries;

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public KachuwaDirectoryContents(IEnumerable<IFileInfo> entries)
        {
            if (entries == null)
                throw new ArgumentNullException("entries");
            this._entries = entries;
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return this._entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this._entries.GetEnumerator();
        }
    }
}