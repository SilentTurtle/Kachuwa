using System;
using Microsoft.AspNetCore.Html;

namespace Kachuwa.KGrid
{
    public interface IHtmlGrid<T> : IHtmlContent
    {
        IKachuwaGrid<T> Grid { get; }
        String PartialViewName { get; set; }
        IHtmlGrid<T> Pagination(Action<KachuwaPager> builder);
        IHtmlGrid<T> Build(Action<IKachuwaGridColumnsOf<T>> builder);
        //IHtmlGrid<T> ProcessWith(IGridProcessor<T> processor);

        //IHtmlGrid<T> Filterable(Boolean isFilterable);
        //IHtmlGrid<T> MultiFilterable();
        //IHtmlGrid<T> Filterable();

        //IHtmlGrid<T> Sortable(Boolean isSortable);
        //IHtmlGrid<T> Sortable();

        IHtmlGrid<T> RowCss(Func<T, String> cssClasses);
        IHtmlGrid<T> Css(String cssClasses);
        IHtmlGrid<T> Empty(String text);
        IHtmlGrid<T> Named(String name);

        //IHtmlGrid<T> Pageable(Action<IGridPager<T>> builder);
        // IHtmlGrid<T> Pageable();
        IHtmlGrid<T> AddCommands(Action<IKachuwaGridCommandsOf<T>> action);
    }
}