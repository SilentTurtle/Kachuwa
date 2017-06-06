using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Kachuwa.Grid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Kachuwa.KGrid
{
    public interface IKachuwaGrid
    {
        String Name { get; set; }
        String NoDataText { get; set; }
        String CssClasses { get; set; }
        ViewContext ViewContext { get; set; }
        IQueryCollection Query { get; set; }

        IKachuwaGridColumns<IKachuwaGridColumn> Columns { get; }
        IKachuwaGridRows<Object> Rows { get; }
        IKachuwaGridCommands<IKachuwaGridCommand> Commands { get; }

        KachuwaPager Pager { get; set; }
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
       
        public String Name { get; set; }
       
        public String CssClasses { get; set; }
        public String FooterPartialViewName { get; set; }

        public IQueryable<T> Source { get; set; }
        public IQueryCollection Query { get; set; }
        public ViewContext ViewContext { get; set; }
      //  public IList<IGridProcessor<T>> Processors { get; set; }

        IKachuwaGridColumns<IKachuwaGridColumn> IKachuwaGrid.Columns => Columns;
        public IKachuwaGridColumnsOf<T> Columns { get; set; }

        IKachuwaGridRows<Object> IKachuwaGrid.Rows => Rows;
        

        public IKachuwaGridRowsOf<T> Rows { get; set; }
        public KachuwaPager Pager { get; set; }

        public KachuwaGrid(IEnumerable<T> source)
        {
            //Processors = new List<IGridProcessor<T>>();
            Source = source.AsQueryable();
            Columns = new KachuwaGridColumns<T>(this);
            Rows = new KachuwaGridRows<T>(this);
            Commands = new KachuwaGridCommands<T>(this);
            Pager=new KachuwaPager(100,1);
        }
        public String NoDataText { get; set; }
      
        IKachuwaGridCommands<IKachuwaGridCommand> IKachuwaGrid.Commands => Commands;
        public IKachuwaGridCommandsOf<T> Commands { get; set; }

        //IKachuwaGridCommands<IKachuwaGridCommand> IKachuwaGrid.Commands => Commands;
    }
}
