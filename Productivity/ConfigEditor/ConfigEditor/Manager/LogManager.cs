using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using LogglyManager = log4net.LogManager;
using LogManger = ConfigEditor.LogManager;
using System.Windows;

namespace ConfigEditor
{
    public class LogManager
    {
        public static LogManager instance;

        public static LogManager Instance { 
            get {
                if (instance == null)
                    instance = new LogManager();
                instance.Initialize();
                return instance;
            }
        }

        private LogManager() { }

        private TextWriter logWriter;
        private StringBuilder logBuilder;
        private ILog logger;

        private bool initialized = false;
        public void Initialize()
        {
            if (!initialized)
                initialized = true;
            else
                return;

            if (File.Exists("Log.txt"))
                File.Delete("Log.txt");

            logWriter = File.CreateText("Log.txt");
            logBuilder = new StringBuilder();
            logger = LogglyManager.GetLogger("ConfigEditor");
        }

        public void Cleanup()
        {
            logWriter.Close();
            logger.Info(logBuilder.ToString());
        }

        public void Log(string logStr)
        {
            String str = "[LOG] " + logStr;
            Console.WriteLine(str);
            logWriter.WriteLine(str);
            logBuilder.AppendLine(str);
        }

        public void Warn(string warnStr)
        {
            String str = "[WARN] " + warnStr;
            Console.WriteLine(str);
            logWriter.WriteLine(str);
            logBuilder.AppendLine(str);
        }

        public void Error(string errStr)
        {
            String str = "[ERROR] " + errStr;
            Console.WriteLine(str);
            logWriter.WriteLine(str);
            logBuilder.AppendLine(str);
        }

        public void Error(Exception e, string customMsg)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(customMsg);
            builder.AppendLine(e.ToString());
            builder.AppendLine(e.Message);
            Console.WriteLine(builder.ToString());

            builder.AppendLine(e.StackTrace);
            logWriter.WriteLine(builder.ToString());
        }

        public void ShowErrorMessageBox(string errStr)
        {
            Error(errStr);
            MessageBox.Show(errStr, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
