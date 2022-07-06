using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Kachuwa.Web.Form;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Mustache;

namespace Kachuwa.Web.Grid
{
    public interface IKachuwaGridColumn
    {
        string Name { get; set; }
        string Format { get; set; }
        string CssClasses { get; set; }
        string FormClasses { get; set; }
        bool IsEncoded { get; set; }
        FormInputControl FormControl { get; set; }
        IHtmlContent Title { get; set; }
        IHtmlContent ValueFor(IKachuwaGridRow<Object> row);
        IHtmlContent RenderAttributes(ViewContext viewContext);
        IDictionary<string, object> Attributes { get; set; }
        IEnumerable<FormInputItem> DataSource { get; set; }
        IHtmlContent RenderControlSource(object dataSource, IKachuwaGridRow<Object> row);
        string SelectedValue { get; set; }

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
        IKachuwaGridColumn<T> Add<TValue>(Expression<Func<T, TValue>> constraint, string selectedValue = "");

        IKachuwaGridColumn<T> Insert(Int32 index);
        IKachuwaGridColumn<T> Insert<TValue>(Int32 index, Expression<Func<T, TValue>> constraint);
    }


    public abstract class BaseKachuwaGridColumn<T, TValue> : IKachuwaGridColumn<T>
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public string CssClasses { get; set; }
        public string FormClasses { get; set; } = "form-control";
        public bool IsEncoded { get; set; }
        public IHtmlContent Title { get; set; }
        public IKachuwaGrid<T> Grid { get; set; }
        public FormInputControl FormControl { get; set; } = FormInputControl.Label;
        public Func<T, Object> RenderValue { get; set; }
        public Func<T, TValue> ExpressionValue { get; set; }
        LambdaExpression IKachuwaGridColumn<T>.Expression => Expression;
        public Expression<Func<T, TValue>> Expression { get; set; }

        public abstract IQueryable<T> Process(IQueryable<T> items);
        public abstract IHtmlContent ValueFor(IKachuwaGridRow<Object> row);
        public abstract IHtmlContent RenderAttributes(ViewContext viewContext);
        public abstract IHtmlContent RenderControlSource(object dataSource, IKachuwaGridRow<Object> row);
        public IDictionary<string, object> Attributes { get; set; }
        public IEnumerable<FormInputItem> DataSource { get; set; }
        public string SelectedValue { get; set; } = "";
    }


    public class KachuwaGridColumn<T, TValue> : BaseKachuwaGridColumn<T, TValue> where T : class
    {
        // private readonly Object _obj;
        private IModelMetadataProvider _metadataProvider;
        private ClientValidatorCache _clientValidatorCache;
        private IClientModelValidatorProvider _clientModelValidatorProvider;
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
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _metadataProvider = (IModelMetadataProvider)ContextResolver.Context.RequestServices.GetService(typeof(IModelMetadataProvider));
            _clientValidatorCache = new ClientValidatorCache();
            IOptions<MvcViewOptions> options = (IOptions<MvcViewOptions>)ContextResolver.Context.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
            var clientValidatorProviders = options.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);
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
        public override IHtmlContent RenderAttributes(ViewContext viewContext)
        {
            var expresionProvider = ContextResolver.Context.RequestServices
                .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;

            var modelExpression = expresionProvider.CreateModelExpression(new ViewDataDictionary<T>(_metadataProvider, new ModelStateDictionary()), Expression);
            var validationAttributes = new AttributeDictionary() { };
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
        public void AddValidationAttributes(
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

        public override IHtmlContent RenderControlSource(object dataSource, IKachuwaGridRow<Object> row)
        {
            StringBuilder sb = new StringBuilder();

            if (this.DataSource == null)
                return null;
            else
            {
                var controlValue = GetValueFor(row);

                if (this.FormControl == FormInputControl.Select)
                    sb.AppendFormat("<option value='{0}'>{1}</option>", "0", "Select...");

                if (controlValue == null)
                {
                    if (this.FormControl == FormInputControl.Select)
                    {

                        foreach (var item in this.DataSource)
                        {
                            if (item.IsSelected)
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
                    else if (this.FormControl == FormInputControl.CheckBoxList)
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
                    //assigning value checking as expression value matched
                    foreach (var item in this.DataSource)
                    {

                        if (this.FormControl == FormInputControl.Select)
                        {
                            if (controlValue.ToString() == item.Value.ToString())
                            {
                                sb.AppendFormat("<option id='{0}' selected='selected' value='{1}'>{2}</option>",
                                    item.Id,
                                    item.Value, item.Label);
                            }
                            else
                            {
                                sb.AppendFormat("<option id='{0}' value='{1}'>{2}</option>", item.Id, item.Value,
                                    item.Label);
                            }
                        }
                        else if (this.FormControl == FormInputControl.CheckBoxList)
                        {

                            if (controlValue.ToString() == item.Value.ToString())
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
            }



            Object value = sb.ToString();
            if (value == null) return HtmlString.Empty;
            //if (value is IHtmlContent)
            //    return value as IHtmlContent;
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
        public virtual IKachuwaGridColumn<T> Add<TValue>(Expression<Func<T, TValue>> expression, string selectedValue)
        {
            IKachuwaGridColumn<T> column = new KachuwaGridColumn<T, TValue>(Grid, expression);
            column.SelectedValue = selectedValue;
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
