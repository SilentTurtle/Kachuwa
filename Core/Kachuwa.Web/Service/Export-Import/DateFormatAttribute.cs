using System;

namespace Kachuwa.Web
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateFormatAttribute : Attribute
    {
        public string DateFormatter;
        public DateFormatAttribute(string dateFormatter)
        {
            DateFormatter = dateFormatter;
        }
    }
}