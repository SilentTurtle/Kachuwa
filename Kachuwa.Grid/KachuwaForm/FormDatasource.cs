using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kachuwa.Form
{
    public class FormDatasource
    {
        private ConcurrentDictionary<string,IEnumerable<object>> _sources
        {
            get;
            set;
        }=new ConcurrentDictionary<string, IEnumerable<object>>();
        public IEnumerable<object> GetSource(string key)
        {
            if(_sources.ContainsKey(key))
                return _sources[key];
            else
            {
                return null;
            }
        }

        public bool SetSource(string key, IEnumerable<object> objects)
        {
            if (!_sources.ContainsKey(key))
            {
                _sources.GetOrAdd(key, objects);
                return true;
            }
            return false;
        }
        

    }
}