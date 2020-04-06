using System;

namespace Kachuwa.Security
{
    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy _policy = new SecurityHeadersPolicy();

        public SecurityHeadersBuilder AddDefaultSecurePolicy()
        {
            AddFrameOptionsDeny();
            AddXssProtectionBlock();
            AddContentTypeOptionsNoSniff();
            //AddStrictTransportSecurityMaxAge();
            RemoveServerHeader();
            //http://www.dotnetnoob.com/2012/09/security-through-http-response-headers.html
            //https://blog.elmah.io/improving-security-in-asp-net-mvc-using-custom-headers/
        
            return this;
        }

        /// <summary>
        ///  prevent code injection attacks like cross-site scripting and 
        /// clickjacking, by telling the browser which dynamic resources that are allowed to load.
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddContentSecurity(Action<ContentSecurityPolicyBuiilder> builder)
        {
           
            var cspPolicy = new ContentSecurityPolicyBuiilder();
            builder(cspPolicy);
            _policy.AddNonce = cspPolicy.SupportNonce;
            _policy.CspBuiilder = cspPolicy;
           // _policy.SetHeaders["Content-Security-Policy"] = cspPolicy.Build();
                //"default-src *;" +
                //                                            "script-src 'self' 'nonce-" + nonce+"';"+
                //                                            "object-src 'self';" +
                //                                            "style-src 'self' 'unsafe-inline' 'unsafe-eval';" +
                //                                            "img-src 'self';" +
                //                                           // "data:assets-cdn.github.com identicons.github.com www.google-analytics.com ...;" +
                //                                            "media-src 'none';" +
                //                                            "child-src 'self';" +
                //                                            //"render.githubusercontent.com ;" +
                //                                           // "font-src assets-cdn.github.com;" +
                //                                            "connect-src 'self';" +
                //                                            "base-uri 'self';" +
                //                                            "form-action 'self';";
            return this;
        }

        /// <summary>
        ///  prevent code injection attacks like cross-site scripting and 
        /// clickjacking, by telling the browser which dynamic resources that are allowed to load.
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddContentSecurityAllowCDN(string[] cdnSites)
        {

            _policy.SetHeaders["Content-Security-Policy"] = "default-src 'self'" + " " + String.Join(" ", cdnSites);
            return this;
        }

        /// <summary>
        /// prevent any communication happening over HTTP
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddStrictTransportSecurityMaxAge()
        {
            _policy.SetHeaders["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            return this;
        }

        /// <summary>
        /// To avoid MIME type sniffin
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddContentTypeOptionsNoSniff()
        {
            _policy.SetHeaders["X-Content-Type-Options"] = "nosniff";
            return this;
        }
        /// <summary>
        /// stop loading the page when a cross-site scripting attack is detected
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddXssProtectionBlock()
        {

            _policy.SetHeaders["X-Xss-Protection"] = "1; mode=block";
            return this;
        }

        /// <summary>
        /// Deny all origin to load iframe
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddFrameOptionsDeny()
        {
            _policy.SetHeaders["X-Frame-Options"] = "DENY";
            return this;
        }
        /// <summary>
        /// allow iframing with in same origin
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersBuilder AddFrameOptionsSameOrigin()
        {
            _policy.SetHeaders["X-Frame-Options"] = "SAMEORIGIN";
            return this;
        }

        /// <summary>
        /// Allow other domains to load iframe
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public SecurityHeadersBuilder AddFrameOptionsSameOrigin(string uri)
        {
            _policy.SetHeaders["X-Frame-Options"] = "ALLOW-FROM " + uri;
            return this;
        }

        public SecurityHeadersBuilder RemoveServerHeader()
        {
            _policy.RemoveHeaders.Add("SERVER");
            return this;
        }

        public SecurityHeadersBuilder AddCustomHeader(string header, string value)
        {
            _policy.SetHeaders[header] = value;
            return this;
        }

        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            _policy.RemoveHeaders.Add(header);
            return this;
        }

        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }
}