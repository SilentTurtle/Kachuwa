using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web.Layout
{
    public class FileBaseLayoutProvider : ILayoutContentProvider
    {//store in json format for each file


        public Task<ILayoutContent> Get(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Save(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Reset(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ILayoutContent>> GetAll()
        {
            throw new System.NotImplementedException();
        }
    }
}