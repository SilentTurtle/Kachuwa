using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using Kachuwa.KGrid;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mustache;

namespace Kachuwa.Form
{
    public interface IForm
    {
        String Name { get; set; }
        bool VerticalForm { get; set; }
        String Heading { get; set; }
        String SubHeading { get; set; }
        String CssClasses { get; set; }

        Boolean RequestAntiFrogeryToken { get; set; }
        ViewContext ViewContext { get; set; }
        IFormCollection FormCollections { get; set; }

        // IKachuwaGridColumns<IKachuwaGridColumn> Columns { get; }
        // IFormRows<Object> Rows { get; }
        IKachuwaGridCommands<IKachuwaGridCommand> Commands { get; }

        IFormSections<IFormSection> Sections { get; }

        object FormModel { get;  }

    }

    public interface IForm<T> : IForm
    {
        T Model { get; set; }
        // new IKachuwaGridColumnsOf<T> Columns { get; }
        //  new IFormRows<T> Rows { get; }
        new IKachuwaGridCommandsOf<T> Commands { get; }

        new IFormSectionsOf<T> Sections { get; }
    }

    public class KachuwaForm<T> : IForm<T> where T : class
    {
        public IForm<T> Form { get; set; }
        public String Name { get; set; }

        public String CssClasses { get; set; }

        public T Model { get; set; }
        public IFormCollection FormCollections { get; set; }
        public ViewContext ViewContext { get; set; }
        //  public IList<IGridProcessor<T>> Processors { get; set; }

        //IKachuwaGridColumns<IKachuwaGridColumn> IKachuwaGrid.Columns => Columns;
        //public IKachuwaGridColumnsOf<T> Columns { get; set; }

        IFormSections<IFormSection> IForm.Sections => Sections;


        public IFormSectionsOf<T> Sections { get; set; }
        public object FormModel => Model;

        public KachuwaForm()
        {
            //Processors = new List<IGridProcessor<T>>();
            //Model = model;
            // Columns = new KachuwaGridColumns<T>(this);
            // Rows = new FormRows<T>(this);
            //Commands = new KachuwaGridCommands<T>(this);
            Sections = new FormSections<T>(this);


        }
        public KachuwaForm(T model)
        {
            Model = model;
            //Processors = new List<IGridProcessor<T>>();
            //Model = model;
            // Columns = new KachuwaGridColumns<T>(this);
            // Rows = new FormRows<T>(this);
            //Commands = new KachuwaGridCommands<T>(this);
            Sections = new FormSections<T>(this);


        }
        // public String NoDataText { get; set; }

        // IKachuwaGridCommands<IKachuwaGridCommand> IKachuwaGrid.Commands => Commands;
        public IKachuwaGridCommandsOf<T> Commands { get; set; }
        public Boolean RequestAntiFrogeryToken { get; set; }

        IKachuwaGridCommands<IKachuwaGridCommand> IForm.Commands => Commands;

        public virtual IForm<T> CreateSection(Action<IFormSectionsOf<T>> builder)
        {
            builder(Form.Sections);

            return this;
        }

        public Boolean VerticalForm { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }

        //public virtual IForm<T> CreateSection(Action<IFormSectionsOf<T>> builder)
        //{
        //    builder(Form.Sections);

        //    return this;
        //}
    }

    public interface IKachuwaHtmlForm<T> : IHtmlContent
    {

    }
    public class KachuwaHtmlForm<T> : IKachuwaHtmlForm<T>
    {
        public IForm<T> Form { get; set; }
        public IHtmlHelper Html { get; set; }
        public String PartialViewName { get; set; }

        public KachuwaHtmlForm(IHtmlHelper html, IForm<T> form)
        {
            Form = form;
            // form.Query = form.Query ?? html.ViewContext.HttpContext.Request.Query;
            Form.ViewContext = form.ViewContext ?? html.ViewContext;
            PartialViewName = "KachuwaGrid/Form";
            Html = html;

        }


        public virtual IKachuwaHtmlForm<T> CreateSection(Action<IFormSectionsOf<T>> builder)
        {
            builder(Form.Sections);

            return this;
        }
        //public virtual IHtmlGrid<T> ProcessWith(IKachuwaGridProcessor<T> processor)
        //{
        //    Grid.Processors.Add(processor);

        //    return this;
        //}



        //public virtual IHtmlGrid<T> RowCss(string cssClasses)
        //{
        //    Form.CssClasses = cssClasses;

        //    return this;
        //}
        //public virtual IForm<T> Css(String cssClasses)
        //{
        //    Grid.CssClasses = cssClasses;

        //    return this;
        //}
        //public virtual IHtmlGrid<T> Empty(String text)
        //{
        //    Grid.NoDataText = text;

        //    return this;
        //}
        //public virtual IHtmlGrid<T> Named(String name)
        //{
        //    Grid.Name = name;

        //    return this;
        //}


        //public virtual IHtmlGrid<T> Pageable(Action<KachuwaPager> builder)
        //{
        //    Grid.Pager = new KachuwaPager(100, 1);
        //    builder(Grid.Pager);


        //    return this;
        //}
        //public virtual IHtmlGrid<T> Pageable()
        //{
        //    return Pageable(builder => { });
        //}
        //public virtual IHtmlGrid<T> AddCommands(Action<IKachuwaGridCommandsOf<T>> builder)
        //{
        //    builder(Grid.Commands);

        //    return this;
        //}
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            Html.Partial(PartialViewName, Form).WriteTo(writer, encoder);
        }


    }
}
