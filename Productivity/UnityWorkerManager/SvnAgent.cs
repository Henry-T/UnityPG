using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace UnityWorkerManager
{
    // TODO 用SVNSharp实现
    public static class SvnAgent
    {
        public static bool CheckIsSvnDir(string path)
        {
            string stdOutput = "";
            string errOutput = "";

            Process svnCheckProcess = new Process();
            svnCheckProcess.StartInfo.FileName = "svn";
            svnCheckProcess.StartInfo.Arguments = "status " + path;
            svnCheckProcess.StartInfo.CreateNoWindow = true;
            svnCheckProcess.StartInfo.UseShellExecute = false;
            svnCheckProcess.StartInfo.RedirectStandardOutput = true;
            svnCheckProcess.StartInfo.RedirectStandardError = true;
            svnCheckProcess.EnableRaisingEvents = false;

            svnCheckProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e)=>{
                stdOutput += e.Data;
            };
            svnCheckProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e)=>{
                errOutput += e.Data;
            };

            svnCheckProcess.Start();
            svnCheckProcess.BeginOutputReadLine();
            svnCheckProcess.BeginErrorReadLine();
            svnCheckProcess.WaitForExit();      // NOTE 即使进程结束很快，所有的输出也都可获取到

            if (svnCheckProcess.ExitCode == 0)
            {
                // 通过输出特征字符串进行检查
                if (errOutput.Contains("is not a working copy"))
                    return false;
                return
                    true;
            }
            else
            {
                Program.Console.WriteLine("svn status 未能成功执行 错误信息：");
                Program.Console.WriteLine(errOutput);

                return false;
            }
        }

        public static bool SvnUpdate(string path)
        {
            string stdOutput = "";
            string errOutput = "";

            Process svnCheckProcess = new Process();
            svnCheckProcess.StartInfo.FileName = "svn";
            svnCheckProcess.StartInfo.Arguments = "update " + path;
            svnCheckProcess.StartInfo.CreateNoWindow = true;
            svnCheckProcess.StartInfo.UseShellExecute = false;
            svnCheckProcess.StartInfo.RedirectStandardOutput = true;
            svnCheckProcess.StartInfo.RedirectStandardError = true;
            svnCheckProcess.EnableRaisingEvents = false;

            svnCheckProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                stdOutput += e.Data + "/n";
            };
            svnCheckProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                errOutput += e.Data + "/n";
            };

            svnCheckProcess.Start();
            svnCheckProcess.BeginOutputReadLine();
            svnCheckProcess.BeginErrorReadLine();
            svnCheckProcess.WaitForExit();      // NOTE 即使进程结束很快，所有的输出也都可获取到

            // NOTE 尽量不要人为操作这些自动目录，易出冲突
            if (svnCheckProcess.ExitCode == 0)
            {
                // 通过输出特征字符串进行检查
                if (errOutput.Contains("Conflict discovered"))
                {
                    return false;
                }
                else
                {
                    Program.Console.WriteLine("SVN Update 成功");
                    return true;
                }
            }
            else
            {
                if (errOutput.Contains("svn cleanup"))
                {
                    Program.Console.WriteLine("此SVN已锁定，将在Cleanup后重试...");
                    SvnCleanup(path);
                    return SvnUpdate(path);    // 再次尝试
                }
                else
                {
                    Program.Console.WriteLine("svn update 未能成功执行 错误信息：");
                    Program.Console.WriteLine(errOutput);
                    return false;
                }
            }
        }


        public static bool SvnCleanup(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = "svn";
            process.StartInfo.Arguments = "cleanup " + path;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            Program.Console.WriteLine("SVN Cleanup 完成");
            return true;
        }

        public static bool SvnCommit(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = "svn";
            process.StartInfo.Arguments = "commit " + path + " -m UnitySlave_AutoCommit";
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            // NOTE 尚不能处理"冲突"的情况 
            // 在团队公用此工具打包时此情况极易出现
            Program.Console.WriteLine("SVN Commit 完成");
            return true;

            // TODO 冲突处理
            if (process.ExitCode != 0)
            {
                Program.Console.WriteLine("SVN Commit 失败 ");
                return false;
            }
            else
            {
                Program.Console.WriteLine("SVN Commit 成功 ");
                return true;
            }
        } 
    }
}
