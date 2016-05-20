using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class NatureConfig : XlsData
    {
        public List<NatureEntry> NatureEntries;

        public NatureConfig(String name, String keyName):base(name, keyName)
        {
        }

        public NatureEntry this[ENature natureType] 
        {
            get 
            { 
                foreach (NatureEntry entry in NatureEntries)
                {
                    if (entry.ID == (int)natureType)
                        return entry;
                }
                return NatureEntry.NullNatureEntry;
            }
        }

        public override void Load()
        {
            base.Load();

            NatureEntries = new List<NatureEntry>();

            DataView view = DataTable.DefaultView;

            foreach(DataRowView rowView in view)
            {
                NatureEntry newEntry = new NatureEntry();
                newEntry.ID = int.Parse(rowView["ID"].ToString());
                newEntry.Name = rowView["Name"] as String;
                string addsStr = rowView["Additions"] as String;

                JSONArray ary = JSON.Parse(addsStr).AsArray;
                newEntry.Additions = new AdditionList();

                foreach(JSONNode node in ary)
                {
                    Addition add = new Addition();
                    add.Type = (EAddition)node["Type"].AsInt;
                    add.Value = node["Value"].AsInt;
                    newEntry.Additions.Add(add);
                }

                NatureEntries.Add(newEntry);
            }
        }
    }

    public class NatureEntry
    {
        public int ID;
        public string Name;

        public AdditionList Additions;

        public static NatureEntry NullNatureEntry
        {
            get 
            {
                if (nullNatureEntry == null)
                {
                    nullNatureEntry = new NatureEntry();
                    nullNatureEntry.ID = 0;
                    nullNatureEntry.Name = "无";
                    nullNatureEntry.Additions = new AdditionList();
                }
                return nullNatureEntry;
            }
        }
        private static NatureEntry nullNatureEntry;
    }


    public class AdditionList : ObservableCollection<Addition>
    {

        // Key Name
        public Addition this[EAddition type]
        {
            get{
                foreach(Addition add in this)
                {
                    if (add.Type == type)
                        return add;
                }
                return Addition.NullAddition;
            }
        }

        // Key ID
        //public Addition this[int index]
        //{
        //    get{
        //        if (index < this.Count)
        //        {
        //            return base[index];
        //        }
        //        return Addition.NullAddition;
        //    }
        //}
    }

    public class Addition
    {
        public EAddition Type;
        public int Value;

        public static Addition NullAddition
        {
            get
            {
                if (nullAddition == null)
                {
                    nullAddition = new Addition();
                    nullAddition.Type = EAddition.无;
                    nullAddition.Value = 0;
                }
                return nullAddition;
            }
        }
        private static Addition nullAddition;
    }
}
