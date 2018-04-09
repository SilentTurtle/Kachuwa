using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageAttribute : InputAttribute
    {
        public ImageAttribute() : base("image")
        {

        }

        public string Allow { get; set; }

        public bool IsSingleFile { get; set; }
    }
}