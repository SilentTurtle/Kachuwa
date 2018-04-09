using System;

namespace Kachuwa.Data.Crud.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EditorAttribute : InputAttribute
    {
        public EditorAttribute() : base("editor")
        {

        }
    }
}