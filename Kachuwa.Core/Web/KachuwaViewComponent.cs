using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    public abstract class KachuwaViewComponent : ViewComponent
    {

        public abstract string DisplayName { get; }

        public abstract bool IsVisibleOnUI { get; }

    }
}