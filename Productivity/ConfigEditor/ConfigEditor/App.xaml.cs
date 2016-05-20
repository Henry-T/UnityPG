using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            String userConfigPath = "UserConfig.json";
            String commonConfigPath = "CommonConfig.json";

            UserConfigManager.Instance.Load(userConfigPath);
            CommonConfigManager.Instance.Load(commonConfigPath);

            // 启动时检查路径配置
            bool needSetPath = 
                UserConfigManager.Instance.Config.ExcelDir == null ||
                !Directory.Exists(UserConfigManager.Instance.Config.ExcelDir) ||
                UserConfigManager.Instance.Config.ResDir == null ||
                !Directory.Exists(UserConfigManager.Instance.Config.ResDir);

            if (needSetPath)
            {
                Application.Current.MainWindow = new EditorWindow();
            }
            else
            {
                Application.Current.MainWindow = new SkillEditWindow();
            }

            Application.Current.MainWindow.Show();

            // Unhandled Exception 测试
            // throw new Exception("Yep!");
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // NOTE thy 此处无法吞噬异常，程序退出无法取消
            // http://stackoverflow.com/questions/11393604/can-we-swallow-unhandled-exception-in-windows-service
            Exception exception = e.ExceptionObject as Exception;

            LogManager.Instance.Error("--- Exception Start");
            LogManager.Instance.Error(exception.ToString());
            LogManager.Instance.Error(exception.Message);
            LogManager.Instance.Error(exception.StackTrace);
            LogManager.Instance.Error("--- Exception End");

            // 崩溃前写入并提交Log
            LogManager.Instance.Cleanup();

            MessageBox.Show("程序出现错误，请通知维护人员");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            RenderAppManager.Instance.Stop();
            LogManager.Instance.Cleanup();
        }
    }
}
