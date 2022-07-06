//using System;
//using Microsoft.Extensions.Localization;

//namespace Kachuwa.Localization
//{
//    public class DataAnnotationLocalizerFactory : IStringLocalizerFactory
//    {
//        private readonly ILocaleResourceProvider _localeResourceProvider;

//        public DataAnnotationLocalizerFactory(ILocaleResourceProvider localeResourceProvider)
//        {
//            _localeResourceProvider = localeResourceProvider;
          
//        }
//        public IStringLocalizer Create(Type resourceSource)
//        {
//            return new DataAnnotationLocalizer(_localeResourceProvider);
//        }

//        public IStringLocalizer Create(string baseName, string location)
//        {
//            return new DataAnnotationLocalizer(_localeResourceProvider);
//        }
//    }
//}