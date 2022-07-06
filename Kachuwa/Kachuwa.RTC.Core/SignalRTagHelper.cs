//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Html;
//using Microsoft.AspNetCore.Razor.TagHelpers;

//namespace Kachuwa.RTC
//{
//    public class SignalRTagHelper : TagHelperComponent
//    {

//        //order to inject first or last
//        public override int Order => 1;
//        public SignalRTagHelper()
//        {

//        }

//        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
//        {
//            if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
//            {
//                string signalRScripts = @"<script src='/lib/signalr/dist/signalr.min.js'></script>
//                    <script src ='/lib/signalr/signalr-protocol-msgpack/dist/signalr-protocol-msgpack.min.js'></script>";
//                output.PostContent.AppendHtmlLine(signalRScripts);
//            }

//            return Task.CompletedTask;
//        }
//    }
//}