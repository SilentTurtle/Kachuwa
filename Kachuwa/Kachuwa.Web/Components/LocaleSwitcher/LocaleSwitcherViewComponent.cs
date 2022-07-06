using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Components.LocaleChanger
{
    
    [ViewComponent(Name = "LocaleSwitcher")]
    public class LocaleSwitcherViewComponent : KachuwaViewComponent
    {

        public LocaleSwitcherViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }

        public override string DisplayName { get; } = "Language Switcher";
        public override bool IsVisibleOnUI { get; } = true;
    }
}
