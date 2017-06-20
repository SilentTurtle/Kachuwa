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
        string Name { get; set; }
        bool VerticalForm { get; set; }
        string Heading { get; set; }
        string Action { get; set; }
        string SubHeading { get; set; }
        string CssClasses { get; set; }

        Boolean RequestAntiFrogeryToken { get; set; }
        ViewContext ViewContext { get; set; }
        IFormCollection FormCollections { get; set; }

        // IKachuwaGridColumns<IKachuwaGridColumn> Columns { get; }
        // IFormRows<Object> Rows { get; }
        IKachuwaGridCommands<IKachuwaGridCommand> Commands { get; }

        IFormSections<IFormSection> Sections { get; }

        object FormModel { get; }
        string CancelUrl { get; set; }

    }

    public interface IForm<T> : IForm
    {
        T Model { get; set; }
        // new IKachuwaGridColumnsOf<T> Columns { get; }
        //  new IFormRows<T> Rows { get; }
        new IKachuwaGridCommandsOf<T> Commands { get; }

        new IFormSectionsOf<T> Sections { get; }
    }

    public class KachuwaForm<T> : IForm<T> where T : class, new()
    {
        public IForm<T> Form { get; set; }
        public string Name { get; set; } = "";
        public string Action { get; set; } = "";
        public string CssClasses { get; set; } = "";

        public T Model { get; set; }
        public IFormCollection FormCollections { get; set; }
        public ViewContext ViewContext { get; set; }
        //  public IList<IGridProcessor<T>> Processors { get; set; }

        //IKachuwaGridColumns<IKachuwaGridColumn> IKachuwaGrid.Columns => Columns;
        //public IKachuwaGridColumnsOf<T> Columns { get; set; }

        IFormSections<IFormSection> IForm.Sections => Sections;
        public string CancelUrl { get; set; } = "#";

        public IFormSectionsOf<T> Sections { get; set; }
        public object FormModel => Model;

        public KachuwaForm(string formName)
        {
            Name = formName;
            Sections = new FormSections<T>(this);
            Model = new T();
        }
        public KachuwaForm(string formName, T model)
        {
            Name = formName;
            Model = model ?? new T();
            Sections = new FormSections<T>(this);

        }
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
      
    }

    public interface IKachuwaHtmlForm<T> : IHtmlContent
    {

    }
    public class KachuwaHtmlForm<T> : IKachuwaHtmlForm<T>
    {
        public IForm<T> Form { get; set; }
        public IHtmlHelper Html { get; set; }
        public string PartialViewName { get; set; }
        public KachuwaHtmlForm(IHtmlHelper html, IForm<T> form)
        {
            Form = form;
            // form.Query = form.Query ?? html.ViewContext.HttpContext.Request.Query;
            Form.ViewContext = form.ViewContext ?? html.ViewContext;
            PartialViewName = "KachuwaGrid/Form";
            Html = html;

        }
        public virtual KachuwaHtmlForm<T> SetHeading(string heading)
        {
            Form.Heading = heading;

            return this;
        }
        public virtual KachuwaHtmlForm<T> SetSubHeading(string subHeading)
        {
            Form.SubHeading = subHeading;

            return this;
        }
        public virtual KachuwaHtmlForm<T> SetClasses(string classes)
        {
            Form.CssClasses = classes;

            return this;
        }
        public virtual KachuwaHtmlForm<T> CancelUrl(string url)
        {
            Form.CancelUrl = url;

            return this;
        }
        public virtual KachuwaHtmlForm<T> ActionUrl(string actionUrl)
        {
            Form.Action = actionUrl;

            return this;
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
        //public virtual IForm<T> Css(string cssClasses)
        //{
        //    Grid.CssClasses = cssClasses;

        //    return this;
        //}
        //public virtual IHtmlGrid<T> Empty(string text)
        //{
        //    Grid.NoDataText = text;

        //    return this;
        //}
        //public virtual IHtmlGrid<T> Named(string name)
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
            try
            {
                Html.Partial(PartialViewName, Form).WriteTo(writer, encoder);
            }
            catch (Exception e)
            {

                throw e;
            }

        }


    }
}
