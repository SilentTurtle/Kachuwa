using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Kachuwa.Web.Module
{
    public interface IModuleComponentDescription
    {
        ViewComponentDescriptor ComponentDescriptor { get; set; }
        string DisplayName { get; set; }
        string FullName { get; set; }
        bool IsVisibleOnUI { get; set; }
        string ShortName { get; set; }
    }
}