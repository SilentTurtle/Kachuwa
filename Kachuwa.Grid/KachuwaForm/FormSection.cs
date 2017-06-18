using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Kachuwa.Form
{
    public interface IFormSection
    {
        string Name { get; set; }
        string Heading { get; set; }
        string SubHeading { get; set; }
        string HelpLine { get; set; }
        string CssClasses { get; set; }
        IFormRows<IFormRow> Rows { get; }

    }
    public interface IFormSection<T> : IFormSection
    {
        IForm<T> Form { get; }
       // Func<T, Object> RenderValue { get; set; }
        new IFormRowsOf<T> Rows { get;  }
    }
    public interface IFormSections<out T> : IEnumerable<T> where T : IFormSection
    {
    }
    public interface IFormSectionsOf<T> : IFormSections<IFormSection<T>>
    {
        IForm<T> Form { get; set; }
        IFormRowsOf<T> Rows { get; }
        IFormSection<T> Add(string name, string classes);
        IFormSection<T> Add(string name, string classes, Action<IFormRowsOf<T>> rowsBuilder);
    }


    public abstract class BaseFormSection<T> : IFormSection<T>
    {
       
        public string Name { get; set; }
        public string CssClasses { get; set; }
        public IForm<T> Form { get; set; }
       // public Func<T, Object> RenderValue { get; set; }
        //  public Func<T, TValue> ExpressionValue { get; set; }
        // LambdaExpression IKachuwaGridCommand<T>.Expression => Expression;
        // public Expression<Func<T, TValue>> Expression { get; set; }
       // public abstract IHtmlContent ValueFor(IFormRow<Object> row);
      //  public IFormRows<IFormRow> Rows { get; set; }
       
        public IFormRowsOf<T> Rows { get; set; }

        IFormRows<IFormRow> IFormSection.Rows => Rows;

        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string HelpLine { get; set; }
    }

    public class FormSection<T> : BaseFormSection<T> where T : class
    {

        public FormSection(IForm<T> form, string name, string classes)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            Rows = new FormRows<T>(Form);

        }

        public FormSection(IForm<T> form, string name,string classes, Action<IFormRowsOf<T>> rowsBuilder)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            Rows =new FormRows<T>(Form);
            rowsBuilder(Rows);
           

            //rowsBuilder.BeginInvoke(new FormRows<T>(form);
        }


    }

    public class FormSections<T> : List<IFormSection<T>>, IFormSectionsOf<T> where T : class
    {
        public IForm<T> Form { get; set; }

        public FormSections(IForm<T> form)
        {
            Form = form;
        }



        public IFormSection<T> Add(string name, string classes)
        {
            IFormSection<T> section = new FormSection<T>(Form, name, classes);
            Add(section);

            return section;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IFormSection<T> Add(string name, string classes, Action<IFormRowsOf<T>> rowsBuilder)
        {

            IFormSection<T> section = new FormSection<T>(Form, name, classes, rowsBuilder);
            Add(section);
            return section;
        }
       

        public IFormRowsOf<T> Rows { get; }
      
    }
}
