using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EmailAttribute : InputAttribute
    {
        public EmailAttribute() : base("email")
        {

        }
    }
}