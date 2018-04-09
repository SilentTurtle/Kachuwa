using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Configuration;
using Kachuwa.Data;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MXTires.Microdata.Core;
using MXTires.Microdata.Core.Intangible;
using MXTires.Microdata.Core.Intangible.StructuredValues;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web
{
    public interface ISeoService
    {
        CrudService<SEO> Seo { get; set; }
        Task<bool> CheckUrlExist(string url, string type);
        Task<SEO> GetDetailByUrl(string url, string type);
        Task<string> GenerateForProduct(SEO productInfo);
        Task<string> GenerateMetaContents();
        Task<SEO> GetByProductId(int productId);
        string GetPage();
        Task<SEO> GetBySeoType(string seoType, int id);
    }


    public class SeoService : ISeoService
    {
        private readonly ISettingService _settingService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private IUrlHelper _urlHelper;
        private Setting _setting;
        private KachuwaAppConfig _kachuwaConfig;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public IUrlHelperFactory _urlHelperFactory { get; set; }
        public SeoService(ISettingService settingService, IActionContextAccessor actionContextAccessor)
        {
           
            _settingService = settingService;
            _actionContextAccessor = actionContextAccessor;

        }

        public CrudService<SEO> Seo { get; set; } = new CrudService<SEO>();


        private string Path { get; set; }

        private async Task<SEO> GetPageContents(string url)
        {
            return await Seo.GetAsync("Where url=@url", new { url });
        }


        public async Task<SEO> GetByProductId(int productId)
        {
            var dbfactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbfactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryFirstAsync<SEO>("Select * from Seo Where SeoType=@SeoType and ProductId=@ProductId", new { SeoType = "product", ProductId = productId });

            }
        }

        public string GetPage()
        {
            if (ContextResolver.Context.Items.Keys.Contains("KPageUrl"))
            {
                var url = ContextResolver.Context.Items["KPageUrl"];
                return url.ToString();
            }
            else
            {
                string actionName = ContextResolver.Context.GetRouteValue("action").ToString();
                string controllerName = ContextResolver.Context.GetRouteValue("controller").ToString();
                return _urlHelper.Action(actionName, controllerName);

            }

        }

        public async Task<string> GenerateMetaContents()
        {
            try
            {
                var actionContext = _actionContextAccessor.ActionContext;
                _urlHelper = new UrlHelper(actionContext);
                _setting = _settingService.CrudService.Get(1);
                // var requestContext = ContextResolver.Context.Request.RequestContext;
                // _urlHelper = new UrlHelper(requestContext);

                string path = GetPage();

                //if (path.StartsWith("/products/"))
                //{
                //    path = path.Replace("/products", "");
                //}
                StringBuilder metatags = new StringBuilder();
                var page = await GetPageContents(path);
                if (page == null)
                    return "";
                //if (page.SeoType == "product")
                //{
                //    return GenerateForProduct(page);
                //}
                else
                {
                    var title = new TitleTag(new Dictionary<string, string>
                    {
                        {"title", page.MetaTitle}
                    });
                    metatags.Append(title.Generate());
                    var normalMetatag = new MetaTag(new Dictionary<string, string>
                    {
                        {"title", page.MetaTitle},
                        {"name", page.MetaTitle},
                        {"description", page.MetaDescription},
                        {"image", _setting.Logo}
                    });
                    metatags.Append(normalMetatag.Generate());
                    var openGrap = new OgMetaTag(new Dictionary<string, string>
                    {
                        //{"type", "page"},
                       
                        {"type", "website"},
                        {"title", page.MetaTitle},
                        {"description", page.MetaDescription},
                        //{"type", "article"},//product
                        {"locale", "en_US"}
                        ,
                        {"image", _setting.Logo}
                        // {"url", _urlHelper.AbsoluteRouteUrl(page.Url)},

                    }, "926715557465044");
                    metatags.Append(openGrap.Generate());

                    var twitter = new TwitterMetaTag(new Dictionary<string, string>
                    {
                        {"card", "summary"},
                        {"title", page.MetaTitle},
                        {"description", page.MetaDescription}
                        ,
                       {"image", _setting.Logo}
                    });
                    metatags.Append(twitter.Generate());
                    return metatags.ToString() + GenerateJsonLdForWebSite(page);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> CheckUrlExist(string url, string type)//1=page
        {


            var dbfactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbfactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.ExecuteScalarAsync<int>(
                      "IF (EXISTS(SELECT 1 FROM SEO WHERE SEOType = @SEOType AND Url = @Url))" +
                      "BEGIN Select 1;END ELSE Begin Select 0 ;End "
                      , new
                      {
                          Url = url,
                          SEOType = type
                      });
                return result == 1;
            }
        }


        public async Task<string> GenerateForProduct(SEO seoProduct)
        {
            try
            {

                //string path = GetPage();
                //if (path.StartsWith("/products/"))
                //{
                //    path = path.Replace("/products", "");
                //}
                //
                //var seoProduct = GetPageContents(path);
                //if (seoProduct == null)
                //    return "";
                StringBuilder metatags = new StringBuilder();
                var seoImage = GetProductImageForSeo(seoProduct.ProductId);

                var title = new TitleTag(new Dictionary<string, string>
                {
                    {"title", seoProduct.MetaTitle}
                });
                metatags.Append(title.Generate());
                var normalMetatag = new MetaTag(new Dictionary<string, string>
                {
                    {"name", seoProduct.MetaTitle},
                    {"description", seoProduct.MetaDescription}
                });
                metatags.Append(normalMetatag.Generate());
                var openGrap = new OgMetaTag(new Dictionary<string, string>
                {

                    {"type", "product"},
                    {"title", seoProduct.MetaTitle},
                    {"description", seoProduct.MetaDescription},
                    {"image", seoImage ?? ""},//TODO
                    //{"type", "article"},//product
                    {"locale", "en_US"}
                    // {"url", _urlHelper.AbsoluteRouteUrl(page.Url)},

                }, "926715557465044");
                metatags.Append(openGrap.Generate());

                var twitter = new TwitterMetaTag(new Dictionary<string, string>
                {
                    {"card", "summary"},
                    {"title", seoProduct.MetaTitle},
                    {"description", seoProduct.MetaDescription},
                    {"image", seoImage ?? ""}
                });
                metatags.Append(twitter.Generate());

                var x = metatags.ToString();
                // var y = GenerateJsonLdForProduct(seoProduct.ProductId);
                return x;//+ y;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<SEO> GetBySeoType(string seoType, int id)
        {
            var dbfactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbfactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryFirstAsync<SEO>("Select * from Seo Where SeoType=@SeoType and ProductId=@ProductId", new { SeoType = seoType, ProductId = id });

            }
        }

        private string GetProductImageForSeo(int productId)
        {
            //var dbfactory = DbFactoryProvider.GetFactory();
            //var prodImage = "";
            //using (var db = (SqlConnection)dbfactory.GetConnection())
            //{
            //    prodImage = db.ExecuteScalar<string>("Select RootImage from Product where ProductId=@ProductId",
            //        new { ProductId = productId });
            //}
            //if (string.IsNullOrEmpty(prodImage))
            //    return "";
            //return _urlHelper.Content(prodImage, true);
            return "";
        }

        public string GenerateJsonLdForWebSite(SEO page)
        {
            var appConfig = ContextResolver.Context.RequestServices.GetService<IOptionsSnapshot<KachuwaAppConfig>>();
            _kachuwaConfig = appConfig.Value;
            var storeInfo = _setting;
            var site = new WebSite();
            site.AlternateName = storeInfo.WebsiteName;
            site.Url = _kachuwaConfig.SiteUrl;
            site.Image = _kachuwaConfig.SiteUrl+ storeInfo.Logo;

            site.PotentialAction = new SearchAction()
            {
                Target = new MXTires.Microdata.Core.Intangible.EntryPoint()
                {
                    UrlTemplate = _kachuwaConfig.SiteUrl+"/search?q={q}",
                },
                //Query = "required name=q",
                QueryInput = new PropertyValueSpecification() { ValueName = "q", ValueRequired = true, MultipleValues = true },
                ActionStatus = MXTires.Microdata.Core.Intangible.Enumeration.ActionStatusType.PotentialActionStatus,
            };
            //page.MainEntity = new Products();
            //{
            //    Name = "Boz Scaggs",
            //    StartDate = "2015-05-02T20:00",
            //    Location = new Place() { Name = "Coral Springs Center for the Performing Arts", Address = new PostalAddress() { AddressLocality = "Coral Springs, FL" } },
            //    Offers = new List<Offer>() { new Offer() { Url = "http://frontgatetickets.com/venue.php?id=11766" } }
            //};
            LocalBusiness shop = new LocalBusiness()
            {
                Name = storeInfo.WebsiteName,
                Description = page.MetaDescription,// storeInfo.Description,
                CurrenciesAccepted = "Nrs",
            };

            Language language = new Language() { Name = "English" }; //may need more differentiation
            shop.Address = new PostalAddress()
            {
                AddressCountry = storeInfo.Country,
                //AddressRegion = "BC",
                AddressLocality = storeInfo.Address1,
                PostalCode = "",
                StreetAddress = storeInfo.Address2,
                AreaServed = storeInfo.City,
                AvailableLanguage = language,
                Email = storeInfo.Email,
                Telephone = storeInfo.PhoneNumber
            };
            shop.Location = new Place();
            shop.Location.Geo = new GeoCoordinates((float)storeInfo.Longitude, (float)storeInfo.Lattitude);

            //OpeningHoursSpecification mondayHours = new OpeningHoursSpecification("5:30 PM", DaysOfWeek.Mo, "9:00 AM");

            //shop.Location.OpeningHoursSpecification = new List<OpeningHoursSpecification>();
            //shop.Location.OpeningHoursSpecification.Add(mondayHours);
            site.SourceOrganization = shop;


            return site.ToIndentedJson();

        }

        public Task<SEO> GetDetailByUrl(string url, string type)
        {
            var appConfig = ContextResolver.Context.RequestServices.GetService<IOptionsSnapshot<KachuwaAppConfig>>();
            _kachuwaConfig = appConfig.Value;
            throw new NotImplementedException();
        }
       


        //public string GenerateJsonLdForProduct(int productId)
        //{
        //    var productService = (IProductService)AutofacDependencyResolver.Current.GetService(typeof(IProductService));
        //    var productData = productService.ProductCrud.Get(productId);
        //    var aggregate = productService.GetReviewSummarySync(productId);

        //    if (productData.ProductTypeId == 1)
        //    {
        //        var product = new Product()
        //        {
        //            Name = productData.Name,
        //            Category = "Category",
        //            Description = productData.Description,
        //            Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
        //            AggregateRating = new AggregateRating()
        //            {
        //                Id = "/SiteAggregateRating",
        //                BestRating = aggregate.HighestRating.ToString(),
        //                WorstRating = aggregate.MinRating.ToString(),
        //                RatingValue = aggregate.AverageRating.ToString(),
        //                ReviewCount = aggregate.TotalReviews.ToString(),
        //                Description = "novolibooks.com Reviews and Ratings by customer.",
        //                Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
        //            },
        //            Offers = new List<Offer>()
        //            {
        //                new Offer()
        //                {
        //                    Availability =
        //                        productData.Quantity > 1 ? ItemAvailability.InStock : ItemAvailability.OutOfStock,
        //                    Price = productData.Price.ToString(),
        //                    PriceCurrency = "GBP",
        //                    ItemCondition = OfferItemCondition.NewCondition,
        //                    Image = productData.RootImage,
        //                    AcceptedPaymentMethod = PaymentMethod.PayPal,

        //                }
        //            }

        //        };
        //        return product.ToIndentedJson();
        //    }
        //    else
        //    {
        //        var bookData = new BookService().Book.Get(productId);
        //        var product = new Book()
        //        {

        //            Name = productData.Name,
        //            Description = productData.Description,
        //            Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
        //            BookFormat = (BookFormatType)Enum.Parse(typeof(BookFormatType), bookData.Binding),
        //            BookEdition = bookData.Edition,
        //            Author = new Person()
        //            {
        //                Name = bookData.Author
        //            },
        //            NumberOfPages = bookData.NoOfPages,
        //            InLanguage = bookData.Language,
        //            AggregateRating = new AggregateRating()
        //            {
        //                Id = "/SiteAggregateRating",
        //                BestRating = aggregate.HighestRating.ToString(),
        //                WorstRating = aggregate.MinRating.ToString(),
        //                RatingValue = aggregate.AverageRating.ToString(),
        //                ReviewCount = aggregate.TotalReviews.ToString(),
        //                Description = "novolibooks.com Reviews and Ratings by customer.",
        //                Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
        //            },
        //            Offers = new List<Offer>()
        //            {
        //                new Offer()
        //                {
        //                    Availability =
        //                        productData.Quantity > 1 ? ItemAvailability.InStock : ItemAvailability.OutOfStock,
        //                    Price = productData.Price.ToString(),
        //                    PriceCurrency = "GBP",
        //                    ItemCondition = OfferItemCondition.NewCondition,
        //                    Image = productData.RootImage,
        //                    AcceptedPaymentMethod = PaymentMethod.PayPal,

        //                }
        //            }

        //        };
        //        return product.ToIndentedJson();
        //    }

        //    //Review review1 = new Review() { Name = "Review1", ReviewRating = new Rating() { RatingValue = "5" }, ReviewBody = "Best product ever!", Author = new Person() { Name = "Some Guy" } };
        //    //Review review2 = new Review() { Name = "Review2", ReviewRating = new Rating() { RatingValue = "4" }, ReviewBody = "I've seen better...", Author = new Person() { Name = "Other Guy" } };
        //    //product.Reviews = new List<Review> { review1, review2 };

        //}

    }
}