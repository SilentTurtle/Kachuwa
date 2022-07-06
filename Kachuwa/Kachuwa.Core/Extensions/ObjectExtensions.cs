using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Kachuwa.Extensions
{
    public static class ObjectExtensions
    {
        public static T ToObject<T>(this IFormCollection postedForm)
            where T : new()
        {
            var instance = new T();

            PropertyInfo[] properties = instance.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!postedForm.Any(x => x.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase)))
                    continue;

                // KeyValuePair<string, object> item = postedForm.First(x => x.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));
                StringValues value = "";
                postedForm.TryGetValue(property.Name, out value);
                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = instance.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                string paramType = newT.FullName.ToLower();

                //checking and handling nullable type
                if (newT.IsArray && newT.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    paramType = newT.GetGenericArguments()[0].FullName.ToLower();

                    if (paramType == "system.datetime" || paramType == "system.single" || paramType == "system.boolean" || paramType == "system.string" || paramType == "system.int32" || paramType == "system.int64" || paramType == "system.decimal" || paramType == "system.double")
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            object newA = Convert.ChangeType(value.ToString(), Type.GetType(newT.GetGenericArguments()[0].FullName));
                            instance.GetType().GetProperty(property.Name).SetValue(instance, newA, null);
                        }
                    }
                }
                //ok for string decimal int not for class object type
                else if (paramType == "system.datetime" ||
                         paramType == "system.single" ||
                         paramType == "system.boolean" ||
                         paramType == "system.string" ||
                         paramType == "system.int32" ||
                         paramType == "system.int64" ||
                         paramType == "system.decimal" ||
                         paramType == "system.double")
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        object newA = Convert.ChangeType(value.ToString(), newT);
                        instance.GetType().GetProperty(property.Name).SetValue(instance, newA, null);
                    }
                }
                else
                {
                    ////if (newT.IsClass == true && newT.IsGenericType == true)
                    ////{
                    ////    Type objectParamType = Type.GetType(newT.FullName);

                    ////    if (objectParamType == null)
                    ////    {

                    ////        if (newT.Assembly.Location != null && newT.Assembly.Location != string.Empty)
                    ////        {
                    ////            Assembly assembly = Assembly.LoadFile(newT.Assembly.Location);
                    ////            objectParamType = assembly.GetType(newT.FullName);
                    ////        }
                    ////    }

                    ////    var obj = DictionaryToList((IList)item.Value, objectParamType);
                    ////    paramObj.GetType().GetProperty(property.Name).SetValue(paramObj, obj, null);

                    ////}
                    ////else 
                    //if (paramType == "system.array")
                    //{
                    //    //value contain arraysvalue
                    //    Array arrValues = DictionaryToArray((IList)item.Value);
                    //    paramObj.GetType().GetProperty(property.Name).SetValue(paramObj, arrValues, null);

                    //}
                    //else if (paramType == "system.collections.arraylist")
                    //{
                    //    //value contains array
                    //    var x = (IList)item.Value;

                    //    if (x.Count > 0)
                    //    {
                    //        ArrayList arrList = DictionaryToArrayList((IList)item.Value);
                    //        paramObj.GetType().GetProperty(property.Name).SetValue(paramObj, arrList, null);
                    //    }
                    //}
                    //else
                    //{

                    //    Type objectParamType = Type.GetType(newT.FullName);
                    //    if (objectParamType == null)
                    //    {

                    //        if (newT.Assembly.Location != null && newT.Assembly.Location != string.Empty)
                    //        {
                    //            Assembly assembly = Assembly.LoadFile(newT.Assembly.Location);
                    //            objectParamType = assembly.GetType(newT.FullName);
                    //        }
                    //    }
                    //    var obj = DictionaryToObj((Dictionary<string, object>)item.Value, objectParamType);
                    //    paramObj.GetType().GetProperty(property.Name).SetValue(paramObj, obj, null);
                    //}
                }
            }
            return instance;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
        public static Dictionary<string, string> ToDictionary(this IFormCollection source)
        {
            var dictionary=new Dictionary<string,string>();
            foreach (var key in source.Keys)
            {
                source.TryGetValue(key,out var val);
                dictionary.TryAdd(key, val.ToString());
            }

            return dictionary;

        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static T To<T>(this object obj) where T : new()
        {
            var instance = new T();

            PropertyInfo[] properties = instance.GetType().GetProperties();

            var objProps = obj.GetType().GetProperties();
            var isSubClass = obj.GetType().IsSubclassOf(instance.GetType());
            foreach (PropertyInfo property in properties)
            {
                if (!objProps.Any(x => x.Name.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase)))
                    continue;
                object value = null;
                if (isSubClass)
                {
                    value = property.GetValue(obj, null);
                }
                else
                {
                 
                    value = GetPropValue(obj, property.Name);
                }
              
                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = instance.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                string paramType = newT.FullName.ToLower();

                if (paramType.Equals("system.int32[]")) continue;

                //checking and handling nullable type
                if (newT.IsArray && newT.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    paramType = newT.GetGenericArguments()[0].FullName.ToLower();

                    if (paramType == "system.datetime" || paramType == "system.single" || paramType == "system.boolean" || paramType == "system.string" || paramType == "system.int32" || paramType == "system.int64" || paramType == "system.decimal" || paramType == "system.double")
                    {
                        if (value!=null)
                        {
                            object newA = Convert.ChangeType(value.ToString(), Type.GetType(newT.GetGenericArguments()[0].FullName));
                            instance.GetType().GetProperty(property.Name).SetValue(instance, newA, null);
                        }
                    }
                }
                //ok for string decimal int not for class object type
                else if (paramType == "system.datetime" ||
                         paramType == "system.single" ||
                         paramType == "system.boolean" ||
                         paramType == "system.string" ||
                         paramType == "system.int32" ||
                         paramType == "system.int64" ||
                         paramType == "system.decimal" ||
                         paramType == "system.double")
                {
                    if (value != null)
                    {
                        object newA = Convert.ChangeType(value.ToString(), newT);
                        instance.GetType().GetProperty(property.Name).SetValue(instance, newA, null);
                    }
                }
                else
                {
                }
            }
            return instance;
        }
    }
}