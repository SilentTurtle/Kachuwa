using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InputAttribute : System.Attribute
    {
        public string Input { get; set; }

        protected InputAttribute(string inputType)
        {
            Input = inputType;
        }
    }
}