using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;

public class ExcelAccessAgent 
{
    // 启动代理读取Cell项
    public static string ReadAttachInfo(string code)
    {
        string stdOutput = "";
        string errOutput = "";

        Process agentProcess = new Process();
        agentProcess.StartInfo.FileName = "ExcelAccessAgent";       // ExcelAccessAgent.exe在同一目录下
        agentProcess.StartInfo.Arguments = Path.Combine(ToolManager.Instance.virtualWorkDir, ToolManager.Instance.HeroExcel) + " read " + code;
        agentProcess.StartInfo.CreateNoWindow = true;
        agentProcess.StartInfo.UseShellExecute = false;
        agentProcess.StartInfo.RedirectStandardOutput = true;
        agentProcess.StartInfo.RedirectStandardError = true;
        agentProcess.EnableRaisingEvents = false;

        agentProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            stdOutput += e.Data;
        };

        agentProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            errOutput += e.Data;
        };

        agentProcess.Start();
        agentProcess.BeginOutputReadLine();
        agentProcess.BeginErrorReadLine();
        agentProcess.WaitForExit();

        if (agentProcess.ExitCode != 0)
        {
            // 用户错误指示
            if (errOutput.Contains("找不到Code为"))
            {
                ToolManager.Instance.InfoStr = "Excel中没有采用此Spine的行!";
            }
            else if(errOutput.Contains("由另一进程使用"))
            {
                ToolManager.Instance.InfoStr = "Excel被别的程序打开,请先关闭";
            }

            Debug.LogError("ExcelAccessAgent遇到错误: " + errOutput);
            // System.Windows.Forms.MessageBox.Show(errOutput, "错误", System.Windows.Forms.MessageBoxButtons.OK);
            return "";
        }
        return stdOutput;
    }

    // 启动代理写入数据项
    public static bool WriteAttachInfo(string code, string value)
    {
        string stdOutput = "";
        string errOutput = "";

        Process agentProcess = new Process();
        agentProcess.StartInfo.FileName = "ExcelAccessAgent";
        agentProcess.StartInfo.Arguments =  Path.Combine(ToolManager.Instance.virtualWorkDir, ToolManager.Instance.HeroExcel) + " write " + code + " " + value;
        agentProcess.StartInfo.CreateNoWindow = true;
        agentProcess.StartInfo.UseShellExecute = false;
        agentProcess.StartInfo.RedirectStandardOutput = true;
        agentProcess.StartInfo.RedirectStandardError = true;
        agentProcess.EnableRaisingEvents = false;

        agentProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            stdOutput += e.Data;
        };

        agentProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            errOutput += e.Data;
        };

        agentProcess.Start();
        agentProcess.BeginOutputReadLine();
        agentProcess.BeginErrorReadLine();
        agentProcess.WaitForExit();

        if (agentProcess.ExitCode != 0)
        {
            // 用户错误指示
            if (errOutput.Contains("System.IO.IOException"))
            {
                ToolManager.Instance.InfoStr = "Excel被别的程序打开,请先关闭";
            }

            Debug.LogError("ExcelAccessAgent遇到错误: " + errOutput);

            return false;
        }
        return true;
    }
}
