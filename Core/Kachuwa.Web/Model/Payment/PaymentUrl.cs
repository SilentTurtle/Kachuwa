using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using Kachuwa.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Payment
{
    public class PaymentUrl
    {
        public static bool UseHttps = false;
        public static string GetReturnUrl(string paymentgateway)
        {
            string host = GetHost();
            string url = string.Format("/Payment/Success/{0}", paymentgateway);
            return host + url;
        }

        public static string GetCancelUrl()
        {
            //redirect to home page
            string host = GetHost();
            string url = "/"; //string.Format("/Payment/Cancel/{0}", paymentgateway);
            return host + url;
        }

        public static string GetVerification(string paymentgateway)
        {
            string host = GetHost();
            string url = string.Format("/Payment/Verify/{0}", paymentgateway);
            return host + url;
        }

        private static string GetHost()
        {
            var snap_kachuwaConfig = ContextResolver.Context.RequestServices.GetService<IOptionsSnapshot<KachuwaAppConfig>>();
            var kachuwaConfig = snap_kachuwaConfig.Value;
            // int index = HttpContext.Current.Request.Url.ToString().IndexOf(":", StringComparison.Ordinal);
            string host = ContextResolver.Context.Request.Host.Value.ToString();


            host = kachuwaConfig.SiteUrl;

            ////Uri myuri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            //Uri myuri = new Uri(HttpHelper.HttpContext.Request.Host.Value);
            //string requested = myuri.Scheme + Uri.SchemeDelimiter + myuri.Host + ":" + myuri.Port;
            //string pathQuery = myuri.PathAndQuery;
            //string hostName = myuri.ToString().Replace(pathQuery, "");
            //string path = "";
            //    // HttpRuntime.AppDomainAppVirtualPath == @"/" ? "/" : HttpRuntime.AppDomainAppVirtualPath + "/";
            //if (hostName.StartsWith("http"))
            //    return hostName;
            //else
            //    return "http://" + hostName;
            return host;
        }

        public static NameValueCollection ParseQueryString(string s)
        {
            NameValueCollection nvc = new NameValueCollection();
            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }
            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");
                if (singlePair.Length == 2)
                {
                    nvc.Add(singlePair[0], singlePair[1]);
                }
                else
                {
                    // only one key with no value specified in query string
                    nvc.Add(singlePair[0], string.Empty);
                }
            }
            return nvc;
        }
    }

    public interface IPaymentUrl
    {
        string GetReturnUrl(string paymentgateway);
        string GetCancelUrl();
        string GetVerification(string paymentgateway);
    }
}