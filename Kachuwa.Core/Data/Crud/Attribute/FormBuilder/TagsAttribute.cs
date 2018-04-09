using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TagsAttribute : InputAttribute
    {
        public TagsAttribute() : base("tags")
        {

        }
    }
}