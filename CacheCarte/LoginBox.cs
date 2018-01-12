using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheCarte
{
    public partial class LoginBox : Form
    {
        public static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int PORT = 100;
        private static Boolean recu = false;
        private static string[] infos;
        private static string mypseudo;

        public LoginBox()
        {
            InitializeComponent();
        }

        private void LoginBox_Load(object sender, EventArgs e)
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
        }

        private void LoginButtonConnexion_Click(object sender, EventArgs e)
        {
            string pseudo = "mypseudo:"+LoginInputPseudo.Text;
            mypseudo = LoginInputPseudo.Text;
            if (pseudo.Length > 0)
            {
                
                while (!recu)
                {
                    SendRequest(pseudo);
                    ReceiveResponse();
                }
                Game g = new Game(infos,mypseudo,ClientSocket);
                g.Text = mypseudo;
                g.Show();
                Hide();
            }
        }
        private static void ReceiveResponse()
        {
            var buffer = new byte[1024];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);

            if (text.Contains("begindata")){
                MessageBox.Show(text, "vous avez recu", MessageBoxButtons.OK);
                recu = true;
                infos = text.Split(';');
            }

        }
        private static void SendRequest(string request)
        {
            Console.WriteLine("Send a request: " + request);

            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
        private static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }
    }
}


