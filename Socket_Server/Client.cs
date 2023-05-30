using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace Socket_Server
{
    class Client
    {
        private TcpClient tcp;
        private string nick;
        private bool waitingPing;

        public Client(TcpClient p_tcp)
        {
            tcp = p_tcp;
            nick = "Anonymous";
            waitingPing = false;
        }

        public TcpClient GetTcpClient() { return tcp; }
        public string GetNick() { return nick; }
        public bool GetWaitingPing() { return waitingPing; }
        public void SetWaitingPing(bool p_waitingPing) { waitingPing = p_waitingPing; }
    }
}
