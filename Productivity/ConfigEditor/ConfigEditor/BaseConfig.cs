using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class BaseConfig
    {

        public Object this[string name]
        {
            get 
            {
                PropertyInfo pInfo = this.GetType().GetProperty(name);
                if (pInfo != null)
                    return pInfo.GetValue(this);
                else
                    return null;
            }
            set
            {
                PropertyInfo pInfo = this.GetType().GetProperty(name);
                if (pInfo != null)
                    pInfo.SetValue(this, value);
            }
        }
    }
}
