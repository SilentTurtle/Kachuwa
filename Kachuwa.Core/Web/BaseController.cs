using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kachuwa.Web
{
    [LogError]
    public class BaseController:Controller
    {
        public readonly ModelService ModelService = new ModelService();


        public RedirectResult RedirectToAnother(string url)
        {
            return base.Redirect(url);
        }

        
       
       // protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
            //if (!HttpContext.User.Identity.IsAuthenticated)
            //{
            //    if (HttpContext.Request.Cookies.Get(ApplicationClaim.Anonymous) == null)
            //    {
            //        var userId = Guid.NewGuid().ToString();
            //        HttpCookie cookie = new HttpCookie(ApplicationClaim.Anonymous);
            //        cookie.HttpOnly = true;
            //        cookie.Expires = DateTime.Now.AddMinutes(30);
            //        cookie.Value = userId;
            //        HttpContext.Response.Cookies.Add(cookie);
            //    }
            //}
            //base.OnActionExecuting(filterContext);
       // }
    }
    public class ModelService
    {
        public async Task<T> GetAsync<T>(object id)
        {
            return await new CrudService<T>().GetAsync(id);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(object whereConditions)
        {
            return await new CrudService<T>().GetListAsync(whereConditions);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string conditions,
            object parameters)
        {
            return await new CrudService<T>().GetListAsync(conditions, parameters);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>()
        {
            return await new CrudService<T>().GetListAsync();
        }

        public async Task<IEnumerable<T>> GetListPagedAsync<T>(int pageNumber, int rowsPerPage,
            int pageSize, string conditions, string orderby, object parameters = null)
        {
            return await new CrudService<T>().GetListPagedAsync(pageNumber, rowsPerPage, pageSize, conditions, orderby, parameters);

        }

        public async Task<int?> InsertAsync<T>(object entityToInsert)
        {
            return await new CrudService<T>().InsertAsync<int?>(entityToInsert);

        }

        //public  async Task<TKey> InsertAsync<TKey>(object entityToInsert)
        //{
        //    return await new CrudService<>().InsertAsync<TKey>(entityToInsert);
        //}
        public async Task<bool> UpdateAsync<T>(object entityToUpdate)
        {
            return await new CrudService<T>().UpdateAsync(entityToUpdate);

        }
        public async Task<int> DeleteAsync<T>(object entityToDelete)
        {
            return await new CrudService<T>().DeleteAsync(entityToDelete);

        }
        public async Task<int> DeleteByIdAsync<T>(object id)
        {
            return await new CrudService<T>().DeleteAsync(id);

        }
        // int DeleteList(object whereConditions);
        public async Task<int> DeleteListAsync<T>(object whereConditions)
        {
            return await new CrudService<T>().DeleteListAsync(whereConditions);

        }
        public async Task<int> DeleteListAsync<T>(string conditions,
            object parameters = null)
        {
            return await new CrudService<T>().DeleteListAsync(conditions, parameters);
        }

        public async Task<int> RecordCountAsync<T>(string conditions = "",
            object parameters = null)
        {

            return await new CrudService<T>().RecordCountAsync(conditions, parameters);


        }
        public async Task<int> RecordCountAsync<T>(object whereConditions)
        {
            return await new CrudService<T>().RecordCountAsync(whereConditions);

        }


    }
}
