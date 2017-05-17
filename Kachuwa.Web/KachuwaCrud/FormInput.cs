using System;

namespace Kachuwa.Web
{
    public abstract class FormInput
    {
        public virtual string Name { get; set; }
        public virtual TypeCode ValueType { get; set; }
        public virtual object Value { get; set; }

        public virtual void OnInit()
        {
            
        }
        public virtual object Get()
        {
            return (ValueType) Value;
        }
        public virtual void Set()
        {

        }
        public virtual void OnReset()
        {

        }
    }
}