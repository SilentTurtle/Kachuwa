using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DependentAttribute : System.Attribute
    {
        public string Get { get; set; }
        public Type Model { get; set; }
        public string Condition { get; set; }
    }
}