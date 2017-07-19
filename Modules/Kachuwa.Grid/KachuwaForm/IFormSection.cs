//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Kachuwa.KGrid;

//namespace Kachuwa.Form
//{
//    //public interface IFormSection
//    //{
        
//    //}

//    public interface IFormSection
//    {
//        String Name { get; set; }
//    }
//    public interface IFormSection<out T>
//    {
//        String CssClasses { get; set; }
//        T Model { get; }
//    }
//    public interface IFormSections<out T> : IEnumerable<IFormSection<T>>
//    {
//    }

//    public interface IFormSectionOf<T> : IFormSections<T>
//    {
//        Func<T, String> CssClasses { get; set; }
//        IForm<T> Form { get; }

//        IFormSection<T> Add(String name, String classes);
//        IFormSection<T> Add(String name, String classes, Action<IFormRowsOf<T>> action);
//        //IFormSection<T> Add();
//        //IFormSection<T> Add<TValue>(Expression<Func<T, TValue>> constraint);

//       // IFormRows<T> Add(Action<IFormSectionOf<T>> builder)
//    }
//    public interface IFormSections
//    {

//    }
//    public class FormSection<T> : IFormSection<T>
//    {
//        public String CssClasses { get; set; }
//        public T Model { get; set; }

//        public FormSection(T model)
//        {
//            Model = model;
//        }
//    }
//    public class FormSections<T> : IFormSectionOf<T>
//    {
//        public IEnumerable<IFormSection<T>> CurrentRows { get; set; }
//        public Func<T, String> CssClasses { get; set; }
//        public IForm<T> Form { get; set; }

//        public FormSections(IForm<T> form)
//        {
//            Form = form;
//        }

//        public virtual IEnumerator<IFormSection<T>> GetEnumerator()
//        {
//            if (CurrentRows == null)
//            {
//                var items = Form.Model;
//                CurrentRows = (IEnumerable<IFormSection<T>>) items;
//                //.ToList()
//                //.Select(model => new KGrid.KachuwaGridRow<T>(model)
//                //{
//                //    CssClasses = CssClasses?.Invoke(model)
//                //});
//                //IQueryable<T> items = Grid.Source;
//                //foreach (IGridProcessor<T> processor in Grid.Processors.Where(proc => proc.ProcessorType == GridProcessorType.Pre))
//                //    items = processor.Process(items);

//                //foreach (IGridProcessor<T> processor in Grid.Processors.Where(proc => proc.ProcessorType == GridProcessorType.Post))
//                //    items = processor.Process(items);

//                //CurrentRows = items
//                //    .ToList()
//                //    .Select(model => new GridRow<T>(model)
//                //    {
//                //        CssClasses = CssClasses?.Invoke(model)
//                //    });
//            }

//            return CurrentRows.GetEnumerator();
//        }
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        public IFormSection<T> Add(string name, string classes)
//        {
//            throw new NotImplementedException();
//        }

//        public IFormSection<T> Add(string name, string classes, Action<IFormRowsOf<T>> action)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}

using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

public class MyContent : IHtmlContent
{
    public IHtmlHelper Html { get; set; }

    public MyContent(IHtmlHelper html)
    {
        Html = html;
    }
    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
        Html.Partial("_PageNotFound").WriteTo(writer, encoder);
    }
   
}

public static class tt
{
    public static MyContent RenderMyContent(this IHtmlHelper html)
    {
        return new MyContent(html);
    }
}