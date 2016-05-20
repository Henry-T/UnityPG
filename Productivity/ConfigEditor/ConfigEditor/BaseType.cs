using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace ConfigEditor
{
    [ImplementPropertyChanged]
    public class BaseType
    {
        public int ID {get; set;}

        public BaseType()
        {

        }

        public BaseType ShallowCopy()
        {
            return (BaseType)this.MemberwiseClone();
        }
    }
}
