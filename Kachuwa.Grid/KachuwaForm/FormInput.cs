using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Kachuwa.Form
{
    public interface IFormInput
    {
        string Id { get; set; }
        string Name { get; set; }
        string PlaceHolder { get; set; }
        string CssClasses { get; set; }
        bool IsEncoded { get; set; }
        object Value { get; set; }
        string Help { get; set; }
        IHtmlContent ValueFor(object model);

        FormInputControl InputType { get; set; }
        string DisplayName { get; set; }
        IEnumerable<FormInputItem> DataSource { get; set; }
        IHtmlContent RenderControlSource();
        //IHtmlContent RenderDataSource(object model);

    }
    public interface IFormInput<T> : IFormInput
    {
        IForm<T> Form { get; }
        LambdaExpression Expression { get; }
        Func<T, Object> RenderValue { get; set; }



    }
    public interface IFormInputs<out T> : IEnumerable<T> where T : IFormInput
    {
    }
    public interface IFormInputsOf<T> : IFormInputs<IFormInput<T>>
    {
        IForm<T> Form { get; }

        IFormInput<T> Add(string name, string classes);//final forms
        IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint);
        IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl);
        IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl, IEnumerable<FormInputItem> datasource);


    }

    public enum FormInputControl
    {
        TextBox, Password, Select, Radio, CheckBox, RadioList, CheckBoxList,
        Number, File, Image, Email, Url, Telephone, Date, DateTime, Color, TextArea, Editor, Hidden, Template
    }

    public abstract class BaseFormInput<T, TValue> : IFormInput<T>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PlaceHolder { get; set; }
        public string CssClasses { get; set; }
        public string Help { get; set; }
        public bool IsEncoded { get; set; }
        public object Value { get; set; }
        public string DisplayName { get; set; }
        public IForm<T> Form { get; set; }
        public FormInputControl InputType { get; set; } = FormInputControl.TextBox;
        public Func<T, object> RenderValue { get; set; }
        public abstract IHtmlContent ValueFor(object model);

        public Func<T, TValue> ExpressionValue { get; set; }
        LambdaExpression IFormInput<T>.Expression => Expression;
        public Expression<Func<T, TValue>> Expression { get; set; }


        public IEnumerable<FormInputItem> DataSource { get; set; }
        public abstract IHtmlContent RenderControlSource();
    }

    public class FormInput<T, TValue> : BaseFormInput<T, TValue> where T : class
    {
        public FormInput(IForm<T> form,
            string name, string classes)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            // Expression = expression;
            //ExpressionValue = expression.Compile();
            // Controls=new 
            DisplayName = Name;


        }
        public FormInput(IForm<T> form,
          string name, string classes,
          Expression<Func<T, TValue>> expression)
        {
            Form = form;
            Name = name;
            CssClasses = classes;
            //PlaceHolder = placeHolder;
            Expression = expression;
            ExpressionValue = expression.Compile();
            DisplayName = ExpressionHelper.GetExpressionText(expression);
            Id = this.Form.Name + "_" + this.Name;

        }
        public FormInput(IForm<T> form, Expression<Func<T, TValue>> expression, string classes)
        {
            Form = form;
            //Name = name;
            CssClasses = classes;
            //PlaceHolder = placeHolder;
            //Value = value;
            Expression = expression;
            ExpressionValue = expression.Compile();
            DisplayName = ExpressionHelper.GetExpressionText(expression);
            Name = DisplayName;
            Id = this.Form.Name + "_" + this.Name;

        }

        public FormInput(IForm<T> form, Expression<Func<T, TValue>> expression, string classes,
            FormInputControl inputType)
        {
            Form = form;
            CssClasses = classes;
            Expression = expression;
            ExpressionValue = expression.Compile();
            DisplayName = ExpressionHelper.GetExpressionText(expression);
            InputType = inputType;
            Name = DisplayName;
            Id = this.Form.Name + "_" + this.Name;
        }
        public FormInput(IForm<T> form, Expression<Func<T, TValue>> expression, string classes,
          FormInputControl inputType, IEnumerable<FormInputItem> dataSource)
        {
            Form = form;
            CssClasses = classes;
            Expression = expression;
            ExpressionValue = expression.Compile();
            DisplayName = ExpressionHelper.GetExpressionText(expression);
            InputType = inputType;
            Name = DisplayName;
            Id = this.Form.Name + "_" + this.Name;
            DataSource = dataSource;
        }



        public override IHtmlContent RenderControlSource()
        {
            StringBuilder sb = new StringBuilder();

            if (this.DataSource == null)
                return null;
            else
            {
                foreach (var item in this.DataSource)
                {
                    if (this.InputType == FormInputControl.Select)
                        sb.AppendFormat("<option id='{0}' value='{1}'>{2}</option>", item.Id, item.Id, item.Label);
                }
            }

            Object value = sb.ToString();
            if (value == null) return HtmlString.Empty;
            //if (value is IHtmlContent)
            //    return value as IHtmlContent;
            return new HtmlString(value.ToString());
        }
        private Object GetValueFor(object model)
        {
            try
            {
                if (RenderValue != null)
                    return RenderValue(model as T);

                return ExpressionValue(model as T);

            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
        public override IHtmlContent ValueFor(object model)
        {
            Object value = GetValueFor(model);
            if (value == null)
                return HtmlString.Empty;
            if (value is IHtmlContent)
                return value as IHtmlContent;
            //if (Format != null)
            //{
            //    if (System.Type.GetTypeCode(value.GetType()) == TypeCode.Object)
            //    {
            //        FormatCompiler compiler = new FormatCompiler();
            //        Generator generator = compiler.Compile(Format);
            //        string result = generator.Render(value);
            //        value = result;
            //    }
            //    else
            //    {
            //        value = string.Format(Format, value);
            //    }
            //}
            if (IsEncoded)
                return new HtmlString(HtmlEncoder.Default.Encode(value.ToString()));

            return new HtmlString(value.ToString());
        }



    }


    public class FormInputs<T> : List<IFormInput<T>>, IFormInputsOf<T> where T : class
    {
        public FormInputs(IForm<T> form)
        {
            Form = form;
        }
        public FormInputs()
        {

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IForm<T> Form { get; set; }
        public IFormInput<T> Add(string name, string classes)
        {
            IFormInput<T> inputControl = new FormInput<T, Object>(Form, name, classes);
            Add(inputControl);

            return inputControl;
        }

        public IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint)
        {
            IFormInput<T> inputControl = new FormInput<T, TValue>(Form, constraint, classes);
            Add(inputControl);

            return inputControl;
        }

        public IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl)
        {
            IFormInput<T> inputControl = new FormInput<T, TValue>(Form, constraint, classes, formControl);
            Add(inputControl);
            return inputControl;
        }
        public IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl, IEnumerable<FormInputItem> datasource)
        {
            IFormInput<T> inputControl = new FormInput<T, TValue>(Form, constraint, classes, formControl, datasource);
            Add(inputControl);
            return inputControl;
        }


    }

}
