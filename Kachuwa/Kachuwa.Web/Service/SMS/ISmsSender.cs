using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Web.Services;
using Newtonsoft.Json;

namespace Kachuwa.Web.Service
{
    public static class PhoneNoHelper
    {
        public static string PhoneNumberWithoutCountry(this string phoneNumber)
        {

            if (phoneNumber.StartsWith("+977"))
            {
                return phoneNumber.Substring(4);
            }
            else if (phoneNumber.StartsWith("977"))
            {
                return phoneNumber.Substring(3);
            }
            else
            {
                if (phoneNumber.Length - 10 > 0) //taking last 10 number
                    return phoneNumber.Substring(phoneNumber.Length - 10);
                else return phoneNumber;
            }

            //return phoneNumber;
        }

        public static string CleanAlfaNum(this string smsSender)
        {

            if (smsSender.StartsWith("+977"))
            {
                return smsSender.Substring(4);
            }
            else if (smsSender.StartsWith("977"))
            {
                return smsSender.Substring(3);
            }

            return smsSender;
        }
        public static string ToAsteric(this string phoneNumber,int start,int end)
        {

            char[] chars = phoneNumber.ToCharArray();
            for(int i = start; i < end; i++)
            {
                chars[i] = '*';
            }

            return chars.ToString();
        }
    }

    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }

    public interface ISMSTemplateService
    {

    }

    public class SMSTemplateService: ISMSTemplateService
    {

    }
}
