//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Kachuwa.Web.Layout
//{
//    public class LayoutManager
//    {
//        public ILayoutContentProvider Provider { get; set; }


//        public LayoutManager(ILayoutContentProvider provider)
//        {
//            Provider = provider;
//        }

//        public async Task<bool> Save(string name)
//        {
//            return await Provider.Save(name);

//        }

//        public Task<List<ILayoutContent>> GetAll()
//        {
//            throw new System.NotImplementedException();
//        }

//        public async Task<bool> Reset(string name)
//        {
//            return true;
//        }

//        public async Task<ILayoutContent> Get(string name)
//        {
//            throw new System.NotImplementedException();
//        }

//        public async Task<bool> Delete(string name)
//        {
//            return true;
//        }
//    }
//}