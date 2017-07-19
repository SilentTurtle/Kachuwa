namespace Kachuwa.Web
{
    public interface ITemplateEngine
    {
        string Render(string template,object model);
        string Render(string template, object model, bool isHtml);
        string RenderFromFile(string filePath, object model,bool isHtml);
    }
}