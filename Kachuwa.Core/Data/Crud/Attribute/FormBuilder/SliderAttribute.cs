using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SliderAttribute : InputAttribute
    {
        public SliderAttribute() : base("slider")
        {

        }
    }
}