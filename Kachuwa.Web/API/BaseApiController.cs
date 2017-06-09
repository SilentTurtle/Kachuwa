using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kachuwa.Web.API
{
    [LogError]
    public class BaseApiController : Controller
    {
        public ApiResponse HttpResponse(int statusCode, string msg, object data)
        {

            return new ApiResponse
            {
                Code = statusCode,
                Message = msg,
                Data = data
            };
        }

        public ApiResponse HttpResponse(int statusCode, string msg)
        {
            return new ApiResponse
            {
                Code = statusCode,
                Message = msg,
            };
        }

        public ApiResponse ErrorResponse(int statusCode, string msg)
        {

            return new ApiResponse
            {
                Code = statusCode,
                Message = msg,
            };
        }
        public ApiResponse ErrorResponse(string msg)
        {
            return new ApiResponse
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = msg
            };
        }
        public async Task<ApiResponse> Crud<T>(string searchCondition = null) where T : new()
        {
            try
            {
                var routeData = Request.HttpContext.GetRouteData();
                string task = routeData.Values["task"].ToString();
                var crudService = new CrudService<T>();

                var postedForm = Request.Form;
                if (postedForm.Keys != null)
                    if (!postedForm.Keys.Any())
                    {
                        throw new Exception("Invalid form post");
                    }

                //  var jsonObj = JObject.Parse(param.ToString());
                switch (task)
                {
                    case "get":
                        int id = int.Parse(postedForm["id"].ToString());
                        var brand = await crudService.GetAsync(id);
                        return HttpResponse((int)HttpStatusCode.OK, "", brand);

                    case "list":

                        int offset = int.Parse(postedForm["offset"].ToString());
                        int limit = int.Parse(postedForm["limit"].ToString());

                        string search = postedForm["search"].ToString();
                        string filter = postedForm["filter"].ToString();
                        string searchcondition = searchCondition + search + "%' ";
                        var data = await crudService.GetListPagedAsync(offset, limit, 1, searchcondition, "");
                        // int rowTotal = await crudService.RecordCountAsync();
                        return HttpResponse((int)HttpStatusCode.OK, "", data);

                    case "save":

                        var model = postedForm.ToObject<T>();
                        //var model = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(param.ToString(), new JsonSerializerSettings()
                        //{
                        //    DateParseHandling = DateParseHandling.None,
                        //    NullValueHandling = NullValueHandling.Ignore//for autofill

                        //});
                        model.AutoFill();
                        var prop = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() == 1);
                        object ret = prop != null ? prop.GetValue(model, null) : null;
                        if (ret != null)
                        {
                            var primaryId = int.Parse(ret.ToString());
                            if (primaryId == 0)
                            {
                                int insertId = await crudService.InsertAsync<int>(model);
                                return HttpResponse((int)HttpStatusCode.OK, "", insertId);
                            }
                            else
                            {
                                await crudService.UpdateAsync(model);
                                return HttpResponse((int)HttpStatusCode.OK, "", "");
                            }
                        }
                        else
                        {
                            return HttpResponse((int)HttpStatusCode.BadRequest, "", "Primary Id Missing");
                        }

                    case "count":
                        string condition = postedForm["condition"].ToString();
                        int count = await crudService.RecordCountAsync(condition);
                        return HttpResponse((int)HttpStatusCode.OK, "", count);

                    case "delete":
                        string dids = postedForm["id"].ToString();
                        string[] sids = dids.Split(',');
                        int[] ids = sids.Select(int.Parse).ToArray(); //Array.ConvertAll(sids, int.Parse);
                        await crudService.DeleteAsync(ids);
                        break;
                    case "dependencies":

                        var dependencies = await crudService.GetDependents();
                        return HttpResponse((int)HttpStatusCode.OK, "", dependencies);

                }

                return HttpResponse((int)HttpStatusCode.OK, "", true);

            }
            catch (Exception ex)
            {
                return ErrorResponse((int)HttpStatusCode.BadRequest, ex.Message.ToString());
            }
        }

    }
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
    }
}