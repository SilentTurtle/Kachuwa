using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SelectAttribute : InputAttribute
    {
        public SelectAttribute() : base("select")
        {

        }
    }
}