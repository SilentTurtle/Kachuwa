//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Xml;
//using System.Xml.Serialization;

//namespace Kachuwa.Web
//{
//    public class SitemapProvider: ISitemapProvider
//    {
//        public string CreateSitemap(HttpContextBase httpContext, IEnumerable<SitemapNode> nodes)
//        {
//            if (httpContext == null)
//            {
//                throw new ArgumentNullException("httpContext");
//            }

//            List<SitemapNode> nodeList = nodes != null ? nodes.ToList() : new List<SitemapNode>();
//            return CreateSitemapInternal(httpContext, nodeList);
//        }
//        private string CreateSitemapInternal(HttpContextBase httpContext, List<SitemapNode> nodes)
//        {
//            SitemapModel sitemap = new SitemapModel(nodes);
            
//            XmlSerializer ser = new XmlSerializer(typeof(SitemapModel));
           
//            var xml = "";
//            using (var sww = new StringWriter())
//            {
//                using (XmlWriter writer = XmlWriter.Create(sww))
//                {
//                    ser.Serialize(writer, sitemap);
//                    xml = sww.ToString(); // Your XML
//                }
//            }
//            return xml;
//            // return _sitemapActionResultFactory.CreateSitemapResult(httpContext, sitemap);
//            //return new XmlResult<SitemapModel>(sitemap);
//        }
//    }
//}