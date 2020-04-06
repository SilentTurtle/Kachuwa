using System;
using System.Runtime.ConstrainedExecution;
using Newtonsoft.Json;

namespace Kachuwa.Storage
{
    [Serializable]
    public class FileSession
    {
        public string Id { get;  set; }

        public long User { get;  set; }

        public DateTime CreatedDate { get;  set; }
        private bool failed = false;

        public DateTime LastUpdate { get;  set; }

        public long Timeout { get;  set; }

        private static long DEFAULT_TIMEOUT = 3600L;


        public FileSession(long user, FileInformation fileInfo) : this(user, fileInfo, DEFAULT_TIMEOUT)
        {

        }
        [JsonConstructor]
        public FileSession(long user, FileInformation fileInfo, bool changeId = false)
        {
            if (changeId)
                this.Id = System.Guid.NewGuid().ToString();
            this.CreatedDate = DateTime.Now;
            this.LastUpdate = this.CreatedDate;
            this.User = user;
            this.FileInfo = fileInfo;
            this.Timeout = DEFAULT_TIMEOUT;
        }




        public FileSession(long user, FileInformation fileInformation, long timeout)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedDate = DateTime.Now;
            this.LastUpdate = this.CreatedDate;
            this.User = user;
            this.FileInfo = fileInformation;
            this.Timeout = timeout;
        }


        public double Progress
        {
            get
            {
                if (FileInfo.TotalNumberOfChunks == 0)
                    return 0;

                return SuccessfulChunks / (FileInfo.TotalNumberOfChunks * 1f);
            }
        }

        public String Status
        {
            get
            {
                if (failed)
                    return "failed";
                else if (IsConcluded)
                    return "done";

                return "ongoing";
            }
        }

        public bool IsConcluded
        {
            get
            {
                return FileInfo.TotalNumberOfChunks == FileInfo.AlreadyPersistedChunks.Count;
            }
        }


        public int SuccessfulChunks
        {
            get
            {
                return FileInfo.AlreadyPersistedChunks.Count;
            }
        }

        public bool HasFailed()
        {
            return failed;
        }

        public bool IsExpired
        {
            get
            {
                TimeSpan span = DateTime.Now - LastUpdate;
                return span.TotalSeconds >= Timeout;
            }
        }

        public void MaskAsFailed()
        {
            failed = true;
        }

        public FileInformation FileInfo
        {
            get;  set;
        }

        public void RenewTimeout()
        {
            LastUpdate = DateTime.Now;
        }
    }
}