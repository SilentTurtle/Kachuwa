

namespace Kachuwa.Web
{
    public class SendAsyncState
    {

        /// <summary>
        /// Contains all info that you need while handling message result
        /// </summary>
        public Email EmailInfo { get; private set; }


        public SendAsyncState(Email emailInfo)
        {
            EmailInfo = emailInfo;
        }
    }
}
