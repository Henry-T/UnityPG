using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnityWorkerManager
{
    public partial class ManagerForm : Form
    {
        public delegate void StringHandler(object sender, string txt);

        public Waiter Waiter;
        public List<Slave> Slaves = new List<Slave>();
        public List<String> PostBuildCmds = new List<String>();

        // 其他配置
        public static string UnityPath = "unity.exe";
        private int listenPort = 8090;

        public bool BatchWorking = false;

        public bool RunPostCmdAfterBuild = true;

        public ManagerForm()
        {
            InitializeComponent();
        }

        // 大部分输出信息都显示在这个文本框中
        public void WriteLine(String txt)
        {
            tbOutput.BeginInvoke(new StringHandler(Output), this, txt + '\n');
        }

        private void Output(object sender, String txt)
        {
            tbOutput.AppendText(txt);
        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            // 读配置
            FileInfo exeInfo = new FileInfo(Application.ExecutablePath);
            Directory.SetCurrentDirectory(exeInfo.Directory.FullName);
            string configFile = Path.Combine(exeInfo.Directory.FullName, "config.xml");
            if (File.Exists(configFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFile);

                XmlNode docNode = xmlDoc.DocumentElement;

                // Unity程序路径
                XmlElement unityEle = docNode.SelectSingleNode("Unity") as XmlElement;
                UnityPath = unityEle.Attributes["ExePath"].Value;

                // 端口
                XmlElement socketEle = docNode.SelectSingleNode("Socket") as XmlElement;
                listenPort = int.Parse(socketEle.Attributes["Port"].Value);

                // 工人
                XmlElement workersEle = docNode.SelectSingleNode("workers") as XmlElement;
                XmlNodeList workerNodes = workersEle.SelectNodes("worker");
                foreach (XmlElement xmlEle in workerNodes)
                {
                    String name = xmlEle.Attributes["Name"].Value;
                    String projPath = xmlEle.Attributes["ProjectPath"].Value;
                    String defCmd = xmlEle.Attributes["DefaultCmd"].Value;
                    Slave slave = new Slave { Name = name, ProjectPath = projPath, DefaultCmd = defCmd };
                    slave.Initialize();
                    Slaves.Add(slave);
                }

                // 后处理
                XmlElement postBuildCmdsEle = docNode.SelectSingleNode("PostBuildCmds") as XmlElement;
                foreach (XmlElement xmlEle in postBuildCmdsEle)
                {
                    string cmd = xmlEle.Attributes["Cmd"].Value;
                    PostBuildCmds.Add(cmd);
                }


                WriteLine("");
                WriteLine(">>>>>>>> 初始化完毕");

                // 命令参考

            }

            // 允许客户端接入
            Waiter = new Waiter(8090);
        }

        private void btnCmd_Click(object sender, EventArgs e)
        {
            DoOrder(tbInput.Text);
        }

        public void DoOrder(string cmd)
        {
            WriteLine("开始分析指令:");
            WriteLine(cmd);
            
            // 缩减连续空格
            Regex regex = new Regex(" +");
            string fixCmd = regex.Replace(cmd, " ");

            string[] args = fixCmd.Split(' ');

            // 预定义参数
            string arg_slave = "";
            string arg_command = "";
            string arg_platform = "";
            string arg_packages = "";

            // 读取参数对
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-slave":
                        arg_slave = args[i + 1];
                        i++;
                        break;
                    case "-command":
                        arg_command = args[i + 1];
                        i++;
                        break;
                    case "-platform":
                        arg_platform = args[i + 1];
                        i++;
                        break;
                    case "-packages":
                        arg_packages = args[i + 1];
                        i++;
                        break;
                }
            }

            // 组合Unity命令
            StringBuilder unityCMDBuilder = new StringBuilder();
            bool commandOK = true;
            string commandErrInfo = "";

            // 默认基础参数
            unityCMDBuilder.Append(" -quit  -batchmode -nographics ");

            // 设定Slave
            Slave curSlave = Slaves.Find(s => s.Name == arg_slave);
            if (curSlave != null)
            {
                unityCMDBuilder.Append(" -projectPath " + curSlave.ProjectPath + " ");


                if (curSlave.IsRunning)
                {
                    WriteLine("奴隶" + curSlave.Name + "非常繁忙，请过会儿再鞭挞他 ");
                    return;
                }

                // ========
                // 发布命令
                // ========
                if (arg_command == "build")
                {
                    if (arg_platform == "android")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformBuildAndroid ");
                        // TODO 检查此项
                        WriteLine("APK Identifier必须先设置好，此错误不会有任何提示");
                    }
                    else if (arg_platform == "ios")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformBuildIOS ");
                    }
                    else if (arg_platform == "web")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformBuildWeb ");
                    }
                    else if (arg_platform == "all")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformBuildAll ");
                    }
                    else
                    {
                        string info = "奴隶" + arg_slave + ": 我不认识这个目标平台" + arg_platform;
                        commandErrInfo += info + "\n";
                        WriteLine(info);
                        commandOK = false;
                    }
                }
                // ========
                // 打包命令
                // ========
                else if (arg_command == "package")
                {
                    if (arg_platform == "android")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformPackageAndroid ");
                    }
                    else if (arg_platform == "ios")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformPackageIOS ");
                    }
                    else if (arg_platform == "web")
                    {
                        unityCMDBuilder.Append(" -executeMethod BackgroundWorker.PerformPackageWeb ");
                    }
                }
                else
                {
                    WriteLine("奴隶" + arg_slave + "：我只会打包(package)和发布(build)");
                    commandOK = false;
                }

            }
            else
            {
                string info = "指定的奴隶不存在! " + arg_slave;
                commandErrInfo += info + "\n";
                Console.WriteLine(info);
                commandOK = false;
            }

            // 执行
            if (commandOK)
            {
                WriteLine("");
                WriteLine(">>>>>>> 已成功向奴隶下达指令");
                WriteLine("UnityCmd: " + unityCMDBuilder.ToString());
                curSlave.StartActionThread(unityCMDBuilder.ToString());
            }
            else
            {
                WriteLine("命令存在问题，无法开始");
                WriteLine(commandErrInfo);
            }
        }
        
        private void menuBuildAndroid_Click(object sender, EventArgs e)
        {
            tbInput.Text = "-slave build_android -command build -platform android";
        }

        private void menuBuildIOS_Click(object sender, EventArgs e)
        {
            tbInput.Text = "-slave build_ios -command build -platform ios";
        }

        private void menuBuildWeb_Click(object sender, EventArgs e)
        {
            tbInput.Text = "-slave build_web -command build -platform web";
        }

        private void menuPackageAndroid_Click(object sender, EventArgs e)
        {
            tbInput.Text = "-slave package_android -command package -platform android -packages Box";
        }

        private void menuPackageIOS_Click(object sender, EventArgs e)
        {
            tbInput.Text = "-slave package_ios -command package -platform ios -packages Box";
        }

        private void menuPackageWeb_Click(object sender, EventArgs e)
        {
            tbInput.Text = "-slave package_web -command package -platform web -packages Box";
        }

        private void btnLinkAll_Click(object sender, EventArgs e)
        {
            foreach(Slave slave in Slaves)
            {
                mklink(Path.Combine(slave.ProjectPath, "Assets/Editor"));
            }
        }

        private void btnLinkClear_Click(object sender, EventArgs e)
        {
            btnLinkAll_Click(sender, e);
        }

        private void btnLinkOverrite_Click(object sender, EventArgs e)
        {
            btnLinkAll_Click(sender, e);
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        static extern bool CreateSymbolicLink(string SymbolicFileName, string TargetFileName, UInt32 Flags);

        const UInt32 SymbolicLinkFlagFile = 0;
        const UInt32 SymbolicLinkFlagDirectory = 1;

        private void mklink(string tgtFolder)
        {
            // NOTE git不支持link，因此改用文件拷贝，也就修改的时候会稍微麻烦点
            FileInfo fInfo = new FileInfo(Path.Combine(tgtFolder, "BackgroundWorker.cs"));
            if (!fInfo.Directory.Exists)
                Directory.CreateDirectory(fInfo.Directory.FullName);

            if (fInfo.Exists)
                fInfo.Delete();

            FileInfo srcFile = new FileInfo("BackgroundWorker.cs");

            File.Copy(srcFile.FullName, fInfo.FullName, true);

            //CreateSymbolicLink(fInfo.FullName, srcFile.FullName, SymbolicLinkFlagFile);

           // FileInfo dFile = new FileInfo("Xyz.cs");
           // CreateSymbolicLink(dFile.FullName, srcFile.FullName, SymbolicLinkFlagFile);
        }

        private void btnBuildAll_Click(object sender, EventArgs e)
        {
            BatchWorking = true;
            tmrCheckWorkDone.Start();
            foreach (Slave slave in Slaves)
            {
                DoOrder(" -slave " + slave.Name +" -command build -platform all");
            }
        }

        private void btnCommonCmd_Click(object sender, EventArgs e)
        {
            string command = tbInput.Text;

            // 执行一条其他命令
            // unity -quit -batchmode -nographics -projectPath E:\proj\UnityPG\3D\Movement_CB9 -executeMethod BackgroundWorker.PerformBuildWeb
            int pos = command.IndexOf(' ');

            string stdOutput = "";
            string errOutput = "";

            Process agentProcess = new Process();
            agentProcess.StartInfo.FileName = command.Substring(0, pos);
            agentProcess.StartInfo.Arguments = command.Substring(pos, command.Length - pos);
            agentProcess.StartInfo.CreateNoWindow = true;
            agentProcess.StartInfo.UseShellExecute = false;
            agentProcess.StartInfo.RedirectStandardOutput = true;
            agentProcess.StartInfo.RedirectStandardError = true;
            agentProcess.EnableRaisingEvents = false;

            agentProcess.OutputDataReceived += (object s, DataReceivedEventArgs ea) =>
            {
                stdOutput += ea.Data;
            };

            agentProcess.ErrorDataReceived += (object s, DataReceivedEventArgs ea) =>
            {
                errOutput += ea.Data;
            };

            agentProcess.Start();
            agentProcess.BeginOutputReadLine();
            agentProcess.BeginErrorReadLine();
            agentProcess.WaitForExit();

            WriteLine(stdOutput);
            WriteLine(errOutput);

        }

        private void btnDoPostBuildCmd_Click(object sender, EventArgs e)
        {
            RunPostBuildCmds();
        }

        public void RunPostBuildCmds()
        {
            foreach (string postBuildCmd in PostBuildCmds)
            {
                Process process = new Process();
                FileInfo fInfo = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), postBuildCmd));
                process.StartInfo.FileName = fInfo.FullName;
                process.StartInfo.WorkingDirectory = fInfo.Directory.FullName;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
        }

        private void tmrCheckWorkDone_Tick(object sender, EventArgs e)
        {
            if (BatchWorking)
            {
                bool done = true;
                foreach(Slave s in Slaves)
                {
                    if (s.IsRunning)
                    {
                        done = false;
                        break;
                    }
                }
                if (done)
                {
                    tmrCheckWorkDone.Stop();
                    if (RunPostCmdAfterBuild)
                        RunPostBuildCmds();
                }
            }
            else
            {
                tmrCheckWorkDone.Stop();
            }
        }

        private void cbRunPostCmdAfterBuild_Click(object sender, EventArgs e)
        {
            cbRunPostCmdAfterBuild.Checked = !cbRunPostCmdAfterBuild.Checked;
            RunPostCmdAfterBuild = cbRunPostCmdAfterBuild.Checked;
        }

        private void btnBuildSWB_Click(object sender, EventArgs e)
        {
            BatchWorking = true;
            tmrCheckWorkDone.Start();
            DoOrder(" -slave " + "SWBuilder" + " -command build -platform all");
        }

        private void btnBuildTianshen_Click(object sender, EventArgs e)
        {
            BatchWorking = true;
            tmrCheckWorkDone.Start();
            DoOrder(" -slave " + "Tianshen" + " -command build -platform all");
        }
    }
}
