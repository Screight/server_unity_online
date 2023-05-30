using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

namespace Socket_Server
{
    public class Network_Manager
    {
        private List<Client> clients;
        private TcpListener serverListener;
        private Mutex clientListMutex;
        private int lastTimePing;
        private List<Client> disconnectClients;

        public Network_Manager()
        {
            serverListener = new TcpListener(IPAddress.Any, 6543);
            clients = new List<Client>();
            clientListMutex = new Mutex();
            this.lastTimePing = Environment.TickCount;
            this.disconnectClients = new List<Client>();
        }

        public void Start_Network_Service()
        {
            try
            {
                this.serverListener.Start();
                StartListening();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void StartListening()
        {
            Console.WriteLine("Esperando nueva conexión");
            this.serverListener.BeginAcceptTcpClient(AcceptConnection,
            this.serverListener);
        }

        private void AcceptConnection(IAsyncResult ar)
        {
            Console.WriteLine("Recibo una conexion");

            TcpListener listener = (TcpListener)ar.AsyncState;
            this.clientListMutex.WaitOne();
            this.clients.Add(new Client(listener.EndAcceptTcpClient(ar)));
            this.clientListMutex.ReleaseMutex();
            StartListening();
        }

        public void CheckMessage()
        {
            clientListMutex.WaitOne();
            foreach (Client client in this.clients)
            {
                NetworkStream netStream = client.GetTcpClient().GetStream();

                if (netStream.DataAvailable)
                {
                    StreamReader reader = new StreamReader(netStream, true);
                    string data = reader.ReadLine();
                    if (data != null)
                    {
                        ManageData(client, data);
                    }
                }
            }
            clientListMutex.ReleaseMutex();
        }

        private void ManageData(Client client, string data)
        {
            string[] parameters = data.Split('/');
            switch (parameters[0])
            {
                case "0":
                    Login(parameters[1], parameters[2], client);
                    break;
                case "1":
                    ReceivePing(client);
                    break;
                case "2":
                    GetAndSendClasses(client);
                    break;
                case "3":
                    ConfirmConnection(client);
                    break;
                case "4":
                    Register(parameters[1], parameters[2], parameters[3], client);
                    break;
            }
        }

        void GetAndSendClasses(Client client)
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

            List<Race> raceList = Database_Manager._DATABASE_MANAGER.GetClasses();

            string returnValue = "races;";

            if (raceList == null) { returnValue += "0"; }
            else
            {
                returnValue += "1";

                foreach (Race race in raceList)
                {
                    returnValue += ";";
                    returnValue += race.ID + ":";
                    returnValue += race.Name + ":";
                    returnValue += race.MaxHealth + ":";
                    returnValue += race.Damage + ":";
                    returnValue += race.Speed + ":";
                    returnValue += race.JumpForce + ":";
                    returnValue += race.FireRate;
                }
            }

            writer.WriteLine(returnValue);
            writer.Flush();
        }

        void Register(string nick, string password, string p_raceID, Client client)
        {
            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

            string userName = Database_Manager._DATABASE_MANAGER.TryRegisteringUser(nick, password, p_raceID);

            string returnValue = "";

            if (userName == null) { returnValue = "register;0"; }
            else
            {
                returnValue = "register;1;";
                returnValue += userName + ";";
            }

            writer.WriteLine(returnValue);

            writer.Flush();
        }

        private void Login(string nick, string password, Client client)
        {
            Console.WriteLine("Petición de: " + nick + " usando la clave: " +
            password);

            StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());

            User? user = Database_Manager._DATABASE_MANAGER.TryGettingUser(nick, password);

            string returnValue = "";

            if (user == null) { returnValue = "login;0"; }
            else {
                returnValue = "login;1;";
                returnValue += user.Value.Name + ";";
                returnValue += user.Value.RaceID + ";";
            }

            writer.WriteLine(returnValue);

            writer.Flush();
        }

        private void SendPing(Client client)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
                writer.WriteLine("ping");
                writer.Flush();
                client.SetWaitingPing(true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + " with client" +
                client.GetNick());
            }
        }
        void ConfirmConnection(Client client)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.GetTcpClient().GetStream());
                writer.WriteLine("connection");
                writer.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + " with client" +
                client.GetNick());
            }
        }
        public void CheckConnection()
        {
            if (Environment.TickCount - this.lastTimePing > 5000)
            {
                clientListMutex.WaitOne();
                foreach (Client client in this.clients)
                {
                    if (client.GetWaitingPing() == true)
                    {
                        disconnectClients.Add(client);
                    }
                    else
                    {
                        SendPing(client);
                    }
                }
                this.lastTimePing = Environment.TickCount;
                clientListMutex.ReleaseMutex();
            }
        }

        public void DisconnectClients()
        {
            clientListMutex.WaitOne();
            foreach (Client client in this.disconnectClients)
            {
                Console.WriteLine("Desconectando usuarios");
                client.GetTcpClient().Close();
                this.clients.Remove(client);
            }
            this.disconnectClients.Clear();
            clientListMutex.ReleaseMutex();
        }

        private void ReceivePing(Client client) { client.SetWaitingPing(false); }

    }
}