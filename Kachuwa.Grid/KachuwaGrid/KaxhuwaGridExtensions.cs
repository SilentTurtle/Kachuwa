using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Kachuwa.Data;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.KGrid
{
    public static class KaxhuwaGridExtensions
    {
        public static int GetPriamaryKey(this IKachuwaGridRow<Object> row)
        {
            int value = 0;
            var Name = "";
            foreach (var prop in row.Model.GetType().GetProperties())
            { 

                if (prop.GetCustomAttribute<Kachuwa.Data.Crud.Attribute.KeyAttribute>() != null)
                {
                    value=(int) prop.GetValue(row.Model);
                    Name = prop.Name;
                }
            }
            return value;

        }

        public static IKachuwaGridColumn<TModel> RenderedAs<TModel>(this IKachuwaGridColumn<TModel> column, Func<TModel, Object> value)
        {
            column.RenderValue = value;

            return column;
        }

        public static T Encoding<T>(this T column, Boolean isEncoded) where T : IKachuwaGridColumn
        {
            column.IsEncoded = isEncoded;

            return column;
        }
        public static T Template<T>(this T column, String format) where T : IKachuwaGridColumn
        {
            column.Format = format;

            return column;
        }
        public static T Css<T>(this T column, String cssClasses) where T : IKachuwaGridColumn
        {
            column.CssClasses = cssClasses;

            return column;
        }
        public static T SetTitle<T>(this T column, Object value) where T : IKachuwaGridColumn
        {
            column.Title = value as IHtmlContent ?? new HtmlString(value?.ToString());

            return column;
        }
        public static T Named<T>(this T column, String name) where T : IKachuwaGridColumn
        {
            column.Name = name;

            return column;
        }
        public static KachuwaHtmlGrid<T> CreateKachuwaGrid<T>(this IHtmlHelper html, IEnumerable<T> source) where T : class
        {
            return new KachuwaHtmlGrid<T>(html, new KachuwaGrid<T>(source));
        }
        public static KachuwaHtmlGrid<T> CreateKachuwaGrid<T>(this IHtmlHelper html, String partialViewName, IEnumerable<T> source) where T : class
        {
            return new KachuwaHtmlGrid<T>(html, new KachuwaGrid<T>(source)) { PartialViewName = partialViewName };
        }
       

       
    }
}