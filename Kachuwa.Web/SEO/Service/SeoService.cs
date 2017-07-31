//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Dapper;
//using Kachuwa.Data;
//using Kachuwa.Web.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.Routing;
//using MXTires.Microdata.Core;
//using MXTires.Microdata.Core.CreativeWorks;
//using MXTires.Microdata.Core.Intangible;
//using MXTires.Microdata.Core.Intangible.Enumeration;
//using MXTires.Microdata.Core.Intangible.StructuredValues;
//using MXTires.Microdata.Core.LocalBusinesses.Stores;

//namespace Kachuwa.Web
//{
//    public class SeoService : ISeoService
//    {
//        //private readonly IStoreService _storeService;
//        //private readonly ProductSetting _productSetting;
//        //private readonly IProductService _productService;
//        //private readonly IBrandService _brandService;
//        private UrlHelper _urlHelper;
//        private Store _storeMainBranch;

//        //public SeoService(IStoreService storeService, ProductSetting productSetting,
//        //    IBrandService brandService)
//        //{
//        //   _storeService = storeService;
//        //    _productSetting = productSetting;
//        //    _brandService = brandService;
//        //    _storeMainBranch = _storeService.Store.Get("Where IsActive=1 and IsMainBranch=1");
//        //}

//        public SeoService() { }

//        public CrudService<SEO> Seo { get; set; } = new CrudService<SEO>();

//        private string Controller { get; set; }
//        private string Action { get; set; }
//        private string Path { get; set; }

//        private SEO GetPageContents(string url)
//        {
//            return Seo.Get("Where url=@url", new { url });
//        }


//        public async Task<SEO> GetByProductId(int productId)
//        {
//            var dbfactory = DbFactoryProvider.GetFactory();
//            using (var db = (SqlConnection)dbfactory.GetConnection())
//            {
//                await db.OpenAsync();
//                var result = await db.QueryAsync<SEO>("Select * from dbo.Seo Where SeoType=@SeoType and ProductId=@ProductId", new { SeoType = "product", ProductId = productId });
//                return result.FirstOrDefault();
//            }
//        }

//        public string GetPage()
//        {
//            //string actionName = ContextResolver.Context.Request. RouteData.Values["action"].ToString();
//            //string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

//            //return _urlHelper.ActionUrl(actionName, controllerName, new { area = "" });
//            return "";
//        }

//        public string GenerateMetaContents()
//        {
//            try
//            {
//                var requestContext = HttpContext.Current.Request.RequestContext;
//                _urlHelper = new UrlHelper(requestContext);

//                string path = GetPage();

//                if (path.StartsWith("/products/"))
//                {
//                    path = path.Replace("/products", "");
//                }
//                StringBuilder metatags = new StringBuilder();
//                var page = GetPageContents(path);
//                if (page == null)
//                    return "";
//                if (page.SeoType == "product")
//                {
//                    return GenerateForProduct(page);
//                }
//                else
//                {
//                    var title = new TitleTag(new Dictionary<string, string>
//                    {
//                        {"title", page.MetaTitle}
//                    });
//                    metatags.Append(title.Generate());
//                    var normalMetatag = new MetaTag(new Dictionary<string, string>
//                    {
//                        {"title", page.MetaTitle},
//                        {"name", page.MetaTitle},
//                        {"description", page.MetaDescription},
//                        {"image", _storeMainBranch.StoreLogo}
//                    });
//                    metatags.Append(normalMetatag.Generate());
//                    var openGrap = new OgMetaTag(new Dictionary<string, string>
//                    {
//                        //{"type", "page"},
                       
//                        {"type", "website"},
//                        {"title", page.MetaTitle},
//                        {"description", page.MetaDescription},
//                        //{"type", "article"},//product
//                        {"locale", "en_US"}
//                        ,
//                        {"image", _storeMainBranch.StoreLogo}
//                        // {"url", _urlHelper.AbsoluteRouteUrl(page.Url)},

//                    }, "926715557465044");
//                    metatags.Append(openGrap.Generate());

//                    var twitter = new TwitterMetaTag(new Dictionary<string, string>
//                    {
//                        {"card", "summary"},
//                        {"title", page.MetaTitle},
//                        {"description", page.MetaDescription}
//                        ,
//                        {"image", _storeMainBranch.StoreLogo}
//                    });
//                    metatags.Append(twitter.Generate());
//                    return metatags.ToString() + GenerateJsonLdForWebSite();

//                }

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        public async Task<bool> CheckUrlExist(string url, string type)//1=page
//        {
//            var dbfactory = DbFactoryProvider.GetFactory();
//            using (var db = (SqlConnection)dbfactory.GetConnection())
//            {
//                var p = new DynamicParameters();
//                p.Add("@Url", url);
//                p.Add("@SEOType", type);
//                p.Add("@IsExist", dbType: DbType.Boolean, direction: ParameterDirection.Output);
//                await db.OpenAsync();
//                // p.Add("@c", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
//                await db.ExecuteAsync("usp_SEO_CheckUrlExist", p, commandType: CommandType.StoredProcedure);
//                return p.Get<bool>("@IsExist");
//            }
//        }

