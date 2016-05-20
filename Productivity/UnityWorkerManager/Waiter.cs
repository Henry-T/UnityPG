using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace UnityWorkerManager
{
    /// <summary>
    /// 接受客户端的连接请求
    /// </summary>
    public class Waiter
    {
        public TcpListener TcpListener;
        public List<Client> Users;
        
        public Waiter(int port)
        {
            Users = new List<Client>();

            TcpListener = new TcpListener(port);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(Receive, TcpListener);
        }

        public void Receive(IAsyncResult ar)
        {
            TcpListener tcpListener = (TcpListener)ar.AsyncState;
            TcpClient client = tcpListener.EndAcceptTcpClient(ar);
            Client newUser = new Client(client);

            TcpListener.BeginAcceptTcpClient(Receive, TcpListener);
        }        
    }
}
