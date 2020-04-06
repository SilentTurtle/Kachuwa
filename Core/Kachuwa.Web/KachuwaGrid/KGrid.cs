using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Kachuwa.Grid;
using Kachuwa.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kachuwa.Web.Grid
{
    public interface IKachuwaGrid
    {
        string Name { get; set; }
        string NoDataText { get; set; }
        Func<dynamic, HelperResult> NoDataTemplate { get; set; }
        string CssClasses { get; set; }
        ViewContext ViewContext { get; set; }
        IQueryCollection Query { get; set; }

        IKachuwaGridColumns<IKachuwaGridColumn> Columns { get; }
        IKachuwaGridRows<Object> Rows { get; }
        IKachuwaGridCommands<IKachuwaGridCommand> Commands { get; }

        Pager Pager { get; set; }
        bool UseCardView { get; set; }
        string SearchBarClasses { get; set; }
        bool HideSearchBar { get; set; }
        bool HideSearcButton { get; set; }
        bool DisableRowSelection { get; set; }
    }

    public interface IKachuwaGrid<T> : IKachuwaGrid
    {
        IQueryable<T> Source { get; set; }
        new IKachuwaGridColumnsOf<T> Columns { get; }
        new IKachuwaGridRowsOf<T> Rows { get; }
        new IKachuwaGridCommandsOf<T> Commands { get; }
    }

    public class KachuwaGrid<T> : IKachuwaGrid<T> where T : class
    {
       
        public string Name { get; set; }
        public Func<dynamic, HelperResult> NoDataTemplate { get; set; }
        public string CssClasses { get; set; }
        public string FooterPartialViewName { get; set; }

        public IQueryable<T> Source { get; set; }
        public IQueryCollection Query { get; set; }
        public ViewContext ViewContext { get; set; }
      //  public IList<IGridProcessor<T>> Processors { get; set; }

        IKachuwaGridColumns<IKachuwaGridColumn> IKachuwaGrid.Columns => Columns;
        public IKachuwaGridColumnsOf<T> Columns { get; set; }

        IKachuwaGridRows<Object> IKachuwaGrid.Rows => Rows;
        

        public IKachuwaGridRowsOf<T> Rows { get; set; }
        public Pager Pager { get; set; }
        public bool UseCardView { get; set; } = false;
        public bool HideSearchBar { get; set; } = false;
        public bool HideSearcButton { get; set; } = false;
        public bool DisableRowSelection { get; set; } = false;

        public KachuwaGrid(IEnumerable<T> source)
        {
            //Processors = new List<IGridProcessor<T>>();
            Source = source.AsQueryable();
            Columns = new KachuwaGridColumns<T>(this);
            Rows = new KachuwaGridRows<T>(this);
            Commands = new KachuwaGridCommands<T>(this);
           var row = Rows.FirstOrDefault();
           int rowTotal=row==null?0: row.GetRowTotal();
           Pager = new Pager(rowTotal, 1);
        }
        public string NoDataText { get; set; }
        public string  SearchBarClasses { get; set; }

        IKachuwaGridCommands<IKachuwaGridCommand> IKachuwaGrid.Commands => Commands;
        public IKachuwaGridCommandsOf<T> Commands { get; set; }

        //IKachuwaGridCommands<IKachuwaGridCommand> IKachuwaGrid.Commands => Commands;
    }
}
