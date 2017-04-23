using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Localization
{
    public class ResourceBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ResourceBuilder(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }



        public void  Build()
        {

        }





    }
}