//        public async Task<SEO> GetDetailByUrl(string url, string type)
//        {
//            var dbfactory = DbFactoryProvider.GetFactory();
//            using (var db = (SqlConnection)dbfactory.GetConnection())
//            {
//                var p = new DynamicParameters();
//                p.Add("@Url", url);
//                p.Add("@SEOType", type);
//                await db.OpenAsync();
//                // p.Add("@IsExist", dbType: DbType.Boolean, direction: ParameterDirection.Output);
//                // p.Add("@c", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
//                var x = await db.QueryAsync<SEO>("usp_SEO_GetDetailByUrl", p, commandType: CommandType.StoredProcedure);
//                return x.FirstOrDefault();
//            }
//        }

//        public string GenerateForProduct(SEO seoProduct)
//        {
//            try
//            {

//                //string path = GetPage();
//                //if (path.StartsWith("/products/"))
//                //{
//                //    path = path.Replace("/products", "");
//                //}
//                //
//                //var seoProduct = GetPageContents(path);
//                //if (seoProduct == null)
//                //    return "";
//                StringBuilder metatags = new StringBuilder();
//                var seoImage = GetProductImageForSeo(seoProduct.ProductId);

//                var title = new TitleTag(new Dictionary<string, string>
//                {
//                    {"title", seoProduct.MetaTitle}
//                });
//                metatags.Append(title.Generate());
//                var normalMetatag = new MetaTag(new Dictionary<string, string>
//                {
//                    {"name", seoProduct.MetaTitle},
//                    {"description", seoProduct.MetaDescription}
//                });
//                metatags.Append(normalMetatag.Generate());
//                var openGrap = new OgMetaTag(new Dictionary<string, string>
//                {

//                    {"type", "product"},
//                    {"title", seoProduct.MetaTitle},
//                    {"description", seoProduct.MetaDescription},
//                    {"image", seoImage ?? ""},//TODO
//                    //{"type", "article"},//product
//                    {"locale", "en_US"}
//                    // {"url", _urlHelper.AbsoluteRouteUrl(page.Url)},

//                }, "926715557465044");
//                metatags.Append(openGrap.Generate());

//                var twitter = new TwitterMetaTag(new Dictionary<string, string>
//                {
//                    {"card", "summary"},
//                    {"title", seoProduct.MetaTitle},
//                    {"description", seoProduct.MetaDescription},
//                    {"image", seoImage ?? ""}
//                });
//                metatags.Append(twitter.Generate());

//                var x = metatags.ToString();
//                var y = GenerateJsonLdForProduct(seoProduct.ProductId);
//                return x + y;


//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        public async Task<SEO> GetBySeoType(string seoType, int id)
//        {
//            var dbfactory = DbFactoryProvider.GetFactory();
//            using (var db = (SqlConnection)dbfactory.GetConnection())
//            {
//                await db.OpenAsync();
//                var result = await db.QueryAsync<SEO>("Select * from dbo.Seo Where SeoType=@SeoType and ProductId=@ProductId", new { SeoType = seoType, ProductId = id });
//                return result.FirstOrDefault();
//            }
//        }

//        private string GetProductImageForSeo(int productId)
//        {
//            var dbfactory = DbFactoryProvider.GetFactory();
//            var prodImage = "";
//            using (var db = (SqlConnection)dbfactory.GetConnection())
//            {
//                prodImage = db.ExecuteScalar<string>("Select RootImage from dbo.Product where ProductId=@ProductId",
//                    new { ProductId = productId });
//            }
//            if (string.IsNullOrEmpty(prodImage))
//                return "";
//            return _urlHelper.Content(prodImage, true);
//        }

//        public string GenerateJsonLdForWebSite()
//        {
//            var storeInfo = _storeService.Store.Get("Where IsActive=1 and IsMainBranch=1");
//            var site = new WebSite();
//            site.AlternateName = "Novoli Books";
//            site.Url = "http://www.novolibooks.com/";
//            site.Image = storeInfo.StoreLogo;

//            site.PotentialAction = new SearchAction()
//            {
//                Target = new MXTires.Microdata.Intangible.EntryPoint() { UrlTemplate = "http://novolibooks.com/products/simple/search?q={q}", },
//                //Query = "required name=q",
//                QueryInput = new PropertyValueSpecification() { ValueName = "q", ValueRequired = true, MultipleValues = true },
//                ActionStatus = MXTires.Microdata.Intangible.Enumeration.ActionStatusType.PotentialActionStatus,
//            };
//            //page.MainEntity = new Products();
//            //{
//            //    Name = "Boz Scaggs",
//            //    StartDate = "2015-05-02T20:00",
//            //    Location = new Place() { Name = "Coral Springs Center for the Performing Arts", Address = new PostalAddress() { AddressLocality = "Coral Springs, FL" } },
//            //    Offers = new List<Offer>() { new Offer() { Url = "http://frontgatetickets.com/venue.php?id=11766" } }
//            //};
//            LocalBusiness shop = new LocalBusiness()
//            {
//                Name = storeInfo.StoreName,
//                Description = storeInfo.Description,
//                CurrenciesAccepted = "GBP",
//            };

