using ServeurApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Serveur
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
        private static List<String> listCard = new List<String>();
        private static string cardRandomized;
        private static List<String> listCardGame = new List<String>();
        static Random rnd = new Random();
        static string returned = "cards;";
        static void Main()
        {
            Console.Title = "Server";
            cardRandomized = RandomizeCard();
            SetupServer();
            Console.ReadLine(); // When we press enter close everything
            CloseAllSockets();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Démarrage serveur...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Serveur démarré avec succès");
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        /// </summary>
        private static void CloseAllSockets()
        {
            foreach (Socket cs in clientSockets)
            {
                cs.Shutdown(SocketShutdown.Both);
                cs.Close();
            }

            serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
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

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
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

        private static void SendMessage(Socket socket,string data)
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
                    clientSockets[0].Send(Encoding.ASCII.GetBytes("begindata;1;"+pseudos[1]));
                    clientSockets[1].Send(Encoding.ASCII.GetBytes("begindata;2;" + pseudos[0]));
                }
                counterPseudo++;
                Console.WriteLine("compteur de pseudo : " + counterPseudo);
            }

            if (data.StartsWith("cardorder?"))
            {
                clientSockets[client].Send(Encoding.ASCII.GetBytes(cardRandomized));
                Console.WriteLine("Envoi de l'ordre des cartes au client de : " + pseudos[client]);
            }
            if (data.StartsWith("coup"))
            {
                if (socket.Equals(clientSockets[0]))
                {
                    clientSockets[1].Send(Encoding.ASCII.GetBytes(data));
                    Console.WriteLine("Envoi à {0} que {1} a fait l'action {2}", pseudos[1], pseudos[0], data);
                }
                if (socket.Equals(clientSockets[1]))
                {
                    clientSockets[0].Send(Encoding.ASCII.GetBytes(data));
                    Console.WriteLine("Envoi à {0} que {1} a fait l'action {2}", pseudos[0], pseudos[1], data);

                }
            }
        }
        static void addCard()
        {
            listCard.Add("ethernet");
            listCard.Add("vga");
            listCard.Add("fibre");
            listCard.Add("serie");
            listCard.Add("dvi");
        }
        private static string RandomizeCard()
        {
            addCard();
            int cpt = 0;
            Boolean exist = false;
            for (int i = 0; i < 10; i++)
            {

                exist = false;
                String tmp = "";
                while (!exist)
                {
                    cpt = 0;
                    tmp = listCard.ElementAt(rnd.Next(5));
                    if (listCardGame.Count() != 0)
                    {
                        foreach (String s in listCardGame)
                        {
                            if (s.Equals(tmp))
                                cpt++;
                        }
                    }
                    if (cpt < 2)
                    {
                        listCardGame.Add(tmp);
                        exist = true;
                    }
                }
            }
            foreach (String s in listCardGame)
            {
                returned += s;
                returned += ";";
            }
            Console.WriteLine("------ Ordre des cartes généré -----");
            Console.WriteLine(returned);
            
            return returned;
        }
    }
}

