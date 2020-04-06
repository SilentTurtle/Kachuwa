﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using Kachuwa.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Kachuwa.Web.Form
{
   

    public static class HtmlHelperExtensions
    {
        public static string GetExpressionText<TModel, TResult>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression)
        {
            var expresionProvider = htmlHelper.ViewContext.HttpContext.RequestServices
                .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;

            return expresionProvider.GetExpressionText(expression);
        }
    }
    public interface IFormInput
    {
        string Id { get; set; }
        string Name { get; set; }
        string PlaceHolder { get; set; }
        string CssClasses { get; set; }
        string Parent1Class { get; set; }
        string Parent2Class { get; set; }
        string Parent3Class { get; set; }
        bool IsEncoded { get; set; }
        object Value { get; set; }
        string Help { get; set; }
        IHtmlContent ValueFor(object model);
        IDictionary<string, object> Attributes { get; set; }
        IHtmlContent RenderAttributes(ViewContext viewContext);
        FormInputControl InputType { get; set; }
        string DisplayName { get; set; }
        IEnumerable<FormInputItem> DataSource { get; set; }
        IHtmlContent RenderControlSource(object model);
        //IHtmlContent RenderDataSource(object model);
        IHtmlContent Template { get; set; }
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
        IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl, object attributes = null);
        IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl, IEnumerable<FormInputItem> datasource, object attributes = null);
        IFormInput<T> Add<TValue>(Func<dynamic, HelperResult> template);

    }

    public enum FormInputControl
    {
        TextBox, Password, Select, Radio, CheckBox, RadioList, CheckBoxList,
        Number, File, Image, Email, Url, Telephone, Date, DateTime, Color, TextArea, Editor, Hidden, Tag, Template, Switch,ImageViewer
    }

    public abstract class BaseFormInput<T, TValue> : IFormInput<T>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PlaceHolder { get; set; }
        public string CssClasses { get; set; }
        public string Parent1Class { get; set; }
        public string Parent2Class { get; set; }
        public string Parent3Class { get; set; }
        public string Help { get; set; }
        public bool IsEncoded { get; set; }
        public object Value { get; set; }
        public string DisplayName { get; set; }
        public IForm<T> Form { get; set; }
        public IDictionary<string, object> Attributes { get; set; }
        public FormInputControl InputType { get; set; } = FormInputControl.TextBox;
        public Func<T, object> RenderValue { get; set; }
        public abstract IHtmlContent ValueFor(object model);

        public Func<T, TValue> ExpressionValue { get; set; }
        LambdaExpression IFormInput<T>.Expression => Expression;
        public Expression<Func<T, TValue>> Expression { get; set; }


        public IEnumerable<FormInputItem> DataSource { get; set; }
        public abstract IHtmlContent RenderControlSource(object model);
        public abstract IHtmlContent RenderAttributes(ViewContext viewContext);
        public IHtmlContent Template { get; set; }
    }

    public class FormInput<T, TValue> : BaseFormInput<T, TValue> where T : class
    {
        private IModelMetadataProvider _metadataProvider;
        private ClientValidatorCache _clientValidatorCache;
        private IClientModelValidatorProvider _clientModelValidatorProvider;
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
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);

        }
        public FormInput(IForm<T> form, Func<dynamic, HelperResult> template)
        {
            Form = form;
            Name = "";
            CssClasses = "";
            Template = template.Invoke(null);
            // Expression = expression;
            //ExpressionValue = expression.Compile();
            // Controls=new 
            DisplayName = Name;
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);

        }
        private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
        {
            IDictionary<string, object> htmlAttributeDictionary = null;
            if (htmlAttributes != null)
            {
                htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
                if (htmlAttributeDictionary == null)
                {
                    htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    //already defined in input class ie removing
                    if (htmlAttributeDictionary.ContainsKey("id"))
                        htmlAttributeDictionary.Remove("id");
                    if (htmlAttributeDictionary.ContainsKey("class"))
                        htmlAttributeDictionary.Remove("class");
                    if (htmlAttributeDictionary.ContainsKey("placeholder"))
                        htmlAttributeDictionary.Remove("placeholder");
                    if (htmlAttributeDictionary.ContainsKey("value"))
                        htmlAttributeDictionary.Remove("value");
                }
            }

            return htmlAttributeDictionary;
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
            var expresionProvider = ContextResolver.Context.RequestServices
          .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            DisplayName = expresionProvider.GetExpressionText(expression);
            Id = this.Form.Name + "_" + this.Name;
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);

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
            var expresionProvider = ContextResolver.Context.RequestServices
            .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            DisplayName = expresionProvider.GetExpressionText(expression);// ExpressionHelper.GetExpressionText(expression);
            Name = DisplayName;
            Id = this.Form.Name + "_" + this.Name;
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));

            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);

        }

        public FormInput(IForm<T> form, Expression<Func<T, TValue>> expression, string classes,
            FormInputControl inputType, object attributes)
        {
            Form = form;
            CssClasses = classes;
            Expression = expression;
            ExpressionValue = expression.Compile();
            var expresionProvider = ContextResolver.Context.RequestServices
          .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            DisplayName = expresionProvider.GetExpressionText(expression);
            InputType = inputType;
            Name = DisplayName;
            Id = this.Form.Name + "_" + this.Name;
            Attributes = GetHtmlAttributeDictionaryOrNull(attributes);
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);
        }
        public FormInput(IForm<T> form, Expression<Func<T, TValue>> expression, string classes,
          FormInputControl inputType, IEnumerable<FormInputItem> dataSource, object attributes)
        {
            Form = form;
            CssClasses = classes;
            Expression = expression;
            ExpressionValue = expression.Compile();
            var expresionProvider = ContextResolver.Context.RequestServices
        .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            DisplayName = expresionProvider.GetExpressionText(expression);
            InputType = inputType;
            Name = DisplayName;
            Id = this.Form.Name + "_" + this.Name;
            DataSource = dataSource;
            Attributes = GetHtmlAttributeDictionaryOrNull(attributes);
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);
        }

        public override IHtmlContent RenderAttributes(ViewContext viewContext)
        {
            var expresionProvider = ContextResolver.Context.RequestServices
         .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;

            var modelExpression = expresionProvider.CreateModelExpression(new ViewDataDictionary<T>(_metadataProvider, new ModelStateDictionary()), Expression);
            var validationAttributes = new AttributeDictionary() {};
            AddValidationAttributes(viewContext, modelExpression.ModelExplorer, validationAttributes);

            TextWriter writer = new StringWriter();
            if (validationAttributes.Count > 0)
            {
                foreach (var attribute in validationAttributes)
                {
                    var key = attribute.Key;
                    writer.Write(" ");
                    writer.Write(key);
                    writer.Write("=\"");
                    if (attribute.Value != null)
                    {
                        HtmlEncoder.Default.Encode(writer, attribute.Value.ToString());
                    }

                    writer.Write("\"");
                }
                return new HtmlString(writer.ToString());
            }
            if (this.Attributes != null && this.Attributes.Count > 0)
            {
                foreach (var attribute in Attributes)
                {
                    var key = attribute.Key;
                    writer.Write(" ");
                    writer.Write(key);
                    writer.Write("=\"");
                    if (attribute.Value != null)
                    {
                        HtmlEncoder.Default.Encode(writer, attribute.Value.ToString());
                    }

                    writer.Write("\"");
                }
                return new HtmlString(writer.ToString());
            }
            return null;
        }
      
        public  void AddValidationAttributes(
          ViewContext viewContext,
          ModelExplorer modelExplorer,
          IDictionary<string, string> attributes)
        {

            
           

            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            if (modelExplorer == null)
            {
                throw new ArgumentNullException(nameof(modelExplorer));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            var formContext = viewContext.ClientValidationEnabled ? viewContext.FormContext : null;
            if (formContext == null)
            {
                return;
            }

            var validators = _clientValidatorCache.GetValidators(
                modelExplorer.Metadata,
                _clientModelValidatorProvider);
            if (validators.Count > 0)
            {
                var validationContext = new ClientModelValidationContext(
                    viewContext,
                    modelExplorer.Metadata,
                    _metadataProvider,
                    attributes);

                for (var i = 0; i < validators.Count; i++)
                {
                    var validator = validators[i];
                    validator.AddValidation(validationContext);
                }
            }
        }


        public override IHtmlContent RenderControlSource(object model)
        {
            StringBuilder sb = new StringBuilder();

            if (this.DataSource == null)
                return null;
            else
            {
                var controlValue = GetValueFor(model);

                if (this.InputType == FormInputControl.Select)
                    sb.AppendFormat("<option value='{0}'>{1}</option>", "0", "Select...");

                if (controlValue == null)
                {
                    if (this.InputType == FormInputControl.Select)
                    {

                        foreach (var item in this.DataSource)
                        {
                            if (!item.IsSelected)
                            {

                                sb.AppendFormat("<option id='{0}' value='{1}'>{2}</option>", item.Id, item.Value,
                                    item.Label);
                            }
                            else
                            {
                                sb.AppendFormat("<option id='{0}' selected='selected' value='{1}'>{2}</option>",
                                        item.Id,
                                        item.Value, item.Label);
                            }
                        }


                    }
                    else if (this.InputType == FormInputControl.CheckBoxList)
                    {
                        foreach (var item in this.DataSource)
                        {
                            if (item.IsSelected)
                            {
                                sb.AppendFormat(
                                    "<div class='checkbox'><label><input type='checkbox' name='{0}' checked='checked' value='{1}'>{2}</label></div> ",
                                    this.Name, item.Id, item.Label);

                            }
                            else
                            {
                                sb.AppendFormat(
                                "<div class='checkbox'><label><input type='checkbox' name='{0}' value='{1}'>{2}</label></div> ",
                                this.Name, item.Id, item.Label);
                            }

                        }

                    }

                }

                else
                {
                    System.TypeCode typeCode = System.Type.GetTypeCode(controlValue.GetType());

                    foreach (var item in this.DataSource)
                    {


                        //if col value is int
                        if (typeCode == TypeCode.Int32)
                        {
                            if (controlValue.ToString() == item.Id.ToString())
                            {
                                if (this.InputType == FormInputControl.Select)
                                {
                                    sb.AppendFormat("<option id='{0}' selected='selected' value='{1}'>{2}</option>",
                                        item.Id,
                                        item.Value, item.Label);
                                }

                            }
                            else
                            {
                                if (this.InputType == FormInputControl.Select)
                                {
                                    sb.AppendFormat("<option id='{0}' value='{1}'>{2}</option>", item.Id, item.Value,
                                        item.Label);
                                }

                            }
                        }//value is string
                        else if (typeCode == TypeCode.String)
                        {
                            if (controlValue.ToString() == item.Label.ToString())
                            {
                                if (this.InputType == FormInputControl.Select)
                                {
                                    sb.AppendFormat(
                                        "<option id='{0}' selected='selected' value='{1}'>{2}</option>", item.Id,
                                        item.Value, item.Label);
                                }

                            }
                            else
                            {
                                if (this.InputType == FormInputControl.Select)
                                {
                                    sb.AppendFormat("<option id='{0}' value='{1}'>{2}</option>", item.Id, item.Value,
                                        item.Label);
                                }

                            }
                        }//value is int array
                        else if (typeCode == TypeCode.Int64)
                        {

                        }
                        else if (typeCode == TypeCode.Object)
                        {
                            var x = controlValue as int[];
                            if (x.Contains(item.Id))
                            {
                                if (this.InputType == FormInputControl.Select)
                                {
                                    sb.AppendFormat("<option id='{0}' selected='selected' value='{1}'>{2}</option>",
                                        item.Id,
                                        item.Value, item.Label);
                                }

                            }
                            else
                            {
                                if (this.InputType == FormInputControl.Select)
                                {
                                    sb.AppendFormat("<option id='{0}' value='{1}'>{2}</option>", item.Id, item.Value,
                                        item.Label);
                                }

                            }
                        }

                    }
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

        public IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl, object attributes = null)
        {
            IFormInput<T> inputControl = new FormInput<T, TValue>(Form, constraint, classes, formControl, attributes);
            Add(inputControl);
            return inputControl;
        }
        public IFormInput<T> Add<TValue>(string classes, Expression<Func<T, TValue>> constraint, FormInputControl formControl, IEnumerable<FormInputItem> datasource, object attributes = null)
        {
            IFormInput<T> inputControl = new FormInput<T, TValue>(Form, constraint, classes, formControl, datasource, attributes);
            Add(inputControl);
            return inputControl;
        }

        public IFormInput<T> Add<TValue>(Func<dynamic, HelperResult> template)
        {
            IFormInput<T> inputControl = new FormInput<T, TValue>(Form,template);
            Add(inputControl);
            return inputControl;
        }
    }

}
