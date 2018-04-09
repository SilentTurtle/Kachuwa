using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideAttribute : InputAttribute
    {

        public HideAttribute() : base("hidden")
        {

        }

    }
}