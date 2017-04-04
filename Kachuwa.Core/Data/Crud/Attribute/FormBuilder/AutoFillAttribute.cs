using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoFillAttribute : InputAttribute
    {
        public object DefaultValue;
        public bool IsDate = false;

        public AutoFillAttribute(object value) : base("hidden")
        {
            DefaultValue = value;
        }
        public AutoFillAttribute() : base("hidden")
        {

        }


        public bool GetCurrentUser = false;
    }
}