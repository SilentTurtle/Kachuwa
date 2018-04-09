using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Kachuwa.Web.Templating
{
    public interface ITemplateComponentBuilder<T> 
    {
         IEnumerable<T> Templates { get; set; }
        IEnumerable<T> GetInvoiceComponents();

    }
}