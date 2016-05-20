using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace UnityWorkerManager
{
    class Program
    {
        public static ManagerForm Console;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console = new ManagerForm();
            Application.Run(Console);
        }
    }
}
