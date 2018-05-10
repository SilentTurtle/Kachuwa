using System;

namespace Kachuwa.Web
{
    public class Pager
    {

        public Pager(int totalItems, int? page, int pageSize = 10,string api="/")
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            //Api = api;
        }

        public void Reset()
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)TotalItems / (decimal)PageSize);
            var currentPage = CurrentPage == 0 ? 1 : CurrentPage;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            // TotalItems = totalItems;
            CurrentPage = currentPage;
            // PageSize = pageSize;
            //TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
        public int TotalItems { get; private set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public string WrapperClass { get; set; } = "pagination";
        public string PrevClass { get; set; } = "prev";
        public string NextClass { get; set; } = "next";
        public string DisabledClass { get; set; } = "disabled";
        public bool ShowPrevNext { get; set; } = true;
        public string Api { get; set; } = "";
        public string FirstTemplate { get; set; } = "<span aria-hidden='true'>&larr;</span>";
        public string LastTemplate { get; set; } = "<span aria-hidden='true'>&larr;</span>";
        public string ActiveClass { get; set; } = "active";
        public string LastClass { get; set; } = "lastpage";
        public string FirstClass { get; set; } = "firstpage";
    }
}