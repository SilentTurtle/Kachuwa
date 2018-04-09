using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;

namespace Kachuwa.Form
{

    public static class KachuwaFormHelpers
    {

        public static T SetHeading<T>(this T section, string heading) where T : IFormSection
        {
            section.Heading = heading;

            return section;
        }
        public static T SetSubHeading<T>(this T section, string heading) where T : IFormSection
        {
            section.SubHeading = heading;

            return section;
        }
        public static T SetHelpLine<T>(this T section, string helpText) where T : IFormSection
        {
            section.HelpLine = helpText;

            return section;
        }

        public static T SetId<T>(this T input, string id) where T : IFormInput
        {
            input.Id = id;

            return input;
        }
        public static T SetName<T>(this T input, string name) where T : IFormInput
        {
            input.Name = name;

            return input;
        }
        public static T SetDisplayName<T>(this T input, string name) where T : IFormInput
        {
            input.DisplayName = name;

            return input;
        }
        public static T SetPlaceHolder<T>(this T input, string placeholder) where T : IFormInput
        {
            input.PlaceHolder = placeholder;

            return input;
        }
        public static T Encode<T>(this T input, bool isEncoded) where T : IFormInput
        {
            input.IsEncoded = isEncoded;

            return input;
        }
        //public static IFormInput SetDataSource<T,T2>(this T input, FormDatasource source, string key, Expression<Func<T2, FormInputItem>> action) where T : IFormInput<T>
        //{
        //    //input.IsEncoded = isEncoded;
        //    var src = source.GetSource(key);
        //    input.DataSourceExpression=action;
        //    input.DataSource = src;
        //    return input;
        //}
        public static T SetValue<T>(this T input, object value) where T : IFormInput
        {
            input.Value = value;
            return input;
        }
        public static T SetHelp<T>(this T input, string helpText) where T : IFormInput
        {
            input.Help = helpText;
            return input;
        }
        public static T SetFirstParentClass<T>(this T input, string parentClass) where T : IFormInput
        {
            input.Parent1Class = parentClass;
            return input;
        }
        public static T SetSecondParentClass<T>(this T input, string parentClass) where T : IFormInput
        {
            input.Parent2Class = parentClass;
            return input;
        }
        public static T SetThirdParentClass<T>(this T input, string parentClass) where T : IFormInput
        {
            input.Parent3Class = parentClass;
            return input;
        }

        public static T SetFirstChildClass<T>(this T column, string level1Class) where T : IFormColumn
        {
            column.Child1Class = level1Class;
            return column;
        }
        public static T SetSecondChildClass<T>(this T column, string level2Class) where T : IFormColumn
        {
            column.Child2Class = level2Class;
            return column;
        }
        public static T SetThridChildClass<T>(this T column, string level3Class) where T : IFormColumn
        {
            column.Child3Class = level3Class;
            return column;
        }


    }
}