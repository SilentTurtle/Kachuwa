using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Configuration;
using Kachuwa.Data;
using Kachuwa.Log;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MXTires.Microdata.Core;
using MXTires.Microdata.Core.CreativeWorks;
using MXTires.Microdata.Core.Intangible;
using MXTires.Microdata.Core.Intangible.StructuredValues;

namespace Kachuwa.Web
{
    public class SeoService : ISeoService
    {
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private IUrlHelper _urlHelper;
        private Setting _setting;
        private KachuwaAppConfig _kachuwaConfig;


        [ViewContext] public ViewContext ViewContext { get; set; }

        public IUrlHelperFactory UrlHelperFactory { get; set; }

        public SeoService(ISettingService settingService, IActionContextAccessor actionContextAccessor, ILogger logger)
        {

            _settingService = settingService;
            _actionContextAccessor = actionContextAccessor;
            _logger = logger;
        }

        public CrudService<SEO> Seo { get; set; } = new CrudService<SEO>();


        private string Path { get; set; }
        private bool IsDynamicPage = false;

        private async Task<SEO> GetPageContents(string url)
        {
            if (!url.StartsWith("/"))
                url = "/" + url;
            return await Seo.GetAsync("Where url=@url", new { url });
        }

        private string GetPage()
        {
            if (ContextResolver.Context.Items.Keys.Contains("KPageUrl"))
            {
                var url = ContextResolver.Context.Items["KPageUrl"];
                IsDynamicPage = true;
                return url.ToString();
            }
            else
            {
                IsDynamicPage = false;
                string actionName = ContextResolver.Context.GetRouteValue("action").ToString();
                string controllerName = ContextResolver.Context.GetRouteValue("controller").ToString();
                return _urlHelper.Action(actionName, controllerName);

            }

        }

        public async Task<string> GenerateMetaContents()
        {
            try
            {
                var appConfig = ContextResolver.Context.RequestServices
                    .GetService<IOptionsSnapshot<KachuwaAppConfig>>();
                _kachuwaConfig = appConfig.Value;
                var actionContext = _actionContextAccessor.ActionContext;
                _urlHelper = new UrlHelper(actionContext);
                _setting = await _settingService.CrudService.GetAsync(1);
                string path = GetPage();

                StringBuilder metatags = new StringBuilder();
                var page = await GetPageContents(path);
                if (page == null)
                    return "";
                else
                {

                    var title = new TitleTag(new ConcurrentDictionary<string, string> { ["title"] = page.MetaTitle });
                    metatags.Append(title.Generate());
                    var normalMetatag = new MetaTag(new ConcurrentDictionary<string, string>
                    {
                        ["title"] = page.MetaTitle,
                        ["name"] = page.MetaTitle,
                        ["description"] = page.MetaDescription,
                        ["image"] = _kachuwaConfig.SiteUrl + page.Image
                    });
                    metatags.Append(normalMetatag.Generate());
                    var openGrap = new OgMetaTag(new ConcurrentDictionary<string, string>
                    {

                        ["type"] = "website"
                    ,
                        ["title"] = page.MetaTitle,
                        ["description"] = page.MetaDescription,
                        ["type"] = "article",//product
                        ["locale"] = _setting.BaseCulture,
                        ["image"] = _kachuwaConfig.SiteUrl + page.Image,
                        ["url"] = _kachuwaConfig.SiteUrl + page.Url,

                    }, _kachuwaConfig.FacebookAppId);
                    metatags.Append(openGrap.Generate());

                    var twitter = new TwitterMetaTag(new ConcurrentDictionary<string, string>
                    {
                        ["card"] = "summary",
                        ["title"] = page.MetaTitle,
                        ["description"] = page.MetaDescription,
                        ["image"] = _kachuwaConfig.SiteUrl + page.Image
                    });
                    metatags.Append(twitter.Generate());
                    return metatags.ToString();

                }

            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, () => ex.Message, ex);
                return "";
            }

        }

        public async Task<bool> CheckUrlExist(string url, string type)
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


