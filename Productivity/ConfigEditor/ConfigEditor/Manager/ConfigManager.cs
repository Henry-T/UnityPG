using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;

namespace ConfigEditor
{
    public class ConfigManager<T> where T : BaseConfig, new()
    {
        private static ConfigManager<T> instance;
        public static ConfigManager<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigManager<T>();
                }
                return instance;
            }
        }

        public T Config;
        public string ConfigPath;

        public Object this[string name]
        {
            get {
                return Config[name];
            }
            set {
                Config[name] = value;
            }
        }

        public void Load(string configPath)
        {
            ConfigPath = configPath;

            if (!File.Exists(ConfigPath))
            {
                LogManager.Instance.Warn("找不到配置文件，但会在保存时创建一个新的: " + ConfigPath);
                Config = new T();
            }
            else
            {
                string jsonStr = File.ReadAllText(ConfigPath);
                try
                {
                    Config = JsonConvert.DeserializeObject<T>(jsonStr);
                }
                catch (Exception e)
                {
                    LogManager.Instance.Error(e, "配置文件中有错误，已创建一个空配置，如果你保存此配置，旧的配置将被替换: " + ConfigPath);
                    Config = new T();
                }
            }
        }

        public void Save()
        {
            String jsonStr = JsonConvert.SerializeObject(Config, Newtonsoft.Json.Formatting.Indented);
            try
            {
                File.WriteAllText(ConfigPath, jsonStr);
            }
            catch(Exception e)
            {
                string restorePath = ConfigPath + ".restore";
                LogManager.Instance.Error("保存配置时出错，请确认配置文件未被其他程序占用。当前配置已存入: " + restorePath);
                File.WriteAllText(restorePath, jsonStr);
            }
        }
    }
}
