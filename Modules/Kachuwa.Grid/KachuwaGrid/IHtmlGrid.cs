using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.KGrid
{
    public interface IHtmlGrid<T> : IHtmlContent
    {
        IKachuwaGrid<T> Grid { get; }
        string PartialViewName { get; set; }
        IHtmlGrid<T> Pagination(Action<KachuwaPager> builder);
        IHtmlGrid<T> Build(Action<IKachuwaGridColumnsOf<T>> builder);
        //IHtmlGrid<T> ProcessWith(IGridProcessor<T> processor);

        //IHtmlGrid<T> Filterable(Boolean isFilterable);
        //IHtmlGrid<T> MultiFilterable();
        //IHtmlGrid<T> Filterable();

        //IHtmlGrid<T> Sortable(Boolean isSortable);
        //IHtmlGrid<T> Sortable();

        IHtmlGrid<T> RowCss(Func<T, string> cssClasses);
        IHtmlGrid<T> Css(string cssClasses);
        IHtmlGrid<T> Empty(string text);
        IHtmlGrid<T> Empty(Func<dynamic, HelperResult> template);
        IHtmlGrid<T> Named(string name);

        //IHtmlGrid<T> Pageable(Action<IGridPager<T>> builder);
        // IHtmlGrid<T> Pageable();
        IHtmlGrid<T> AddCommands(Action<IKachuwaGridCommandsOf<T>> action);
    }
}