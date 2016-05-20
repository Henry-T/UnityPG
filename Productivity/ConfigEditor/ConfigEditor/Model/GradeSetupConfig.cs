using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class GradeSetupConfig : XlsData
    {
        public List<NatureRequirePreset> NatureRequirePresets;

        public GradeSetupConfig(String name, String keyName):base(name, keyName){}

        public override void Load()
        {
            base.Load();

            NatureRequirePresets = new List<NatureRequirePreset>();
            DataView view = DataTable.AsDataView();

            // TODO thy ID排序不应造成变化，但row次序不同 why?
            // view.Sort = "ID";
            foreach(DataRowView rowView in view)
            {
                string category = rowView["Category"].ToString();
                int grade = int.Parse(rowView["Grade"].ToString());
                int index = grade -1;

                NatureList natureList = new NatureList();
                for(int i=1; i<=8; i++)
                {
                    ENature nature;
                    if (!Enum.TryParse<ENature>(rowView["Nature" + i].ToString(), out nature))
                        break;
                    natureList[i-1] = new NatureEnumWrap(nature);
                }

                NatureRequirePreset preset = GetNatureRequire(category);
                if(preset == null)
                {
                    preset = new NatureRequirePreset();
                    preset.Category = category;
                    NatureRequirePresets.Add(preset);
                }

                preset.Requirement.Add(natureList);
            }
        }

        public NatureRequirePreset GetNatureRequire(string category)
        {
            foreach (NatureRequirePreset require in NatureRequirePresets)
            {
                if (require.Category == category)
                    return require;
            }
            return null;
        }
    }

    public class NatureRequirePreset
    {
        public NatureRequirePreset()
        {
            Requirement = new NatureRequirement();
        }

        public string Category { get; set; }
        public NatureRequirement Requirement;
    }
}
