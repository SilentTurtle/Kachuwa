using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.Form
{
    public interface IFormColumn
    {
        string Name { get; set; }
        string CssClasses { get; set; }

        IFormInputs<IFormInput> Controls { get;  }
        string ColumnFormat { get; set; }
        IHtmlContent Template { get; set; }

    }
    public interface IFormColumn<T> : IFormColumn
    {
        IForm<T> Form { get; }
        LambdaExpression Expression { get; }
        Func<T, Object> RenderValue { get; set; }
        new IFormInputsOf<T> Controls { get;  }
    }
    public interface IFormColumns<out T> : IEnumerable<T> where T : IFormColumn
    {
    }
    public interface IFormColumnsOf<T> : IFormColumns<IFormColumn<T>>
    {
        IForm<T> Form { get; }

        IFormColumn<T> Add(string name, string classes, Action<IFormInputsOf<T>> formControls);//final forms
        IFormColumn<T> Add(string name, string classes,string columnFormat, Action<IFormInputsOf<T>> formControls);//final forms
        IFormColumn<T> Add(string name, string classes, Func<dynamic, HelperResult> template);
        IFormColumn<T> Add(string name, string classes, Func<dynamic, HelperResult> template, object templateModel);
    }

    public abstract class BaseFormColumn<T> : IFormColumn<T>
    {
        public string Name { get; set; }
        public string CssClasses { get; set; }
        public IForm<T> Form { get; set; }
        public LambdaExpression Expression { get; }
        public Func<T, Object> RenderValue { get; set; }
        public IFormInputsOf<T> Controls { get; set; }
        public string ColumnFormat { get; set; } = "{0}";
        IFormInputs<IFormInput> IFormColumn.Controls => Controls;
        public IHtmlContent Template { get; set; }
    }

    public class FormColumn<T> : BaseFormColumn<T> where T : class
    {
        public FormColumn(IForm<T> form, string name, string classes)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            Controls = new FormInputs<T>(Form);

        }

        public FormColumn(IForm<T> form, string name, string classes, Action<IFormInputsOf<T>> formcontrolsBuilder)
        {
            Form = form;
            Name = name;
            Controls = new FormInputs<T>(Form);
            CssClasses = classes;
            formcontrolsBuilder(Controls);


            //rowsBuilder.BeginInvoke(new FormRows<T>(form);
        }
        public FormColumn(IForm<T> form, string name, string classes,string format, Action<IFormInputsOf<T>> formcontrolsBuilder)
        {
            Form = form;
            Name = name;
            ColumnFormat = format;
            Controls = new FormInputs<T>(Form);
            CssClasses = classes;
            formcontrolsBuilder(Controls);


            //rowsBuilder.BeginInvoke(new FormRows<T>(form);
        }
        public FormColumn(IForm<T> form, string name, string classes, Func<dynamic, HelperResult> template)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            Template = template.Invoke(null);
            Controls = new FormInputs<T>(Form);


        }
        public FormColumn(IForm<T> form, string name, string classes, Func<dynamic, HelperResult> template, object templateModel)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            Template = template.Invoke(templateModel);
            Controls = new FormInputs<T>(Form);


        }


    }
    public class FormColumns<T> : List<IFormColumn<T>>, IFormColumnsOf<T> where T : class
    {
        public IForm<T> Form { get; set; }

        public FormColumns(IForm<T> form)
        {
            Form = form;
        }
        public FormColumns()
        {
            
        }

        public IFormColumn<T> Add(string name, string classes)
        {
            IFormColumn<T> column = new FormColumn<T>(Form, name, classes);
            Add(column);

            return column;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IFormColumn<T> Add(string name, string classes, Action<IFormInputsOf<T>> formcontrolsBuilder)
        {

            IFormColumn<T> column = new FormColumn<T>(Form, name, classes, formcontrolsBuilder);
            Add(column);
            return column;
        }

        public IFormColumn<T> Add(string name, string classes,string columnFormat, Action<IFormInputsOf<T>> formcontrolsBuilder)
        {

            IFormColumn<T> column = new FormColumn<T>(Form, name, classes, columnFormat, formcontrolsBuilder);
            Add(column);
            return column;
        }

        public IFormColumn<T> Add(string name, string classes, Func<dynamic, HelperResult> template)
        {
            IFormColumn<T> form = new FormColumn<T>(Form, name, classes, template);
            Add(form);
            return form;
        }
        public IFormColumn<T> Add(string name, string classes, Func<dynamic, HelperResult> template,object templateModel)
        {
            IFormColumn<T> form = new FormColumn<T>(Form, name, classes, template, templateModel);
            Add(form);
            return form;
        }
        public IFormRowsOf<T> Rows { get; }
      
    }


}
