using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

namespace Kachuwa.Web.Grid
{

    public interface IKachuwaGridCommand
    {
        string Name { get; set; }
        string Command { get; set; }
        string CssClasses { get; set; }
        string IconClass { get; set; }
        string ClientCallback { get; set; }
        string ClientCallbackUrl { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        IHtmlContent ValueFor(IKachuwaGridRow<Object> row);
    }
    public interface IKachuwaGridCommand<T> : IKachuwaGridCommand
    {
        IKachuwaGrid<T> Grid { get; }
        LambdaExpression Expression { get; }
        Func<T, Object> RenderValue { get; set; }
    }
    public interface IKachuwaGridCommands<out T> : IEnumerable<T> where T : IKachuwaGridCommand
    {
    }
    public interface IKachuwaGridCommandsOf<T> : IKachuwaGridCommands<IKachuwaGridCommand<T>>
    {
        IKachuwaGrid<T> Grid { get; set; }

        IKachuwaGridCommand<T> Add<TValue>(string name, string command, string callback);
        IKachuwaGridCommand<T> Add<TValue>(string name, string command, string iconClass, string callback );
        IKachuwaGridCommand<T> Add<TValue>(string name, string command, string iconClass, string callback,  Expression<Func<T, TValue>> callbackParam, string callBackUrl);

        IKachuwaGridCommand<T> Add<TValue>(string name, string command, string iconClass, string callback,  string action, Expression<Func<T, TValue>> expression);
    }


    public abstract class BaseKachuwaGridCommand<T, TValue> : IKachuwaGridCommand<T>
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public string CssClasses { get; set; }
        public string IconClass { get; set; }
        public string ClientCallback { get; set; }
        public string ClientCallbackUrl { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; } = "#";
        public IKachuwaGrid<T> Grid { get; set; }
        public Func<T, Object> RenderValue { get; set; }
        public Func<T, TValue> ExpressionValue { get; set; }
        LambdaExpression IKachuwaGridCommand<T>.Expression => Expression;
        public Expression<Func<T, TValue>> Expression { get; set; }
        public abstract IHtmlContent ValueFor(IKachuwaGridRow<Object> row);

    }


    public class KachuwaGridCommand<T, TValue> : BaseKachuwaGridCommand<T, TValue> where T : class
    {

        public KachuwaGridCommand(IKachuwaGrid<T> grid, string name, string command, string callback)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;

        }
        public KachuwaGridCommand(IKachuwaGrid<T> grid, string name, string command, string callback, string iconClass)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;
            IconClass = iconClass;

        }
        public KachuwaGridCommand(IKachuwaGrid<T> grid, string name, string command, string callback, string iconClass, string action, Expression<Func<T, TValue>> expression)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;
            IconClass = iconClass;
            Action = action;
            Expression = expression;
            ExpressionValue = expression.Compile();

        }
        public KachuwaGridCommand(IKachuwaGrid<T> grid, string name, string command, string callback, string iconClass, string action, Expression<Func<T, TValue>> expression, string callbackUrl)
        {
            Grid = grid;
            Name = name;
            Command = command;
            ClientCallback = callback;
            IconClass = iconClass;
            Action = action;
            Expression = expression;
            ExpressionValue = expression.Compile();
            ClientCallbackUrl = callbackUrl;

        }

        public override IHtmlContent ValueFor(IKachuwaGridRow<Object> row)
        {
            string value = GetValueFor(row);
            if (value == null)
                return HtmlString.Empty;

            return new HtmlString(value.ToString());
        }

        private string GetValueFor(IKachuwaGridRow<Object> row)
        {
            try
            {
                if (RenderValue != null)
                    return RenderValue(row.Model as T).ToString();

                var value = ExpressionValue(row.Model as T);
                //for client call back param
                if (value.GetType().GetTypeInfo().IsClass)
                {
                    return JsonConvert.SerializeObject(value);
                }
                else
                {//redirect action url with value
                    if (string.IsNullOrEmpty(this.ClientCallback))
                        return $"{this.Action}/{value}";
                    else
                        return $"{value}";
                }
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

        public IKachuwaGridCommand<T> Add<TValue>(string name, string command, string callback)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T, TValue>(Grid, name, command, callback);
            Add(column);

            return column;
        }

        public IKachuwaGridCommand<T> Add<TValue>(string name, string command, string iconClass,string callback)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T, TValue>(Grid, name, command, callback, iconClass);
            Add(column);

            return column;
        }
        public IKachuwaGridCommand<T> Add<TValue>(string name, string command, string iconClass, string callback, Expression<Func<T, TValue>> callbackParam, string callBackUrl)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T, TValue>(Grid, name, command, callback, iconClass, "", callbackParam, callBackUrl);

            Add(column);

            return column;
        }
        public IKachuwaGridCommand<T> Add<TValue>(string name, string command, string iconClass, string callback,  string action, Expression<Func<T, TValue>> expression)
        {
            IKachuwaGridCommand<T> column = new KachuwaGridCommand<T, TValue>(Grid, name, command, callback, iconClass, action, expression);

            Add(column);

            return column;
        }


    }
}
