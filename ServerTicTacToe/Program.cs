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
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private static string[] pseudos = new string[2];
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static int counterPseudo = 0;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        // Le port d'écoute est configuré dans le fichier App.config, ainsi même si le projet est compilé
        // On peut toujours changer le port d'écoute. (Au cas où ce port est utilisé par un autre service/programme).


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

            Console.WriteLine("Démarrage serveur...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Serveur démarré avec succès");
        }

        // Methode qui accepte la connexion client et attend leur instructions
        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(ar);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket current = (Socket)ar.AsyncState;
            int received;
            try
            {
                received = current.EndReceive(ar);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }
            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Message reçu: " + text);


            SendMessage(current, text);

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }

        // Methode d'envoi d'instructions aux clients
        // Elle regarde le message reçu , par qui, et répond en fonction au client opposé
        private static void SendMessage(Socket socket, string data)
        {
            int client = 0;
            if (socket.Equals(clientSockets[0]))
                client = 0;
            else if (socket.Equals(clientSockets[1]))
                client = 1;

            if (data.StartsWith("mypseudo:"))
            {
                pseudos[client] = data.Substring(9);
                Console.WriteLine("pseudo reçu :" + pseudos[client]);
                if (counterPseudo > 0)
                {
                    clientSockets[0].Send(Encoding.ASCII.GetBytes("begindata;1;" + pseudos[1]));
                    clientSockets[1].Send(Encoding.ASCII.GetBytes("begindata;2;" + pseudos[0]));
                }
                counterPseudo++;
                Console.WriteLine("compteur de pseudo : " + counterPseudo);
            }

            // si le message recu est la case jouée , on envoi a l'adversaire la case qui a été journée
            if (data.StartsWith("caseJouee:"))
            {
                if (socket.Equals(clientSockets[0]))
                {
                    clientSockets[1].Send(Encoding.ASCII.GetBytes(data));
                    Console.WriteLine("envoi à {0} que {1} a fait l'action {2}", pseudos[1], pseudos[0], data);
                }
                if (socket.Equals(clientSockets[1]))
                {
                    clientSockets[1].Send(Encoding.ASCII.GetBytes(data));
                    Console.WriteLine("envoi à {0} que {1} a fait l'action {2}", pseudos[0], pseudos[1], data);
                }
            }



        }

        private static void CloseAllSockets()
        {
            foreach (Socket s in clientSockets)
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }
    }
}
