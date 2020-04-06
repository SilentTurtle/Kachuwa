using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kachuwa.Utility
{
    public static class Converter
    {
        public static IEnumerable<SelectListItem> EnumSelectListConverter<T>()
        {
            return (Enum.GetValues(typeof(T)).Cast<int>().Select(
                enu => new SelectListItem() { Text = Enum.GetName(typeof(T), enu), Value = enu.ToString() })).ToList();
        }
    }
}