//            Language language = new Language() { Name = "English" }; //may need more differentiation
//            shop.Address = new PostalAddress()
//            {
//                AddressCountry = storeInfo.Country,
//                //AddressRegion = "BC",
//                AddressLocality = storeInfo.StreetName,
//                PostalCode = storeInfo.ZipCode,
//                StreetAddress = storeInfo.Address,
//                AreaServed = storeInfo.City,
//                AvailableLanguage = language,
//                Email = storeInfo.SupportEmail,
//                Telephone = storeInfo.ContactNo
//            };
//            shop.Location = new Place();
//            shop.Location.Geo = new GeoCoordinates(storeInfo.Longitude.ToString(), storeInfo.Lattitude.ToString());

//            //OpeningHoursSpecification mondayHours = new OpeningHoursSpecification("5:30 PM", DaysOfWeek.Mo, "9:00 AM");

//            //shop.Location.OpeningHoursSpecification = new List<OpeningHoursSpecification>();
//            //shop.Location.OpeningHoursSpecification.Add(mondayHours);
//            site.SourceOrganization = shop;


//            return site.ToIndentedJson();

//        }

//        public string GenerateJsonLdForProduct(int productId)
//        {
//            var productService = (IProductService)AutofacDependencyResolver.Current.GetService(typeof(IProductService));
//            var productData = productService.ProductCrud.Get(productId);
//            var aggregate = productService.GetReviewSummarySync(productId);

//            if (productData.ProductTypeId == 1)
//            {
//                var product = new Product()
//                {
//                    Name = productData.Name,
//                    Category = "Category",
//                    Description = productData.Description,
//                    Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
//                    AggregateRating = new AggregateRating()
//                    {
//                        Id = "/SiteAggregateRating",
//                        BestRating = aggregate.HighestRating.ToString(),
//                        WorstRating = aggregate.MinRating.ToString(),
//                        RatingValue = aggregate.AverageRating.ToString(),
//                        ReviewCount = aggregate.TotalReviews.ToString(),
//                        Description = "novolibooks.com Reviews and Ratings by customer.",
//                        Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
//                    },
//                    Offers = new List<Offer>()
//                    {
//                        new Offer()
//                        {
//                            Availability =
//                                productData.Quantity > 1 ? ItemAvailability.InStock : ItemAvailability.OutOfStock,
//                            Price = productData.Price.ToString(),
//                            PriceCurrency = "GBP",
//                            ItemCondition = OfferItemCondition.NewCondition,
//                            Image = productData.RootImage,
//                            AcceptedPaymentMethod = PaymentMethod.PayPal,

//                        }
//                    }

//                };
//                return product.ToIndentedJson();
//            }
//            else
//            {
//                var bookData = new BookService().Book.Get(productId);
//                var product = new Book()
//                {

//                    Name = productData.Name,
//                    Description = productData.Description,
//                    Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
//                    BookFormat = (BookFormatType)Enum.Parse(typeof(BookFormatType), bookData.Binding),
//                    BookEdition = bookData.Edition,
//                    Author = new Person()
//                    {
//                        Name = bookData.Author
//                    },
//                    NumberOfPages = bookData.NoOfPages,
//                    InLanguage = bookData.Language,
//                    AggregateRating = new AggregateRating()
//                    {
//                        Id = "/SiteAggregateRating",
//                        BestRating = aggregate.HighestRating.ToString(),
//                        WorstRating = aggregate.MinRating.ToString(),
//                        RatingValue = aggregate.AverageRating.ToString(),
//                        ReviewCount = aggregate.TotalReviews.ToString(),
//                        Description = "novolibooks.com Reviews and Ratings by customer.",
//                        Url = "http://www.novolibooks.com/products" + productData.ProductUrl,
//                    },
//                    Offers = new List<Offer>()
//                    {
//                        new Offer()
//                        {
//                            Availability =
//                                productData.Quantity > 1 ? ItemAvailability.InStock : ItemAvailability.OutOfStock,
//                            Price = productData.Price.ToString(),
//                            PriceCurrency = "GBP",
//                            ItemCondition = OfferItemCondition.NewCondition,
//                            Image = productData.RootImage,
//                            AcceptedPaymentMethod = PaymentMethod.PayPal,

//                        }
//                    }

//                };
//                return product.ToIndentedJson();
//            }

//            //Review review1 = new Review() { Name = "Review1", ReviewRating = new Rating() { RatingValue = "5" }, ReviewBody = "Best product ever!", Author = new Person() { Name = "Some Guy" } };
//            //Review review2 = new Review() { Name = "Review2", ReviewRating = new Rating() { RatingValue = "4" }, ReviewBody = "I've seen better...", Author = new Person() { Name = "Other Guy" } };
//            //product.Reviews = new List<Review> { review1, review2 };

//        }

//    }
//}