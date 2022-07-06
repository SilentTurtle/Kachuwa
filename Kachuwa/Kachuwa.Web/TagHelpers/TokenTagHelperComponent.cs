//using System;
//using System.Threading.Tasks;
//using Kachuwa.Caching;
//using Kachuwa.Log;
//using Kachuwa.Web.Security;
//using Microsoft.AspNetCore.Html;
//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Microsoft.AspNetCore.Razor.TagHelpers;
//using Newtonsoft.Json;

//namespace Kachuwa.Web.TagHelpers
//{
//    public class TokenTagHelperComponent : TagHelperComponent
//    {
              
//        private readonly ILogger _logger;
//        private readonly ICacheService _cacheService;
//        private readonly ITokenGenerator _tokenGenerator;

//        //order to inject first or last
//        public override int Order => 1;
//        public TokenTagHelperComponent(ILogger logger, ICacheService cacheService, ITokenGenerator tokenGenerator)
//        {          
//            _logger = logger;
//            _cacheService = cacheService;
//            _tokenGenerator = tokenGenerator;

//        }

//        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
//        {

//            if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
//            {
//                try
//                {
//                    var token =await  _tokenGenerator.Generate();
//                    string script = "<script type='text/javascript'>" +
//                                    "try { var storTest = window['localStorage'];  storTest.setItem('token', '" + JsonConvert.SerializeObject(token) + "');}" +
//                                    "catch (e) { console.log('faile to store token'); }</script>";
//                    string ajaxCallToken = "<script type='text/javascript'>" +
//                                    @"var _kajax = function() {
//    var post = function(url, param, successFx, error) {
//        $.ajax({
//            type: 'POST',
//            beforeSend: function(xhr, settings) {
//                xhr.setRequestHeader('Authorization', 'Bearer ' + window['localStorage'].getItem('token'));
//            },
//            async: false,
//            url: url,
//            data: param,
//            success: successFx,
//            error: error
//        });
//    };
//    var get = function (url, param, successFx, error) {
//        $.ajax({
//            type: 'get',
//            beforeSend: function (xhr, settings) {
//                xhr.setRequestHeader('Authorization', 'Bearer ' + window['localStorage'].getItem('token'));
//            },
//            async: false,
//            url: url,
//            data: param,
//            success: successFx,
//            error: error
//        });
//    };
//    return { get: get, post: post };
//}();" + "</script>";
//                    output.PostContent.AppendHtmlLine($"{script} {ajaxCallToken}");
//                }
//                catch (Exception e)
//                {
//                    _logger.Log(LogType.Error, () => e.Message, e);
//                }

//            }
//        }

//    }
//}