using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json.Serialization;

namespace Kachuwa.RTC
{  // services.AddSingleton<ITagHelperComponent, LicenseTagHelper>();
    public class RTCNotificationTagHelper : TagHelperComponent
    {

        //order to inject first or last
        public override int Order => 1;
        public RTCNotificationTagHelper()
        {

        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            //if (string.Equals(context.TagName, "body", StringComparison.Ordinal))
            //{
            //    const string script = @"<script src=""/jslogger.js""></script>";
            //    output.PostContent.AppendHtmlLine(script);
            //}
            if (string.Equals(context.TagName, "body", StringComparison.Ordinal))
            {


                string analyticsSnippet = @"
            <script>
               console.log('todo rtc notification here');
            </script>";


                //PreElement
                //PreContent
                //Content
                //PostContent
                //PostElement
                //IsContentModified
                //Attributes

                output.PostContent.AppendHtmlLine(analyticsSnippet);
            }

            return Task.CompletedTask;
        }
    }
    public class SignalRContractResolver : IContractResolver
    {
        public JsonContract ResolveContract(Type type)
        {
            var signalRAssembly = typeof(Microsoft.AspNetCore.SignalR.DynamicHub).GetTypeInfo().Assembly;

            if (type.GetTypeInfo().Assembly.Equals(signalRAssembly))
            {
                var defaultContractSerializer = new DefaultContractResolver();
                return defaultContractSerializer.ResolveContract(type);
            }
            else
            {
                var camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
                return camelCaseContractResolver.ResolveContract(type);
            }
        }
    }
}