using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Xml;

namespace UnityWorkerTool
{    
    public partial class MainForm : Form
    {
        TcpClient tcpClient;
        string serverIP = "";
        int serverPort = 100;
        string userName;

        Queue<WorkQueueItem> MyWorkQueue = new Queue<WorkQueueItem>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            XmlNode docNode = xmlDoc.FirstChild;

            XmlElement serverEle = docNode.SelectSingleNode("Server") as XmlElement;
            serverIP = serverEle.Attributes["IP"].Value;
            serverPort = int.Parse(serverEle.Attributes["Port"].Value);

            XmlElement userNameEle = docNode.SelectSingleNode("UserName") as XmlElement;
            userName = userNameEle.Value;

            Connect();

            Msg_UserLogin msg = new Msg_UserLogin { UserName=userName};
            // TCPUtil.Send(tcpClient, EMessageType.UserLogin, typeof(Msg_UserLogin), msg);
        }

        public void Connect()
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(serverIP, serverPort);
            }
            catch(Exception e)
            {
                MessageBox.Show("连接失败 " + e.Data);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(tbCommand.Text);
            tcpClient.Client.Send(bytes);
        }

        private void menuBuildAndroid_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "-slave build_android -command build -platform android";
        }

        private void menuBuildIOS_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "-slave build_ios -command build -platform ios";
        }

        private void menuBuildWeb_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "-slave build_web -command build -platform web";
        }

        private void menuPackageAndroid_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "-slave package_android -command package -platform android -packages Box";
        }

        private void menuPackageIOS_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "-slave package_ios -command package -platform ios -packages Box";
        }

        private void menuPackageWeb_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "-slave package_web -command package -platform web -packages Box";
        }

        public Timer SendTimer;
        private int lastHour;

        private void btnSendFullHour_Click(object sender, EventArgs e)
        {
            lastHour = DateTime.Now.TimeOfDay.Hours;

            SendTimer = new Timer();
            SendTimer.Interval = 10000;     // 每10秒检查一次
            SendTimer.Tick += (Object s, EventArgs arg) => {
                int nowHour = DateTime.Now.TimeOfDay.Hours;
                if (nowHour > lastHour)
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(tbCommand.Text);
                    tcpClient.Client.Send(bytes);
                }
                lastHour = nowHour;
            };
            SendTimer.Start();
        }
    }
}
