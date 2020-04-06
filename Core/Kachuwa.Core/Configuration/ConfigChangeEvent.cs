using System.Collections.Generic;

namespace Kachuwa.Configuration
{
    public abstract class ConfigChangeEvent
    {
        readonly List<IConfigChangeListner> _changeListners = new List<IConfigChangeListner>();

        // Constructor
        protected ConfigChangeEvent()
        {
           
        }

        public void Attach(IConfigChangeListner listner)
        {
            _changeListners.Add(listner);
        }

        public void Detach(IConfigChangeListner listner)
        {
            _changeListners.Remove(listner);
        }

        public void Notify()
        {
            foreach (IConfigChangeListner investor in _changeListners)
            {
                investor.Update();
            }
        }

      
    }
}