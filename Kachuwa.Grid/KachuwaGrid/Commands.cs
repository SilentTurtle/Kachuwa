using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Kachuwa.Grid;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.SqlServer.Server;

namespace Kachuwa.KGrid
{

    public interface IKachuwaGridCommand
    {
        String Name { get; set; }
        String Command { get; set; }
        String CssClasses { get; set; }
        String IconClass { get; set; }
        String ClientCallback { get; set; }
        String Controller { get; set; }
        String Action { get; set; }
    }
    public interface IKachuwaGridCommand<T> : IKachuwaGridCommand
    {
        IKachuwaGrid<T> Grid { get; }
        // LambdaExpression Expression { get; }
        Func<T, Object> RenderValue { get; set; }
    }
    public interface IKachuwaGridCommands<out T> : IEnumerable<T> where T : IKachuwaGridCommand
    {
    }
    public interface IKachuwaGridCommandsOf<T> : IKachuwaGridCommands<IKachuwaGridCommand<T>>
    {
        IKachuwaGrid<T> Grid { get; set; }

        IKachuwaGridCommand<T> Add(String name, String command, String callback);
        IKachuwaGridCommand<T> Add(String name, String command, String callback, String iconClass);
        IKachuwaGridCommand<T> Add(String name, String command, String callback, String iconClass, String controller, String action);
    }


    public abstract class BaseKachuwaGridCommand<T> : IKachuwaGridCommand<T>
    {
        public String Name { get; set; }
        public String Command { get; set; }
        public String CssClasses { get; set; }
        public String IconClass { get; set; }
        public String ClientCallback { get; set; }
        public String Controller { get; set; }
        public String Action { get; set; }
        public IKachuwaGrid<T> Grid { get; set; }
        public Func<T, Object> RenderValue { get; set; }
        //  public Func<T, TValue> ExpressionValue { get; set; }
        // LambdaExpression IKachuwaGridCommand<T>.Expression => Expression;
        // public Expression<Func<T, TValue>> Expression { get; set; }
        public abstract IHtmlContent ValueFor(IKachuwaGridRow<Object> row);

    }


    public class KachuwaGridCommand<T> : BaseKachuwaGridCommand<T> where T : class
    {

        public KachuwaGridCommand(IKachuwaGrid<T> grid, String name, String command, String callback)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;

        }
        public KachuwaGridCommand(IKachuwaGrid<T> grid, String name, String command, String callback, String iconClass)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;
            IconClass = iconClass;

        }
        public KachuwaGridCommand(IKachuwaGrid<T> grid, String name, String command, String callback, String iconClass, String controller, String action)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;
            IconClass = iconClass;
            Action = action;
            Controller = controller;

        }

        public override IHtmlContent ValueFor(IKachuwaGridRow<Object> row)
        {
            Object value = GetValueFor(row);
            if (value == null) return HtmlString.Empty;
            if (value is IHtmlContent) return value as IHtmlContent;
            return new HtmlString(value.ToString());
        }

        private Object GetValueFor(IKachuwaGridRow<Object> row)
        {
            try
            {
                if (RenderValue != null)
                    return RenderValue(row.Model as T);

                return null;//ExpressionValue(row.Model as T);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

    }

    public class KachuwaGridCommands<T> : List<IKachuwaGridCommand<T>>, IKachuwaGridCommandsOf<T> where T : class
    {
        public IKachuwaGrid<T> Grid { get; set; }

        public KachuwaGridCommands(IKachuwaGrid<T> grid)
        {
            Grid = grid;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IKachuwaGridCommand<T> Add(String name, String command, String callback)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T>(Grid, name, command, callback);
            Add(column);

            return column;
        }

        public IKachuwaGridCommand<T> Add(String name, String command, String callback, String iconClass)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T>(Grid, name, command, callback, iconClass);
            Add(column);

            return column;
        }

        public IKachuwaGridCommand<T> Add(String name, String command, String callback, String iconClass, String controller, String action)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T>(Grid, name, command, callback, iconClass, controller, action);

            Add(column);

            return column;
        }
    }
}
