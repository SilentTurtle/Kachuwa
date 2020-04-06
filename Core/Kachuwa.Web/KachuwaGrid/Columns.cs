using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Mustache;

namespace Kachuwa.Web.Grid
{
    public interface IKachuwaGridColumn
    {
        string Name { get; set; }
        string Format { get; set; }
        string CssClasses { get; set; }
        bool IsEncoded { get; set; }
        IHtmlContent Title { get; set; }
        IHtmlContent ValueFor(IKachuwaGridRow<Object> row);
    }
    public interface IKachuwaGridColumn<T> : IKachuwaGridColumn
    {
        IKachuwaGrid<T> Grid { get; }
        LambdaExpression Expression { get; }
        Func<T, Object> RenderValue { get; set; }
    }
    public interface IKachuwaGridColumns<out T> : IEnumerable<T> where T : IKachuwaGridColumn
    {
    }
    public interface IKachuwaGridColumnsOf<T> : IKachuwaGridColumns<IKachuwaGridColumn<T>>
    {
        IKachuwaGrid<T> Grid { get; set; }

        IKachuwaGridColumn<T> Add();
        IKachuwaGridColumn<T> Add<TValue>(Expression<Func<T, TValue>> constraint);

        IKachuwaGridColumn<T> Insert(Int32 index);
        IKachuwaGridColumn<T> Insert<TValue>(Int32 index, Expression<Func<T, TValue>> constraint);
    }


    public abstract class BaseKachuwaGridColumn<T, TValue> : IKachuwaGridColumn<T>
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public string CssClasses { get; set; }
        public bool IsEncoded { get; set; }
        public IHtmlContent Title { get; set; }
        public IKachuwaGrid<T> Grid { get; set; }
        public Func<T, Object> RenderValue { get; set; }
        public Func<T, TValue> ExpressionValue { get; set; }
        LambdaExpression IKachuwaGridColumn<T>.Expression => Expression;
        public Expression<Func<T, TValue>> Expression { get; set; }

        public abstract IQueryable<T> Process(IQueryable<T> items);
        public abstract IHtmlContent ValueFor(IKachuwaGridRow<Object> row);
    }


    public class KachuwaGridColumn<T, TValue> : BaseKachuwaGridColumn<T, TValue> where T : class
    {
       // private readonly Object _obj;

        public KachuwaGridColumn(IKachuwaGrid<T> grid, Expression<Func<T, TValue>> expression)
        {
            Grid = grid;
            IsEncoded = true;
            Expression = expression;
            Title = GetTitle(expression);
            ExpressionValue = expression.Compile();
            var expresionProvider = ContextResolver.Context.RequestServices
           .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            Name = expresionProvider.GetExpressionText(expression);
        }

        public override IQueryable<T> Process(IQueryable<T> items)
        {
            return items.OrderByDescending(Expression);
        }
        public override IHtmlContent ValueFor(IKachuwaGridRow<Object> row)
        {
            Object value = GetValueFor(row);
            if (value == null) return HtmlString.Empty;
            if (value is IHtmlContent) return value as IHtmlContent;
            if (Format != null)
            {
                if (System.Type.GetTypeCode(value.GetType()) == TypeCode.Object)
                {
                    FormatCompiler compiler = new FormatCompiler();
                    Generator generator = compiler.Compile(Format);
                    string result = generator.Render(value);
                    value = result;
                }
                else
                {
                    value = string.Format(Format, value);
                }
            }
            if (IsEncoded)
                return new HtmlString(HtmlEncoder.Default.Encode(value.ToString()));

            return new HtmlString(value.ToString());
        }

        

        private IHtmlContent GetTitle(Expression<Func<T, TValue>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            DisplayAttribute display = body?.Member.GetCustomAttribute<DisplayAttribute>();

            return new HtmlString(display?.GetShortName());
        }
        private Object GetValueFor(IKachuwaGridRow<Object> row)
        {
            try
            {
                if (RenderValue != null)
                    return RenderValue(row.Model as T);

                return ExpressionValue(row.Model as T);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }

    public class KachuwaGridColumns<T> : List<IKachuwaGridColumn<T>>, IKachuwaGridColumnsOf<T> where T : class
    {
        public IKachuwaGrid<T> Grid { get; set; }

        public KachuwaGridColumns(IKachuwaGrid<T> grid)
        {
            Grid = grid;
        }

        public virtual IKachuwaGridColumn<T> Add()
        {
            return Add<Object>(model => null);
        }
        public virtual IKachuwaGridColumn<T> Add<TValue>(Expression<Func<T, TValue>> expression)
        {
            IKachuwaGridColumn<T> column = new KachuwaGridColumn<T, TValue>(Grid, expression);
            //Grid.Processors.Add(column);
            Add(column);

            return column;
        }
        //public virtual Object Add(Object obj)
        //{
        //    IKachuwaGridColumn<T> column = new KachuwaGridColumn<T, object>(Grid, obj);
        //    //Grid.Processors.Add(column);
        //    Add(column);

        //    return column;
        //}


        public virtual IKachuwaGridColumn<T> Insert(Int32 index)
        {
            return Insert<Object>(index, model => null);
        }
        public virtual IKachuwaGridColumn<T> Insert<TValue>(Int32 index, Expression<Func<T, TValue>> expression)
        {
            IKachuwaGridColumn<T> column = new KachuwaGridColumn<T, TValue>(Grid, expression);
            //  Grid.Processors.Add(column);
            Insert(index, column);

            return column;
        }
    }
}
