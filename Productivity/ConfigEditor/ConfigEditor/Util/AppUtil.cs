using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConfigEditor
{
    public class AppUtil
    {
        public static bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }
    }
}
