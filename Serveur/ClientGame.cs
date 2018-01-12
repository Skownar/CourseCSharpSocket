using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServeurApp
{
    class ClientGame
    {
        private string _pseudo;
        private Socket _socket;

        public ClientGame(string pseudo,Socket socket)
        {
            _pseudo = pseudo;
            _socket = socket;
        }

        public string Pseudo { get => _pseudo; set => _pseudo = value; }
        public Socket Socket { get => _socket; set => _socket = value; }
    }
}
