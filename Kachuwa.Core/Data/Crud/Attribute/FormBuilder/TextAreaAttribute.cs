using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextAreaAttribute : InputAttribute
    {
        public TextAreaAttribute() : base("textarea")
        {

        }
    }
}