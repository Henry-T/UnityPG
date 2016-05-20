using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace UnityWorkerManager
{
    public class Client
    {
        public TcpClient TcpClient;
        public string Name;
        public Client(TcpClient client)
        {
            TcpClient = client;

            Byte[] buffer = new Byte[TcpClient.Client.ReceiveBufferSize];
            TcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, ar =>
            {
                
                string info = System.Text.Encoding.UTF8.GetString(buffer);
                //Console.WriteLine(info);

                Program.Console.WriteLine("一条消息来自 " + client.Client.RemoteEndPoint.ToString());
                Program.Console.WriteLine(info + "\n");
                Program.Console.DoOrder(info.Trim(new char[] { '\0', ' ' }));
                 

                /*
                EMessageType msgType;
                string message;
                TCPUtil.ParseMsg(buffer, out msgType, out message);

                XmlSerializer serializer = null;
                TextReader reader = new StringReader(message);

                switch(msgType)
                {
                    case EMessageType.UserLogin:
                        serializer = new XmlSerializer(typeof(Msg_UserLogin));
                        Msg_UserLogin msg_userLogin = serializer.Deserialize(reader) as Msg_UserLogin;
                        Name = msg_userLogin.UserName;
                        Program.Console.WriteLine("用户登录 " + Name);
                        break;
                }
                 */

            }, TcpClient);
        }
    }
}
