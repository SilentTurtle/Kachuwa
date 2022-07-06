using System;

namespace Kachuwa.Web
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReportHideAttribute : Attribute
    {
        public ReportHideAttribute()
        {
            
        }
    }
}