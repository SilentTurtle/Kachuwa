//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Html;
//using Microsoft.AspNetCore.Razor.TagHelpers;

//namespace Kachuwa.RTC
//{  // services.AddSingleton<ITagHelperComponent, LicenseTagHelper>();
//    public class RTCNotificationTagHelper : TagHelperComponent
//    {

//        //order to inject first or last
//        public override int Order => 22;
//        public RTCNotificationTagHelper()
//        {

//        }

//        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
//        {

//            //if (string.Equals(context.TagName, "body", StringComparison.Ordinal))
//            //{
//            //    const string script = @"<script src=""/jslogger.js""></script>";
//            //    output.PostContent.AppendHtmlLine(script);
//            //}
//            if (string.Equals(context.TagName, "body", StringComparison.Ordinal))
//            {


//                string analyticsSnippet = @"<script src='/dist/rtc.js'> </script>";


//                //PreElement
//                //PreContent
//                //Content
//                //PostContent
//                //PostElement
//                //IsContentModified
//                //Attributes

//                output.PostContent.AppendHtmlLine(analyticsSnippet);
//            }

//            return Task.CompletedTask;
//        }
//    }
//}