        public async Task<SEO> GetBySeoType(string seoType, int id)
        {
            var dbfactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbfactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryFirstAsync<SEO>(
                    "Select * from Seo Where SeoType=@SeoType and ProductId=@ProductId",
                    new { SeoType = seoType, ProductId = id });

            }
        }


        public async Task<string> GenerateJsonLdForWebSite()
        {
            try
            {


                var appConfig = ContextResolver.Context.RequestServices
                    .GetService<IOptionsSnapshot<KachuwaAppConfig>>();
                _kachuwaConfig = appConfig.Value;
                var storeInfo = _setting;
                var site = new WebSite
                {
                    AlternateName = storeInfo.WebsiteName,
                    Url = _kachuwaConfig.SiteUrl,
                    Image = _kachuwaConfig.SiteUrl + storeInfo.Logo,
                    PotentialAction = new SearchAction()
                    {
                        Target = new MXTires.Microdata.Core.Intangible.EntryPoint()
                        {
                            UrlTemplate = _kachuwaConfig.SiteUrl + "/search?q={q}",
                        },
                        //Query = "required name=q",
                        QueryInput =
                            new PropertyValueSpecification()
                            {
                                ValueName = "q",
                                ValueRequired = true,
                                MultipleValues = true
                            },
                        ActionStatus = MXTires.Microdata.Core.Intangible.Enumeration.ActionStatusType
                            .PotentialActionStatus,
                    }
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
                    Description = storeInfo.Description, // storeInfo.Description,
                    CurrenciesAccepted = storeInfo.BaseCurrency,
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
                site.SourceOrganization = shop;
                return site.ToIndentedJson();
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, () => ex.Message, ex);
                return "";
            }
        }

        public async Task<string> GenerateJsonLdForPage()
        {
            string path = GetPage();

            var page = await GetPageContents(path);
            if (page == null)
                return "";
            else
            {
                var appConfig = ContextResolver.Context.RequestServices
                    .GetService<IOptionsSnapshot<KachuwaAppConfig>>();
                _kachuwaConfig = appConfig.Value;
                var storeInfo = _setting;
                var webpage = new WebPage()
                {
                    Image = _kachuwaConfig.SiteUrl +
                            (string.IsNullOrEmpty(page.Image) == true ? storeInfo.Logo : page.Image),
                    Description = page.MetaDescription,
                    Headline = page.MetaTitle,
                    Url = _kachuwaConfig.SiteUrl + page.Url,
                    Name = page.MetaTitle,
                    DateCreated = page.AddedOn

                };
                return webpage.ToIndentedJson();
            }

        }

        public async Task<SEO> GetByProductId(int producId = 0, string type = "page")
        {
            return await Seo.GetAsync("Where ProductId=@ProductId and SeoType=@SeoType ", new { SeoType = type, ProductId = producId });

        }

        public async Task<string> GenerateJsonLdForPage(string page, int productId, string type)
        {
            string path = page;

            var pageContent = productId > 0 ? await GetByProductId(productId, type) : await GetPageContents(path);
            if (pageContent == null)
                return "";
            else
            {
                var appConfig = ContextResolver.Context.RequestServices
                    .GetService<IOptionsSnapshot<KachuwaAppConfig>>();
                _kachuwaConfig = appConfig.Value;
                var storeInfo = _setting;
                var webpage = new WebPage()
                {
                    Image = _kachuwaConfig.SiteUrl +
                            (string.IsNullOrEmpty(pageContent.Image) == true ? storeInfo.Logo : pageContent.Image),
                    Description = pageContent.MetaDescription,
                    Headline = pageContent.MetaTitle,
                    Url = _kachuwaConfig.SiteUrl + pageContent.Url,
                    Name = pageContent.MetaTitle,
                    DateCreated = pageContent.AddedOn

                };
                return webpage.ToIndentedJson();
            }

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


        public async Task<string> GetSEOMetaContentsAsync(string url, string type)
        {
            var page = await Seo.GetAsync("Where lower(seoType)=lower(@seoType) and lower(Url)=lower(@url)",
                new { seoType = type, url });
            if (page == null)
                return "";
            else
            {
                StringBuilder metatags = new StringBuilder();

                var title = new TitleTag(new ConcurrentDictionary<string, string> { ["title"] = page.MetaTitle });
                metatags.Append(title.Generate());
                var normalMetatag = new MetaTag(new ConcurrentDictionary<string, string>
                {
                    ["title"] = page.MetaTitle,
                    ["name"] = page.MetaTitle,
                    ["description"] = page.MetaDescription,
                    ["image"] = _kachuwaConfig.SiteUrl + page.Image
                });
                metatags.Append(normalMetatag.Generate());
                var openGrap = new OgMetaTag(new ConcurrentDictionary<string, string>
                {

                    ["type"] = "website"
                    ,
                    ["title"] = page.MetaTitle,
                    ["description"] = page.MetaDescription,
                    ["type"] = "article",//product
                    ["locale"] = _setting.BaseCulture,
                    ["image"] = _kachuwaConfig.SiteUrl + page.Image,
                    ["url"] = _kachuwaConfig.SiteUrl + page.Url,

                }, _kachuwaConfig.FacebookAppId);
                metatags.Append(openGrap.Generate());

                var twitter = new TwitterMetaTag(new ConcurrentDictionary<string, string>
                {
                    ["card"] = "summary",
                    ["title"] = page.MetaTitle,
                    ["description"] = page.MetaDescription,
                    ["image"] = _kachuwaConfig.SiteUrl + page.Image
                });
                metatags.Append(twitter.Generate());
                return metatags.ToString();


            }

        }

        public async Task<SEO> GetSEODataAsync(string url, string type)
        {
            var page = await Seo.GetAsync("Where lower(seoType)=lower(@seoType) and (lower(Url)=lower(@url) or lower(Url)=lower('/'+@url))",
                new { seoType = type, url });
            return page;
        }
    }
}