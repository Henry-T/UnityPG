using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace UnityWorkerManager
{
    public static class SharpSVNAgent
    {
        public static bool CheckIsSvnDir(string path)
        {
            SvnClient client = new SvnClient();
            return false;
        }

        public static bool SvnUpdate(string path)
        {
            SvnClient client = new SvnClient();
            return client.Update(path);            
        }

        public static bool SvnCleanup(string path)
        {
            SvnClient client = new SvnClient();
            return client.CleanUp(path);
        }

        public static bool SvnCommit(string path)
        {
            SvnClient client = new SvnClient();
            return client.Commit(path);
        }
    }
}
