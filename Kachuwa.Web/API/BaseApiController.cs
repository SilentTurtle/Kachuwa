using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Extensions;
using Kachuwa.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kachuwa.Web.API
{
    [LogError]
    //[Authorize]
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
   
}