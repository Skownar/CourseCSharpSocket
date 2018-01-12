using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTicTacToe
{
    /// <summary>
    /// Cette classe sera utilisée pour identifié un joueur connecté par son pseudo
    /// Mais aussi pour lui attribuer un socket "personnel". Il sera plus facile de communiquer avec le client du joueur avec ce socket.
    /// 
    /// This class will be used to identify a player by his username.
    /// But also to giving to him a "personal" socket. It will be more easy to communicate with the gamer's client with this socket
    /// </summary>
    class Client
    {
        #region Fields
        private string _pseudo;
        private Socket _socket;
        #endregion
        #region Constructor 
        #region EmptyCTOR
        public Client()
        {
            Pseudo = "";
            Socket = new Socket(SocketType.Stream,ProtocolType.Tcp);
        }
        #endregion
        #region FullCTOR
        public Client(string pseudo,Socket socket)
        {
            Pseudo = pseudo;
            Socket = socket;
        }
        #endregion
        #endregion
        #region Properties
        public Socket Socket { get => _socket; set => _socket = value; }
        public string Pseudo { get => _pseudo; set => _pseudo = value; }
        #endregion
    }
}
