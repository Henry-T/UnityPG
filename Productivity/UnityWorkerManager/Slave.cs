using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace UnityWorkerManager
{
    // 负责一条原子命令的执行
    public class Slave
    {
        public string Name;             // 辨识名称
        public string ProjectPath;      // Unity工作目录
        public string DefaultCmd;       // 默认命令

        public Thread Thread;           // 独立监视线程
        public Process UnityProcess;    // Unity后台进程

        public bool IsSvnDir = false;

        // Worker接到命令到执行完毕，IsRunning为true
        public bool IsRunning
        {
            get
            {
                return (Thread != null && Thread.ThreadState != System.Threading.ThreadState.Stopped);
            }
        }
        
        public bool Initialize()
        {
            if (!Directory.Exists(ProjectPath))
                return false;

            // NOTE 此方法检查SVN太缓慢 暂时禁用掉
            // IsSvnDir = SvnAgent.CheckIsSvnDir(ProjectPath);
            // Program.Console.WriteLine("SVN: " + (IsSvnDir?"[是]":"[否]"));
            // Program.Console.WriteLine(ProjectPath);

            IsSvnDir = true;
            return true;
        }
        
        public void StartActionThread( string unityCmd)
        {
            Thread = new Thread(() => action(unityCmd));
            Thread.Start();
        }

        private void action(string unityCmd)
        {
            Program.Console.WriteLine("Unity工作目录：");
            Program.Console.WriteLine(ProjectPath);

            if (IsSvnDir)
            {
                SvnAgent.SvnUpdate(ProjectPath);
            }

            Process slaveProcess = new Process();
            slaveProcess.StartInfo.FileName = ManagerForm.UnityPath;
            slaveProcess.StartInfo.Arguments = unityCmd;
            slaveProcess.StartInfo.CreateNoWindow = true;
            slaveProcess.StartInfo.UseShellExecute = false;
            slaveProcess.StartInfo.RedirectStandardOutput = true;
            slaveProcess.StartInfo.RedirectStandardError = true;
            slaveProcess.EnableRaisingEvents = false;

            string stdOutput = "";
            string errOutput = "";
            slaveProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                stdOutput += e.Data + "/n";
            };
            slaveProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                errOutput += e.Data + "/n";
            };

            UnityProcess = slaveProcess;


            try
            {
                if (slaveProcess.Start())
                {
                    slaveProcess.BeginOutputReadLine();
                    slaveProcess.BeginErrorReadLine();

                    while (!slaveProcess.HasExited)
                    {
                        Program.Console.WriteLine("奴隶" + Name + " -- 正在劳作 " + (DateTime.Now - slaveProcess.StartTime).TotalSeconds.ToString("%d") + "秒");
                        slaveProcess.WaitForExit(1000);
                        slaveProcess.Refresh();
                    }

                    if (slaveProcess.HasExited)
                    {
                        int exitCode = slaveProcess.ExitCode;

                        if (exitCode == 0)
                        {
                            Program.Console.WriteLine("奴隶" + Name + ":Unity命令已经搞定了！");
                        }
                        else
                        {
                            Program.Console.WriteLine("奴隶" + Name + ":Unity工作中出了差错！ (暂无差错详情，你自己去调教它吧- -|||)");
                            Program.Console.WriteLine(stdOutput);
                            Program.Console.WriteLine(errOutput);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                slaveProcess.Close();
                Program.Console.WriteLine("Unity进程存在问题：" + e.Message);
            }

            // SVN 提交
            // TODO snv add 自动记录更改 并保留ignore清单的设置
            if (IsSvnDir)
            {
                SvnAgent.SvnCommit(ProjectPath);
            }

            Program.Console.WriteLine("奴隶" + Name + ":已经完成了任务！");
        }
    }
}
