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
using Kachuwa.Form;
using Microsoft.SqlServer.Server;
using Mustache;

namespace Kachuwa.KGrid
{
    public static class KaxhuwaGridExtensions
    {
        public static object GetPriamaryKey(this IKachuwaGridRow<Object> row)
        {
            object value = 0;
            var Name = "";
            foreach (var prop in row.Model.GetType().GetProperties())
            {

                if (prop.GetCustomAttribute<KeyAttribute>() != null)
                {
                    System.TypeCode typeCode = System.Type.GetTypeCode(prop.PropertyType);
                    if (typeCode == TypeCode.Int64)
                    {
                        value = (long)prop.GetValue(row.Model);
                    }
                    else
                    {
                        value = (int)prop.GetValue(row.Model);
                    }

                }
            }
            return value;

        }
        public static int GetRowTotal(this IKachuwaGridRow<Object> row)
        {
            int value = 0;
            foreach (var prop in row.Model.GetType().GetProperties())
            {

                if (prop.Name.ToLower()=="rowtotal")
                {
                    value = (int)prop.GetValue(row.Model);
                   
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

        public static KachuwaHtmlForm<T> CreateKachuwaForm<T>(this IHtmlHelper html,string name) where T : class, new()
        {
            return new KachuwaHtmlForm<T>(html, new KachuwaForm<T>(name));
        }
        public static KachuwaHtmlForm<T> CreateKachuwaForm<T>(this IHtmlHelper html,string name,T modalObj) where T : class, new()
        {
            return new KachuwaHtmlForm<T>(html, new KachuwaForm<T>(name,modalObj));
        }
        public static T SetFormHeading<T>(this T form, string heading) where T : IForm<T>
        {
            form.Heading = heading;
            return form;
        }
        public static string Render<T>(this IKachuwaHtmlForm<T> form) where T : class
        {
            var res = RenderForm(form);
            return res;
        }

        private static string RenderForm(Object obj) 
        {
            string template =
                "<div id=\"{{Name}}\" class=\"kachuwa-form\">< form name =\"{{Name}}\" class=\"{{CssClasses}}\"></form></div>";

            FormatCompiler compiler = new FormatCompiler();
            Generator generator = compiler.Compile(template);
            string result = generator.Render(obj);
            return result;
        }

    }
}