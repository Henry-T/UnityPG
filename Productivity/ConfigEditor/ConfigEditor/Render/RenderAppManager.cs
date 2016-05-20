using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConfigType;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace ConfigEditor
{
    public class RenderAppManager
    {
        private static RenderAppManager instance;
        public static RenderAppManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RenderAppManager();
                }
                return instance;
            }
        }

        private NamedPipeServerStream pipeServer;
        private byte[] inBuffer;

        private Process renderAppProcess;

        private bool initialized = false;
        public void Initialize()
        {
            if (initialized)
                return;
            else
                initialized = true;

            // 初始化RenderControl
            KillExistingProcess();

            // 启动Pipe
            // NOTE 必须设置 Asynchronous
            pipeServer = new NamedPipeServerStream(
                "testpipe",
                PipeDirection.InOut,
                1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            inBuffer = new byte[1024];

            LogManager.Instance.Log("开始异步等待管道连接");
            pipeServer.BeginWaitForConnection(ar =>
            {
                pipeServer.EndWaitForConnection(ar);
                LogManager.Instance.Log("管道已连接");
                pipeServer.BeginRead(inBuffer, 0, pipeServer.InBufferSize, onClientDataReceived, pipeServer);
            }, pipeServer);

            // ProcessStartInfo info = new ProcessStartInfo(ConfigManager.Instance.Get("RenderAppPath"));
            ProcessStartInfo info = new ProcessStartInfo(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "./SpineRenderer/SpineRenderer.exe"));
            info.UseShellExecute = true;
            info.Arguments = " -popupwindow";
            
            // NOTE Hidden 会造成没有窗口句柄 unityProcess.MainWindowHandle == IntPtr.Zero
            // info.WindowStyle = ProcessWindowStyle.Minimized;
            // info.WindowStyle = ProcessWindowStyle.Normal;
            // info.WindowStyle = ProcessWindowStyle.Hidden;
            renderAppProcess = System.Diagnostics.Process.Start(info);

            // Wait for process to be created and enter idle condition
            LogManager.Instance.Log("开始等待渲染程序空闲");

            // NOTE thy 方法A在Win7上不管用，方法B在Win7/Win8上都OK
            // WaitForInputIdle 并不保证窗口句柄准备好

            // 方法-A
            // renderAppProcess.WaitForInputIdle(5000);

            // 方法-B
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (renderAppProcess.MainWindowHandle == IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(100);
                // stopWatch.ElapsedMilliseconds > 
            }

            if (renderAppProcess.HasExited)
            {
                LogManager.Instance.Warn("内嵌程序已经退出");
            }
            else
            {
                LogManager.Instance.Log("内嵌程序初始化完成");
                LogManager.Instance.Log("检查内嵌程序句柄是否存在: " + renderAppProcess.MainWindowHandle.ToString());
            }
        }

        public void Stop()
        {
            if (pipeServer != null && pipeServer.IsConnected)
            {
                pipeServer.Disconnect();
                pipeServer.Dispose();
            }

            if (renderAppProcess != null && !renderAppProcess.HasExited)
            {
                renderAppProcess.Kill();
                renderAppProcess = null;
            }
        }

        public void DoEmbed(Window window, FrameworkElement control)
        {
            //if (renderAppProcess == null || renderAppProcess.MainWindowHandle == IntPtr.Zero || control == null)
            //    return;

            LogManager.Instance.Log("执行嵌入操作");

            WindowInteropHelper helper = new WindowInteropHelper(window);
            IntPtr ptr = helper.Handle;

            IntPtr renderWindowHandle = renderAppProcess.MainWindowHandle;

            if (renderWindowHandle == IntPtr.Zero)
            {
                renderWindowHandle = FindWindow(null, "SpineRenderer");

                int id = -1;
                GetWindowThreadProcessId(renderWindowHandle.ToInt64(), out id);
                if (id != renderAppProcess.Id)
                {
                    LogManager.Instance.Error("Windows 中存在一个以上SpineRenderer窗口，请确保它是唯一的");
                    LogManager.Instance.Error("发现句柄进程ID: " + id);
                    LogManager.Instance.Error("    原始进程ID: " + renderAppProcess.Id);

                    return;
                }
                else
                {
                    LogManager.Instance.Error("成功获取RenderApp窗口句柄");
                }
            }

            HandleRef windowHandleRef = new HandleRef(this, renderWindowHandle);

            // SetWindowLong(windowHandleRef, GWL_STYLE, WS_VISIBLE);
            // SetWindowLong(windowHandleRef, GWL_STYLE, WS_OVERLAPPED);
            // SetWindowLong(windowHandleRef, GWL_STYLE, WS_CAPTION);

            //long lStyle = GetWindowLong(windowHandleRef, GWL_STYLE);
            //lStyle &= ~(WS_SYSMENU | WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE)));
            //SetWindowLong(windowHandleRef, GWL_STYLE, (int)lStyle);

            //long lExStyle = GetWindowLong(windowHandleRef, GWL_EXSTYLE);
            //lExStyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE);
            //SetWindowLong(windowHandleRef, GWL_EXSTYLE, (int)lExStyle);

            // SetWindowPos(renderAppProcess.MainWindowHandle, 0, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER);

            // MoveWindow(renderAppProcess.MainWindowHandle, (int)control.Margin.Left, (int)control.Margin.Top, (int)control.Width, (int)control.Height, true);
            // ShowWindow(renderAppProcess.MainWindowHandle, SW_SHOWNORMAL);

            // NOTE SetParent之后窗口样式 还原
            LogManager.Instance.Log("设定父窗口前 RenderApp Handle: " + renderAppProcess.MainWindowHandle.ToString());
            SetParent(renderAppProcess.MainWindowHandle, helper.Handle);
            MoveWindow(renderAppProcess.MainWindowHandle, (int)control.Margin.Left, (int)control.Margin.Top, (int)control.Width, (int)control.Height, true);

            LogManager.Instance.Log("嵌入完成");
        }

        public void Detach()
        {
            LogManager.Instance.Log("解嵌前 RenderApp Handle: " + renderAppProcess.MainWindowHandle.ToString());
            SetParent(renderAppProcess.MainWindowHandle, IntPtr.Zero);
            MoveWindow(renderAppProcess.MainWindowHandle, 0, 0, 1, 1, true);

            LogManager.Instance.Log("解嵌完成");
        }

        public void KillExistingProcess()
        {
            // NOTE 结束已经存在的SpineRenderer.exe
            ProcessStartInfo killProcessInfo = new ProcessStartInfo();
            killProcessInfo.FileName = "taskkill";
            killProcessInfo.Arguments = "/f /im SpineRenderer.exe";
            Process killProcess = Process.Start(killProcessInfo);
            killProcess.WaitForExit();
        }

        public void SendToClient(string str)
        {
            byte[] outBuffer = Encoding.UTF8.GetBytes(str);
            int len = outBuffer.Length;
            pipeServer.WriteByte((byte)(len / 256));
            pipeServer.WriteByte((byte)(len % 256));
            pipeServer.Write(outBuffer, 0, len);
            pipeServer.Flush();
        }

        private void onClientDataReceived(IAsyncResult asyncResult)
        {
            pipeServer.EndRead(asyncResult);
            if (inBuffer.Length == 0)
                return;

            EProtocol protocol = (EProtocol)inBuffer[0];

            switch (protocol)
            {
                case EProtocol.SetAttach:
                    EAttachPoint attachPoint = (EAttachPoint)inBuffer[1];
                    int x = inBuffer[2];
                    int y = inBuffer[3];
                    LogManager.Instance.Log(String.Format("Client Command: 修改挂点: {0}-{1},{2}", attachPoint.ToString(), x, y));
                    break;
            }
            try
            {
                if (pipeServer.IsConnected)
                {
                    pipeServer.Flush();
                    pipeServer.BeginRead(inBuffer, 0, pipeServer.InBufferSize, onClientDataReceived, pipeServer);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void LoadSpine(String spineName, String zipPath)
        {
            if (!pipeServer.IsConnected)
            {
                LogManager.Instance.Warn("RenderApp 未连接，无法加载Spine");
                return;
            }

            byte[] nameBuffer = Encoding.UTF8.GetBytes(spineName);
            byte[] zipBuffer = Encoding.UTF8.GetBytes(zipPath);

            byte[] allBuffer = new byte[3 + nameBuffer.Length + zipBuffer.Length];

            allBuffer[0] = (byte)EProtocol.LoadSpine;
            allBuffer[1] = (byte)nameBuffer.Length;
            nameBuffer.CopyTo(allBuffer, 2);
            allBuffer[2 + nameBuffer.Length] = (byte)zipBuffer.Length;
            zipBuffer.CopyTo(allBuffer, 3 + nameBuffer.Length);

            pipeServer.Write(allBuffer, 0, allBuffer.Length);
            pipeServer.Flush();
        }

        public void PlayAnim(String animName)
        {
            if (!pipeServer.IsConnected)
            {
                LogManager.Instance.Warn("RenderApp 未连接，无法播放动画");
                return;
            }

            // 播放动画
            byte[] animNameBuffer = Encoding.UTF8.GetBytes(animName);
            byte[] allBuffer = new byte[2 + animNameBuffer.Length];
            allBuffer[0] = (byte)EProtocol.PlayAnim;
            allBuffer[1] = (byte)animNameBuffer.Length;
            animNameBuffer.CopyTo(allBuffer, 2);

            pipeServer.Write(allBuffer, 0, allBuffer.Length);
            pipeServer.Flush();
        }

        #region Win32 API
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        private static extern long GetWindowThreadProcessId(long hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        private static extern long GetWindowLong(HandleRef hwnd, int nIndex);

        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, long dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, long dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hwnd);
        ///// <summary>
        ///// ShellExecute(IntPtr.Zero, "Open", "C:/Program Files/TTPlayer/TTPlayer.exe", "", "", 1);
        ///// </summary>
        ///// <param name="hwnd"></param>
        ///// <param name="lpOperation"></param>
        ///// <param name="lpFile"></param>
        ///// <param name="lpParameters"></param>
        ///// <param name="lpDirectory"></param>
        ///// <param name="nShowCmd"></param>
        ///// <returns></returns>
        //[DllImport("shell32.dll", EntryPoint = "ShellExecute")]
        //public static extern int ShellExecute(
        //IntPtr hwnd,
        //string lpOperation,
        //string lpFile,
        //string lpParameters,
        //string lpDirectory,
        //int nShowCmd
        //);
        //[DllImport("kernel32.dll")]
        //public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId); 
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);



        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_OVERLAPPED = 0x00000000;
        private const int WM_CLOSE = 0x10;
        private const int WS_CHILD = 0x40000000;

        private const int SW_HIDE = 0; //{隐藏, 并且任务栏也没有最小化图标}
        private const int SW_SHOWNORMAL = 1; //{用最近的大小和位置显示, 激活}
        private const int SW_NORMAL = 1; //{同 SW_SHOWNORMAL}
        private const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
        private const int SW_SHOWMAXIMIZED = 3; //{最大化, 激活}
        private const int SW_MAXIMIZE = 3; //{同 SW_SHOWMAXIMIZED}
        private const int SW_SHOWNOACTIVATE = 4; //{用最近的大小和位置显示, 不激活}
        private const int SW_SHOW = 5; //{同 SW_SHOWNORMAL}
        private const int SW_MINIMIZE = 6; //{最小化, 不激活}
        private const int SW_SHOWMINNOACTIVE = 7; //{同 SW_MINIMIZE}
        private const int SW_SHOWNA = 8; //{同 SW_SHOWNOACTIVATE}
        private const int SW_RESTORE = 9; //{同 SW_SHOWNORMAL}
        private const int SW_SHOWDEFAULT = 10; //{同 SW_SHOWNORMAL}
        private const int SW_MAX = 10;

        private const long WS_CAPTION = 0x00C00000L;
        private const long WS_THICKFRAME = 0x00040000L;
        private const long WS_MINIMIZE = 0x20000000L;
        private const long WS_MAXIMIZE = 0x01000000L;
        private const long WS_SYSMENU = 0x00080000L;

        private const long WS_EX_DLGMODALFRAME = 0x00000001L;
        private const long WS_EX_CLIENTEDGE = 0x00000200L;
        private const long WS_EX_STATICEDGE = 0x00020000L;

        private const long WS_POPUP = 0x80000000L;

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        //{同 SW_SHOWNORMAL}
        //const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        //const int PROCESS_VM_READ = 0x0010;
        //const int PROCESS_VM_WRITE = 0x0020;     
        #endregion Win32 API

    }
}
