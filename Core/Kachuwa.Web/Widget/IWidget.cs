using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Storage;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    public interface IWidget
    {
        string SystemName { get; }
        string Description { get; set; }
        string Author { get; set; }
        IEnumerable<WidgetSetting> Settings { get; set; }
        //Task<IHtmlContent> Render();
        Type WidgetViewComponent { get; set; }
    }
}
