using System.Collections.Generic;
using System.Reflection;
using Kachuwa.Web.Module;
using System;

namespace Kachuwa.ContactUs
{
    public class ContactUsModule : IModule
    {
        public string Name { get; set; }="ContactUs";
        public string Version { get; set; } = "1.0.0.0";
        public List<string> SupportedVersions { get; set; }=new List<string>(){"1.0.0"};
        public string Author { get; set; } = "Binod Tamang";
        public Assembly Assembly { get; set; } = typeof(ContactUsModule).GetTypeInfo().Assembly;
    }
}