using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace WebApp.Test.PugViewEngine
{
    public class Class
    {
    }
    /// <summary>
    /// Defines the contract for a view engine.
    /// </summary>
    public interface IViewEngine
    {
        /// <summary>
        /// Finds the view with the given <paramref name="viewName"/> using view locations and information from the
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The <see cref="ActionContext"/>.</param>
        /// <param name="viewName">The name of the view.</param>
        /// <param name="isMainPage">Determines if the page being found is the main page for an action.</param>
        /// <returns>The <see cref="ViewEngineResult"/> of locating the view.</returns>
        ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage);

        /// <summary>
        /// Gets the view with the given <paramref name="viewPath"/>, relative to <paramref name="executingFilePath"/>
        /// unless <paramref name="viewPath"/> is already absolute.
        /// </summary>
        /// <param name="executingFilePath">The absolute path to the currently-executing view, if any.</param>
        /// <param name="viewPath">The path to the view.</param>
        /// <param name="isMainPage">Determines if the page being found is the main page for an action.</param>
        /// <returns>The <see cref="ViewEngineResult"/> of locating the view.</returns>
        ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage);
    }
    //public class PugzorView : IView
    //{
    //    private string _path;
    //    private INodeServices _nodeServices;

    //    public PugzorView(string path, INodeServices nodeServices)
    //    {
    //        _path = path;
    //        _nodeServices = nodeServices;
    //    }

    //    public string Path
    //    {
    //        get
    //        {
    //            return _path;
    //        }
    //    }

    //    public async Task RenderAsync(ViewContext context)
    //    {
    //        var result = await _nodeServices.InvokeAsync<string>("./pugcompile", Path, context.ViewData.Model);
    //        context.Writer.Write(result);
    //    }
    //}
}
