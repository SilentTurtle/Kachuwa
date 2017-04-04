using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileAttribute : InputAttribute
    {
        public FileAttribute() : base("file")
        {

        }

        public string Allow { get; set; }

        public bool IsSingleFile { get; set; }
    }
}