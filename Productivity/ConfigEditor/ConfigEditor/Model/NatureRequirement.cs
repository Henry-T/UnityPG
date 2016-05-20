using ConfigType;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class NatureRequirement : ObservableCollection<NatureList>
    {
        public static NatureRequirement CreateFromJson(string jsonStr)
        {
            NatureRequirement require = new NatureRequirement();
            JSONArray jsonAry = JSONNode.Parse(jsonStr).AsArray;
            foreach (JSONArray ary in jsonAry.Childs)
            {
                NatureList grade = new NatureList();

                for (int i = 0; i < ary.Childs.Count(); i++)
                {
                    grade[i].Value = (ENature)ary.Childs.ElementAt(i).AsInt;
                }

                require.Add(grade);
            }
            return require;
        }

        public string ConvertToJson()
        {
            JSONArray jsonAry = new JSONArray();

            foreach (NatureList natureList in this)
            {
                JSONArray jNatureList = new JSONArray();

                bool isRowValid = false;
                foreach (NatureEnumWrap wrap in natureList)
                {
                    if (wrap.Value != ENature.无)
                    {
                        isRowValid = true;
                        jNatureList[-1].AsInt = (int)wrap.Value;
                    }
                }

                if (isRowValid)
                    jsonAry[-1] = jNatureList;
            }

            string str = jsonAry.ToString();
            str = str.Replace("\"", "");

            return str;
        }
    }

    public class NatureList : ObservableCollection<NatureEnumWrap>
    {
        public NatureList()
        {
            for (int i = 0; i < 8; i++)
            {
                base.Add(new NatureEnumWrap(ENature.无));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

        }
    }

    public class NatureEnumWrap : INotifyPropertyChanged
    {
        public ENature Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        private ENature value;

        public NatureEnumWrap(ENature v)
        {
            Value = v;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

        }
    }
}
