using System;
using System.Collections.Generic;
using System.Text;
using Kachuwa.Data;

namespace Kachuwa.HtmlContent.Service
{
    public class HtmlContentService: IHtmlContentService
    {
        public CrudService<Model.HtmlContent> HtmlService { get; set; }=new  CrudService<Model.HtmlContent>();

    }
}
