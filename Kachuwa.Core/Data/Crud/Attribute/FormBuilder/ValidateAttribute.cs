using System;
using System.Collections.Generic;
using System.Linq;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateAttribute : System.Attribute
    {
        public List<string> Validations { get; set; }

        public ValidateAttribute(string validation)
        {
            Validations = validation.Split(',').ToList();
        }
    }
}