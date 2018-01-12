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
        static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        static readonly List<Socket> clientsSocket = new List<Socket>();
        static string[] pseudos;
        // Le port d'écoute est configuré dans le fichier App.config, ainsi même si le projet est compilé
        // On peut toujours changer le port d'écoute. (Au cas où ce port est utilisé par un autre service/programme).
        static readonly int PORT = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"].ToString());
        
        const int BUFFER_SIZE = 2048;
        readonly byte[] buffer = new byte[BUFFER_SIZE];

        static void Main(string[] args)
        {
            Console.Title = "ServerTicTacToe";
            StartServer();  // [ Appel la méthode SetupServer, qui attend des instructions en boucle
            Console.Read(); //   Si on appuie sur une touche, le serveur cloturera les connexions clients]
            CloseAllSockets();
        }

        private static void StartServer()
        {

            Console.WriteLine("Server Startup");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        private static void CloseAllSockets()
        {
            throw new NotImplementedException();
        }
    }
}
