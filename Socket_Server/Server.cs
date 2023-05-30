using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            bool bServerOn = true;

            Network_Manager Network_Service = new Network_Manager();
            Database_Manager Database_Manager = new Database_Manager();

            StartServices();

            while (bServerOn)
            {
                Network_Service.CheckConnection();
                Network_Service.CheckMessage();
                Network_Service.DisconnectClients();
            }

            void StartServices()
            {
                Network_Service.Start_Network_Service();
                Database_Manager.OpenConnection();
            }
            Database_Manager.CloseConnection();
        }
    }
}
