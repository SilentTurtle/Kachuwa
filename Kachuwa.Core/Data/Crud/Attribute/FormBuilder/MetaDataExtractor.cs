using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Reflection;
using Kachuwa.Data.Crud.Attribute;
using Newtonsoft.Json;
using KeyAttribute = Kachuwa.Data.Crud.Attribute.KeyAttribute;

namespace Kachuwa.Data.Crud.FormBuilder
{
    public static class MetaDataExtractor
    {

        public static string Extract<T>()
        {
            var instance = Activator.CreateInstance<T>();
            var metadata = new MetaData();
            foreach (var prop in instance.GetType().GetProperties())
            {
                if (prop.Name.ToLower() == "rowtotal")
                    continue;
                if (prop.GetCustomAttribute<IgnoreAllAttribute>() != null)
                    continue;

                IDictionary<string, object> dependentType = new ExpandoObject();
                IDictionary<string, object> inputvalidation = new ExpandoObject();
                IDictionary<string, object> inputMeta = metadata.FormMetaDatas;
                Console.WriteLine("{0}={1}", prop.Name, prop.PropertyType);
                var keyId = prop.GetCustomAttribute<KeyAttribute>();
                var customAtt = prop.GetCustomAttribute<InputAttribute>();
                var validation = prop.GetCustomAttribute<ValidateAttribute>();
                var dependent = prop.GetCustomAttribute<DependentAttribute>();
                bool hasDependent = dependent != null;
                //var sss= prop.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof (InputAttribute).Name);
                string formInputType = "text";
                if (customAtt != null)
                {
                    formInputType = customAtt.Input;
                }
                else
                {
                    // Gets what the data type is of our property (Foreign Key Property)
                    Type propertyType = prop.PropertyType;

                    // Get the type code so we can switch
                    System.TypeCode typeCode = System.Type.GetTypeCode(propertyType);
                    switch (typeCode)
                    {
                        case TypeCode.Int32:
                            formInputType = "number";
                            break;
                        case TypeCode.Int64:
                            formInputType = "number";
                            break;
                        case TypeCode.String:
                            formInputType = "text";
                            break;
                        case TypeCode.Object:
                            break;
                        case TypeCode.Boolean:
                            formInputType = "checkbox";
                            break;
                        case TypeCode.Double:
                            formInputType = "money";
                            break;
                        case TypeCode.Decimal:
                            formInputType = "money";
                            break;
                        default:
                            formInputType = "text";
                            break;
                    }
                }
                if (validation != null)
                {
                    foreach (var val in validation.Validations)
                    {
                        inputvalidation.Add(val, true);
                    }
                }
                bool isPrimary = keyId != null;
                inputMeta.Add(prop.Name, new { input = formInputType, isdependent = hasDependent, primary = isPrimary, rules = inputvalidation });
            }
            string jsonmetadata = JsonConvert.SerializeObject(metadata.FormMetaDatas);
            return jsonmetadata;
        }

        public static string Extract(object model)
        {
            var instance = model;
            var metadata = new MetaData();
            foreach (var prop in instance.GetType().GetProperties())
            {
                if (prop.Name.ToLower() == "rowtotal")
                    continue;
                if (prop.GetCustomAttribute<IgnoreAllAttribute>() != null)
                    continue;

                IDictionary<string, object> dependentType = new ExpandoObject();
                IDictionary<string, object> inputvalidation = new ExpandoObject();
                IDictionary<string, object> inputMeta = metadata.FormMetaDatas;
                Console.WriteLine("{0}={1}", prop.Name, prop.PropertyType);
                var keyId = prop.GetCustomAttribute<KeyAttribute>();
                var customAtt = prop.GetCustomAttribute<InputAttribute>();
                //dotnet implemented validation attributes
                var validations = prop.GetCustomAttributes<ValidationAttribute>();
                var dependent = prop.GetCustomAttribute<DependentAttribute>();
                bool hasDependent = dependent != null;
                //var sss= prop.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof (InputAttribute).Name);
                string formInputType = "text";
                if (customAtt != null)
                {
                    formInputType = customAtt.Input;
                }
                else
                {
                    // Gets what the data type is of our property (Foreign Key Property)
                    Type propertyType = prop.PropertyType;

                    // Get the type code so we can switch
                    System.TypeCode typeCode = System.Type.GetTypeCode(propertyType);
                    switch (typeCode)
                    {
                        case TypeCode.Int32:
                            formInputType = "number";
                            break;
                        case TypeCode.Int64:
                            formInputType = "number";
                            break;
                        case TypeCode.String:
                            formInputType = "text";
                            break;
                        case TypeCode.Object:
                            break;
                        case TypeCode.Boolean:
                            formInputType = "checkbox";
                            break;
                        case TypeCode.Double:
                            formInputType = "money";
                            break;
                        case TypeCode.Decimal:
                            formInputType = "money";
                            break;
                        default:
                            formInputType = "text";
                            break;
                    }
                }
                if (validations != null)
                {
                    foreach (var val in validations)
                    {
                        inputvalidation.Add(val.ToString(), true);
                    }
                }
                bool isPrimary = keyId != null;
                inputMeta.Add(prop.Name, new { input = formInputType, isdependent = hasDependent, primary = isPrimary, rules = inputvalidation });
            }
            string jsonmetadata = JsonConvert.SerializeObject(metadata.FormMetaDatas);
            return jsonmetadata;
        }
    }
}