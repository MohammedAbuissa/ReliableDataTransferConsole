using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Utility;
namespace SelectiveRepeat
{
    class Receiver
    {
        #region Socket
        Socket ReceiveSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        Socket SendSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ServerAddress;
        #endregion
        #region Data

        #endregion
        public Receiver(int ClientPort, IPAddress ServerAddress, int ServerPort,byte WindowSide)
        {
            ReceiveSocket.Bind(new IPEndPoint(IPAddress.Any, ClientPort));
            this.ServerAddress = new IPEndPoint(ServerAddress, ServerPort);

        }
    }
}
