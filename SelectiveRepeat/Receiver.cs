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
    public class Receiver
    {
        #region Socket
        Socket ReceiveSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        Socket SendSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ServerAddress;
        #endregion
        #region Data
        private List<Frame> Buffer;
        public PacketTrunk ReceivedData { get; set; }
        private byte WindowSize;
        private byte _Base;
        private byte Base
        {
            get
            {
                return _Base;
            }
            set
            {
                _Base = (byte)(value % SequenceNumber);
            }
        }
        private byte _NextSeqNumber = 0;
        public byte NextSeqNumber
        {
            get
            {
                return _NextSeqNumber;
            }
            set
            {
                _NextSeqNumber = (byte)(value % SequenceNumber);
            }
        }
        private byte SequenceNumber;
        private int PacketSize;
        #endregion


        public Receiver(int ClientPort, int PacketSize,IPAddress ServerAddress, int ServerPort,byte WindowSize)
        {
            ReceiveSocket.Bind(new IPEndPoint(IPAddress.Any, ClientPort));
            this.ServerAddress = new IPEndPoint(ServerAddress, ServerPort);
            ReceivedData = new PacketTrunk(PacketSize);
            Buffer = new List<Frame>(WindowSize);
            SequenceNumber = (byte)(2 * WindowSize);
            this.WindowSize = WindowSize;
            this.PacketSize = PacketSize;
            for (int i = 0; i < WindowSize; i++)
            {
                Buffer.Add(new Frame(NextSeqNumber++));
            }
            
        }
        public void Receive()
        {
            EndPoint SenderAdress = new IPEndPoint(IPAddress.Any, 0);
            byte[] ExpSeq = new byte[WindowSize];
            byte[] buffer = null;
            do
            {
                buffer = new byte[PacketSize+1];
                for (int i = 0; i < WindowSize; i++)
                {
                    ExpSeq[i] = (byte)((Base + i) % SequenceNumber);
                }
                ReceiveSocket.ReceiveFrom(buffer, ref SenderAdress);
                if(ExpSeq.Contains(buffer[0]))
                {
                    BufferData(buffer);
                    Console.WriteLine("Received Packet: " + buffer[0] + Encoding.ASCII.GetString(buffer,1,buffer.Length-1));
                }
                SendSocket.SendTo(new byte[] { buffer[0] }, ServerAddress);
            } while (buffer[0]!=255);
        }
        private void BufferData (byte[] Packet)
        {
            Frame Container = Buffer.Find(x => x.SequenceNumber == Packet[0]);
            Container.Packet = Packet;
            Container.Received = true;
            while (Buffer[0].Received)
            {
                byte[] Dummy = new byte[Packet.Length - 1];
                Array.Copy(Buffer[0].Packet, 1, Dummy, 0, Dummy.Length);
                ReceivedData.Trunk.Add(Dummy);
                Buffer.RemoveAt(0);
                Buffer.Add(new Frame(NextSeqNumber++));
                Base++;
            }
        }
    }
}
