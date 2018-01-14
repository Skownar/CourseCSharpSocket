using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTicTacToe
{
    class Program
    {
        static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static readonly List<Socket> clientsSocket = new List<Socket>();
        static string[] pseudos;
        // Le port d'écoute est configuré dans le fichier App.config, ainsi même si le projet est compilé
        // On peut toujours changer le port d'écoute. (Au cas où ce port est utilisé par un autre service/programme).
        static readonly int PORT = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"].ToString());

        const int BUFFER_SIZE = 2048;
        static readonly byte[] buffer = new byte[BUFFER_SIZE];

        static int compteurPseudo = 0;

        static void Main(string[] args)
        {
            Console.Title = "ServerTicTacToe";
            StartServer();  // [ Appel la méthode SetupServer, qui attend des instructions en boucle
            Console.Read(); //   Si on appuie sur une touche, le serveur cloturera les connexions clients]
            CloseAllSockets();
        }

        // Methode d'initialisation du serveur
        private static void StartServer()
        {

            Console.WriteLine("Server Startup");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server started succesfully");
        }

        // Methode qui accepte la connexion client et attend leur instructions
        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket inUse;
            try
            {
                inUse = serverSocket.EndAccept(ar);
            }
            catch (ObjectDisposedException e)
            {

                Console.WriteLine(e);
                return;
            }
            clientsSocket.Add(inUse);
            inUse.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, inUse);
            Console.WriteLine("Client connected, waiting for instruction");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket inUse = (Socket)ar.AsyncState;
            int received; // size of received data
            try
            {
                received = inUse.EndReceive(ar);
            }
            catch (Exception)
            {
                Console.WriteLine("[Error Client Socket] client disconnected");
                inUse.Close();
                clientsSocket.Remove(inUse);
                return;
            }
            byte[] receptionBuffer = new byte[BUFFER_SIZE];
            Array.Copy(buffer, receptionBuffer, received);
            string text = Encoding.ASCII.GetString(receptionBuffer);
            Console.WriteLine("GET : {0}", text);
            SendMessage(inUse, text);
            inUse.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, inUse);
        }

        // Methode d'envoi d'instructions aux clients
        // Elle regarde le message reçu , par qui, et répond en fonction au client opposé
        private static void SendMessage(Socket socket, string receiveMsg)
        {
            int client = 0;
            if (socket.Equals(clientsSocket[0]))   // regarde quel client envoi un message (joueur 1 ou joueur 2)
                client = 0;
            else
                client = 1;

            if (receiveMsg.StartsWith("mypseudo:"))
            {
                pseudos[client] = receiveMsg.Substring(9);
                Console.WriteLine("pseudo reçu :" + pseudos[client]);
                if (compteurPseudo > 0) // attend d'avoir reçu les deux pseudo pour les envoyer aux joueurs le pseudo de leur adversaire
                {
                    // envois aux joueurs que le jeu commence, leur numéro de joueur (j1 ou j2) et le pseudo de l'adversaire
                    clientsSocket[0].Send(Encoding.ASCII.GetBytes("begindata;1;" + pseudos[1]));
                    clientsSocket[1].Send(Encoding.ASCII.GetBytes("begindata;2;" + pseudos[0]));
                }
                compteurPseudo++; // incrémentation si on ne reçoit que le pseudo du j1
            }

            // si le message recu est la case jouée , on envoi a l'adversaire la case qui a été journée
            if (receiveMsg.StartsWith("caseJouee:"))
            {
                if (socket.Equals(clientsSocket[0]))
                {
                    clientsSocket[1].Send(Encoding.ASCII.GetBytes(receiveMsg));
                    Console.WriteLine("envoi à {0} que {1} a fait l'action {2}", pseudos[1], pseudos[0], receiveMsg);
                }
                if (socket.Equals(clientsSocket[1]))
                {
                    clientsSocket[1].Send(Encoding.ASCII.GetBytes(receiveMsg));
                    Console.WriteLine("envoi à {0} que {1} a fait l'action {2}", pseudos[0], pseudos[1], receiveMsg);
                }
            }



        }

        private static void CloseAllSockets()
        {
            foreach (Socket s in clientsSocket)
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }
    }
}
