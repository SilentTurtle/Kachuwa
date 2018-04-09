using Kachuwa.Data;

namespace Kachuwa.HtmlContent.Service
{
    public interface IHtmlContentService
    {
        CrudService<Model.HtmlContent> HtmlService { get; set; }

    }
}