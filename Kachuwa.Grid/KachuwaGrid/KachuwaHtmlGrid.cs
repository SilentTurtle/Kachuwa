using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kachuwa.KGrid
{
    public class KachuwaHtmlGrid<T> : IHtmlGrid<T>
    {
        public IKachuwaGrid<T> Grid { get; set; }
        public IHtmlHelper Html { get; set; }
        public string PartialViewName { get; set; }

        public KachuwaHtmlGrid(IHtmlHelper html, IKachuwaGrid<T> grid)
        {
            grid.Query = grid.Query ?? html.ViewContext.HttpContext.Request.Query;
            grid.ViewContext = grid.ViewContext ?? html.ViewContext;
            PartialViewName = "KachuwaGrid/Grid";
            Html = html;
            Grid = grid;
        }
        
          public virtual IHtmlGrid<T> Pagination(Action<KachuwaPager> builder)
        {
            builder(Grid.Pager);

            return this;
        }
        public virtual IHtmlGrid<T> Build(Action<IKachuwaGridColumnsOf<T>> builder)
        {
            builder(Grid.Columns);

            return this;
        }
        //public virtual IHtmlGrid<T> ProcessWith(IKachuwaGridProcessor<T> processor)
        //{
        //    Grid.Processors.Add(processor);

        //    return this;
        //}
      
       

        public virtual IHtmlGrid<T> RowCss(Func<T, string> cssClasses)
        {
            Grid.Rows.CssClasses = cssClasses;

            return this;
        }
        public virtual IHtmlGrid<T> Css(string cssClasses)
        {
            Grid.CssClasses = cssClasses;

            return this;
        }
        public virtual IHtmlGrid<T> Empty(string text)
        {
            Grid.NoDataText = text;

            return this;
        }
        public virtual IHtmlGrid<T> Named(string name)
        {
            Grid.Name = name;

            return this;
        }


        public virtual IHtmlGrid<T> Pageable(Action<KachuwaPager> builder)
        {
            Grid.Pager = new KachuwaPager(100,1);
            builder(Grid.Pager);
            

            return this;
        }
        public virtual IHtmlGrid<T> Pageable()
        {
            return Pageable(builder => { });
        }
        public virtual IHtmlGrid<T> AddCommands(Action<IKachuwaGridCommandsOf<T>> builder)
        {
            builder(Grid.Commands);

            return this;
        }
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            Html.Partial(PartialViewName, Grid).WriteTo(writer, encoder);
        }


    }
}