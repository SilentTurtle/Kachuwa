using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PriceAttribute : InputAttribute
    {
        public PriceAttribute() : base("price")
        {

        }
    }
}