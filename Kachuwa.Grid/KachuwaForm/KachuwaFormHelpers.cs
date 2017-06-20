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

    }
}