using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DatePickerAttribute : InputAttribute
    {
        public DatePickerAttribute() : base("datepicker")
        {

        }
    }
}