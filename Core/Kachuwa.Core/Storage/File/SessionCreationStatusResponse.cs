using System;

namespace Kachuwa.Storage
{
    /// <summary>
    /// Status of a session creation
    /// </summary>
    [Serializable]
    public class SessionCreationStatusResponse
    {
        public SessionCreationStatusResponse() { }
        public static SessionCreationStatusResponse fromSession(FileSession session)
        {
            return new SessionCreationStatusResponse
            {

                SessionId = session.Id,
                UserId = session.User,
                FileName = session.FileInfo.FileName
            };
        }

        /// <summary>
        /// File name
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// Session id
        /// </summary>
        public String SessionId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public long UserId { get; set; }
    }
}