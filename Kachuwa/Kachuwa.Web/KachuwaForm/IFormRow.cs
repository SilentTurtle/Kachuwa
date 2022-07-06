using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.Web.Form
{
    public interface IFormRow
    {
        string CssClasses { get; set; }
        IFormColumns<IFormColumn> Columns { get; }
        IHtmlContent Template { get; set; }
    }
    public interface IFormRow<T>: IFormRow
    {
        IForm<T> Form { get; }
        new IFormColumnsOf<T> Columns { get; }
    }
    public interface IFormRows<out T> : IEnumerable<T> where T : IFormRow
    {
    }

    public interface IFormRowsOf<T> : IFormRows<IFormRow<T>>
    {
        IForm<T> Form { get; }
        IFormRow<T> Add(string name, string classes);
        IFormRow<T> Add(string name, string classes, Action<IFormColumnsOf<T>> action);

        IFormRow<T> Add(string name, string classes, Func<dynamic, HelperResult> template);
        IFormRow<T> Add(string name, string classes, Func<dynamic, HelperResult> template,object templateModel);
    }

    public abstract class BaseFormRow<T> : IFormRow<T>
    {
        public string CssClasses { get; set; }
        public IForm<T> Form { get; set; }
        public IFormColumnsOf<T> Columns { get; set; }

        IFormColumns<IFormColumn> IFormRow.Columns
        {
            get { return Columns; }
        }
        public IHtmlContent Template { get; set; }
    }
    public class FormRow<T> : BaseFormRow<T> where T : class
    {

        public FormRow(IForm<T> form,string name, string classes)
        {
            Form = form;
            CssClasses = classes;
            Template = null;
            Columns = new FormColumns<T>(Form);


        }
        public FormRow(IForm<T> form,string name, string classes, Action<IFormColumnsOf<T>> columnsBuilder)
        {
            Form = form;
            CssClasses = classes;
            Columns = new FormColumns<T>(Form);
            Template = null;
            columnsBuilder(Columns);
        }
        public FormRow(IForm<T> form, string name, string classes, Func<dynamic, HelperResult> template)
        {
            Form = form;
            CssClasses = classes;
            Template = template.Invoke(null);
            Columns = new FormColumns<T>(Form);


        }
        public FormRow(IForm<T> form, string name, string classes, Func<dynamic, HelperResult> template,object templateModel)
        {
            Form = form;
            CssClasses = classes;
            Template = template(templateModel);
            Columns = new FormColumns<T>(Form);


        }
    }
    public class FormRows<T> : List<IFormRow<T>>, IFormRowsOf<T> where T : class 
    {
       
        public IForm<T> Form { get; set; }

        public FormRows(IForm<T> form)
        {
            Form = form;
        }
        public FormRows()
        {
           
        }

        public T Model { get; set; }
        public string CssClasses { get; set; }
        public IFormRow<T> Add(string name, string classes)
        {
            IFormRow<T> form = new FormRow<T>(Form,name, classes);
            Add(form);
            return form;
        }

        public IFormColumns<IFormColumn> Columns { get;  set; }
        

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IFormRow<T> Add(string name, string classes, Action<IFormColumnsOf<T>> action)
        {
            IFormRow<T> form = new FormRow<T>(Form,name, classes, action);
            Add(form);
            return form;
        }

        public IFormRow<T> Add(string name, string classes, Func<dynamic, HelperResult> template)
        {
            IFormRow<T> form = new FormRow<T>(Form, name, classes, template);
            Add(form);
            return form;
        }
        public IFormRow<T> Add(string name, string classes, Func<dynamic, HelperResult> template,object templateModel)
        {
            IFormRow<T> form = new FormRow<T>(Form, name, classes, template, templateModel);
            Add(form);
            return form;
        }


        //public virtual IFormRow<T> GetEnumerator()
        //{
        //    if (CurrentRows == null)
        //    {
        //        var items = Form.Model;
        //        //CurrentRows = items
        //        //  .ToList()
        //        //  .Select(model => new KGrid.KachuwaGridRow<T>(model)
        //        //  {
        //        //      CssClasses = CssClasses?.Invoke(model)
        //        //  });
        //        //IQueryable<T> items = Grid.Source;
        //        //foreach (IGridProcessor<T> processor in Grid.Processors.Where(proc => proc.ProcessorType == GridProcessorType.Pre))
        //        //    items = processor.Process(items);

        //        //foreach (IGridProcessor<T> processor in Grid.Processors.Where(proc => proc.ProcessorType == GridProcessorType.Post))
        //        //    items = processor.Process(items);

        //        //CurrentRows = items
        //        //    .ToList()
        //        //    .Select(model => new GridRow<T>(model)
        //        //    {
        //        //        CssClasses = CssClasses?.Invoke(model)
        //        //    });
        //    }

        //    return CurrentRows.GetEnumerator();
        //}
        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }


}