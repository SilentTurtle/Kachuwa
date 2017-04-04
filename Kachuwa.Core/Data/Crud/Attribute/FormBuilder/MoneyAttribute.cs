using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MoneyAttribute : InputAttribute
    {
        public MoneyAttribute() : base("money")
        {

        }
    }
}