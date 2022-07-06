//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Globalization;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using Dapper;
//using Microsoft.AspNetCore.Mvc.Routing;

//namespace Kachuwa.Web
//{
//    public static class SiteMapService
//    {
//        public static IReadOnlyCollection<SitemapNode> GetSitemapNodes(UrlHelper urlHelper)
//        {
//            List<SitemapNode> nodes = new List<SitemapNode>();
//            //nodes.Add(
//            //    new SitemapNode()
//            //    {
//            //        Url = urlHelper.AbsoluteRouteUrl("index","Home"),
//            //        Priority = 1
//            //    });
//            nodes.Add(
//                new SitemapNode()
//                {
//                    Url = urlHelper.AbsoluteRouteUrl("Index","Search"),
//                    Priority = 0.9M
//                });
//            nodes.Add(
//                new SitemapNode()
//                {
//                    Url = urlHelper.AbsoluteRouteUrl("index","course"),
//                    Priority = 0.9M,
//                    Images = new List<SitemapImage>()
//                    {
//                        new SitemapImage("http://imgurl.com") 
//                    }
//                });
//            //nodes.Add(
//            //  new SitemapNode()
//            //  {
//            //      Url = urlHelper.AbsoluteRouteUrl("index", "course"),
//            //      Priority = 0.9M,
//            //      Video = new SitemapVideo("bar bar dekho","dersdfadf","http://asdf.com","http://youtube.com")
//            //  });
//            //foreach (int productId in productRepository.GetProductIds())
//            //{
//            //    nodes.Add(
//            //        new SitemapNode()
//            //        {
//            //            Url = urlHelper.AbsoluteRouteUrl("ProductGetProduct", new {id = productId}),
//            //            Frequency = SitemapFrequency.Weekly,
//            //            Priority = 0.8
//            //        });
//            //}

//            return nodes;
//        }
//        public static string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
//        {
//            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
//            XElement root = new XElement(xmlns + "urlset");
//            foreach (SitemapNode sitemapNode in sitemapNodes)
//            {
//                XElement urlElement = new XElement(
//                    xmlns + "url",
//                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
//                    sitemapNode.LastModificationDate == null ? null : new XElement(
//                        xmlns + "lastmod",
//                        sitemapNode.LastModificationDate.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
//                    sitemapNode.ChangeFrequency == null ? null : new XElement(
//                        xmlns + "changefreq",
//                        sitemapNode.ChangeFrequency.Value.ToString().ToLowerInvariant()),
//                    sitemapNode.Priority == null ? null : new XElement(
//                        xmlns + "priority",
//                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
//                root.Add(urlElement);
//            }
//            XDocument document = new XDocument(root);
//            return document.ToString();
//        }


//        //public static async Task<IEnumerable<SitemapNode>> GenerateProducts()
//        //{
//        //    var dbfactory = DbFactoryProvider.GetFactory();
//        //    using (var db = (SqlConnection)dbfactory.GetConnection())
//        //    {
//        //        await db.OpenAsync();
//        //    var products=   await db.QueryAsync("Select * from dbo.Product Where IsActive=1 and IsDeleted=0");
//        //    }
//        //} 

//        //incontroller
//        //[Route("sitemap.xml")]
//        //public ActionResult SitemapXml()
//        //{
//        //    var sitemapNodes = GetSitemapNodes(this.Url);
//        //    string xml = GetSitemapDocument(sitemapNodes);
//        //    return this.Content(xml, ContentType.Xml, Encoding.UTF8);
//        //}
//    }
//}