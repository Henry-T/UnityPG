using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpSvn;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace ConfigEditorLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SvnClient SvnClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbInfo.Content = "一切正常";

            String curDir = Directory.GetCurrentDirectory();
            String curExe = System.Reflection.Assembly.GetExecutingAssembly().Location;


            SvnClient = new SvnClient();

            SvnTarget svnTargetCurDir = SvnTarget.FromString(curDir);
            SvnTarget snvTargetCurExe = SvnTarget.FromString(curExe);

            // 未使用 - 获取目录下的文件状态
            Collection<SvnStatusEventArgs> svnStatusEventArgsCollection;
            SvnClient.GetStatus(Directory.GetCurrentDirectory(), out svnStatusEventArgsCollection);

            // 未使用 - 空的
            Collection<SvnPropertyListEventArgs> svnPropertyListEventArgsCollection;
            SvnClient.GetPropertyList(Directory.GetCurrentDirectory(), out svnPropertyListEventArgsCollection);

            // 未使用 - 获取一个文件的状态
            Collection<SvnFileVersionEventArgs> svnFileVersionEventArgsCollection;
            try
            {
                SvnClient.GetFileVersions(snvTargetCurExe, out svnFileVersionEventArgsCollection);
            }
            catch(SvnWorkingCopyPathNotFoundException ex)
            {
                // 如果查询的文件未加入Repo，会抛此异常
            }

            // 未使用 - 一个好像没什么用的信息列表，目录是第一个项，没有版本号
            Collection<SvnListEventArgs> svnListEventArgsCollection;
            SvnClient.GetList(svnTargetCurDir, out svnListEventArgsCollection);

            // 未使用 - 工作目录全路径
            String workingCopyRoot = SvnClient.GetWorkingCopyRoot(Directory.GetCurrentDirectory());
            
            // 未使用 - 整个仓库的最新版本
            long revision = 0;
            SvnClient.Youngest(Directory.GetCurrentDirectory(), out revision);

            // 此目录的相关变更 和 GUI中的ShowLog一样 是此目录的相关变更           
            Collection<SvnLogEventArgs> logList;
            try
            {
                SvnClient.GetLog(Directory.GetCurrentDirectory(), out logList);
            }
            catch(SvnInvalidNodeKindException ex)
            {
                lbInfo.Content = "当前目录不是SVN目录，停用更新检查功能";
                btnUpdateAndLaunch.Visibility = Visibility.Hidden;
                return;
            }

            // 获取本地目录信息，当前版本和修改版本都是本地的
            SvnInfoEventArgs svnInfoEventArgs;
            SvnClient.GetInfo(svnTargetCurDir, out svnInfoEventArgs);

            long curRevision = svnInfoEventArgs.Revision;       // 当前的SVN版本
            long changeRevision = logList[0].Revision;          // 当前目录的最新远程变更版本

            lbVersionChange.Content = changeRevision;
            lbVersionCur.Content = svnInfoEventArgs.Revision;

            if (curRevision < changeRevision)
            {
                btnUpdateAndLaunch.Visibility = Visibility.Visible;
                lbInfo.Content = "发现新版本";
            }
            else
            {
                btnUpdateAndLaunch.Visibility = Visibility.Hidden;
                lbInfo.Content = "已经是最新版";
            }

            // --------------------------------------------------------
            // SvnWorkingCopyClient
            // --------------------------------------------------------
            SvnWorkingCopyClient svnWorkingCopyClient = new SvnWorkingCopyClient();

            // 未使用 只有一个值IsText 没看出有什么用处
            SvnWorkingCopyState svnWorkingCopyState;
            svnWorkingCopyClient.GetState(Directory.GetCurrentDirectory(), out svnWorkingCopyState);

            // 未使用 返回仅本地存在的所有修改版本 
            SvnWorkingCopyVersion svnWorkingCopyVersion;
            svnWorkingCopyClient.GetVersion(curDir, out svnWorkingCopyVersion);

            // --------------------------------------------------------
            // SvnTools
            // --------------------------------------------------------
            // 未使用 传入正确的目录却返回false????
            bool isCurDirInWorkingCopy = SvnTools.IsManagedPath(curDir);

        }

        private void btnUpdateAndLaunch_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("update.bat");
            Process.Start(startInfo);
            Application.Current.Shutdown(0);
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("ConfigEditor");
            Process.Start(startInfo);
            Application.Current.Shutdown(0);
        }

    }
}
