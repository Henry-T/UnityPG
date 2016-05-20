using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Spine;

namespace ConfigEditor
{
    public class SpineUtil
    {
        // ProjectS 中，一个Spine内可以有多个共享部件的角色，它们以动画名称前缀区分
        // json 解析是从 Spine-Runtime 里拿过来的
        public static List<String> GetWarriorNames(String spineName)
        {
            String resDir = UserConfigManager.Instance.Config.ResDir;
            string zipPath = System.IO.Path.Combine(resDir, "spine/" + spineName + ".zip");
            
            FileInfo zipFInfo = new FileInfo(zipPath);
            if (!zipFInfo.Exists)
                return new List<String>();

            ZipFile zf = new ZipFile(zipPath);
            ZipEntry jsonEntry = zf.GetEntry(spineName + ".json");

            Stream jsonStream = zf.GetInputStream(jsonEntry);

            var root = new Dictionary<String, Object>();
            using (TextReader reader = new StreamReader(jsonStream))
            {
                root = Json.Deserialize(reader) as Dictionary<String, Object>;
            }

            if (root == null)
                throw new Exception(String.Format("Spine json 文件错误: {0} {1}", spineName, zipPath));

            List<String> result = new List<string>();

            if (root.ContainsKey("animations"))
            {
                foreach (KeyValuePair<String, Object> entry in (Dictionary<String, Object>)root["animations"])
                {
                    int dashPos = entry.Key.IndexOf('_');
                    if (dashPos != -1)
                    {
                        string actorName = entry.Key.Substring(0, dashPos);
                        if (!result.Contains(actorName))
                        {
                            result.Add(actorName);
                        }
                    }
                }
            }

            return result;
        }
    }
}
