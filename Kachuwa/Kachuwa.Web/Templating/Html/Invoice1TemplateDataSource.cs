//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Kachuwa.Web.Templating
//{
//    public class Invoice1TemplateDataSource : ITemplateDataSource
//    {
//        public TemplateTypes Type { get; } = TemplateTypes.Invoice;
//        public string Key { get; } = "Test Invoice";
//        public string Barcode { get; set; }
//        public EventInfo EventInfo { get; set; }
//        public string Note { get; set; }
//        public Task RenderSource()
//        {
//            this.Barcode = "Imadol";
//            this.Note = "1989 zzz 2";
//            this.EventInfo = new EventInfo() {EventName = "Event 1"};
//            return Task.FromResult(true);
//        }
//        public Task RenderSource(Dictionary<string, object> parameters)
//        {
//            this.Barcode = "Imadol";
//            this.Note = "1989 zzz 2";
//            this.EventInfo = new EventInfo() { EventName = "Event 1" };
//            return Task.FromResult(true);
//        }
//    }
//}