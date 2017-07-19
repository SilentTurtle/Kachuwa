using System;
using Kachuwa.Web;

namespace Kachuwa.KGrid
{
    public class KachuwaPager: Pager
    {
        public KachuwaPager(int totalItems, int? page, int pageSize = 10) : base(totalItems, page, pageSize)
        {
        }
    }
}