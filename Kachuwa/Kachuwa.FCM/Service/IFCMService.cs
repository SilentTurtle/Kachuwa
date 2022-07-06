using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.FCM
{
    public interface IFCMService
    {
        
          void SubscribeTopic(string token, string topic);

          void FcmSend(string token, string title, string message, string Clicek_Url, string Image_Uri);

        void FcmSend(string token, string title, string message);

        void FcmSend(string token, string title, string message, string Clicek_Url);

        void FcmSendTopic(string token, string title, string message, string Clicek_Url, string Image_Uri);

        void FcmSendTopic(string token, string title, string message, string Clicek_Url);
        void FcmSendTopic(string token, string title, string message);
    }
}
