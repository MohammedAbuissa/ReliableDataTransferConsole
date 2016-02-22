using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility;
using System.Timers;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
namespace SelectiveRepeat
{
    using timer = System.Timers.Timer;
    public class Sender
    {
        #region Socket
        private Socket SendSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        private Socket ReceiveSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ClientAddress;
        #endregion
        #region Data
        private List<Frame> Buffer;
        public PacketTrunk Packet { get; private set; }
        private byte _Base = 0;
        private byte Base
        {
            get {
                return _Base;
            }
            set {
                _Base = (byte)(value % SequenceNumberRange);
            }
        }
        private byte SequenceNumberRange;
        private byte _ExpectedSequenceNumber = 0;
        byte ExpectedSequenceNumber
        {
            get {
                return _ExpectedSequenceNumber;
            }
            set {
                _ExpectedSequenceNumber = (byte)(value % SequenceNumberRange);
            }
        }
        private byte WindowSize;
        Random R ;
        int Probability;
        #endregion
        #region Thread
        private timer AckTimer = new timer(0.25);
        private byte NumberOfInterval;
        private Thread ReceiveThread;
        private bool EndOfFile = false;
        #endregion
        
        public Sender(PacketTrunk Packet,byte NumberOfInterval,int ServerPort, IPAddress ClientHost,int ClientPort, byte SlideWindowSize,double Probability, int seed)
        {
            ReceiveSocket.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
            SendSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            ClientAddress = new IPEndPoint(ClientHost, ClientPort);
            if (SlideWindowSize > 127)
                throw new Exception("SlideWindowSize must be less than 128");
            WindowSize = SlideWindowSize;
            SequenceNumberRange = (byte)(2 * WindowSize);
            R = new Random(seed);
            this.Probability = (int)(Probability * 100);
            Buffer = new List<Frame>(SlideWindowSize);
            this.Packet = Packet;
            this.NumberOfInterval = NumberOfInterval;
            AckTimer.Elapsed += AckTimer_Elapsed;
            ReceiveThread = new Thread(new ThreadStart(ReceiveACK));
            for (int i = 0; i < Buffer.Capacity; i++)
            {
                byte[] dummy = Packet.NextPacket;
                if(dummy !=null)
                    Buffer.Add(new Frame(ExpectedSequenceNumber++, dummy));
            }
            Console.WriteLine(Packet.NumberofPackets);
            Console.Read();
            ReceiveThread.Start();
            AckTimer.Start();
            ReceiveThread.Join();
        }

        private void AckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //call the method to edit buffer
            CriticalSection(false);
        }
        private byte DeliverTo;
        private void ReceiveACK()
        {
            byte[] ExpectedSequenceAck = new byte[SequenceNumberRange];
            
            byte[] buffer = new byte[1];
            EndPoint Garbage = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                for (int i = 0; i < ExpectedSequenceAck.Length; i++)
                {
                    ExpectedSequenceAck[i] = (byte)((Base + i) % (SequenceNumberRange));
                }
#if DEBUG
                Console.WriteLine("waiting for ack");
#endif

                ReceiveSocket.ReceiveFrom(buffer, ref Garbage);
                if(ExpectedSequenceAck.Contains(buffer[0]))
                {
                    //good ack received
                    //edit Buffer acording to sum criteria 
                    DeliverTo = buffer[0];
                    CriticalSection(true);
                }
                if (EndOfFile)
                {
                    break;
                }
            }
            SendSocket.SendTo(new byte[] {255}, ClientAddress);
        }
        //critical section
        private object Lock = new object();
        private void CriticalSection(bool Me)
        {
            lock (Lock)
            {
                if (Me)//AckReceiver
                {
#if DEBUG
                    Console.WriteLine("Ack " + DeliverTo);
#endif

                    Frame dummy = Buffer.Find(x => x.SequenceNumber == DeliverTo);
                    if(dummy != null)
                        dummy.Received = true;
                    while (Buffer.Count>0 && Buffer[0].Received)
                    {
#if DEBUG
                        Console.WriteLine("moving window");
#endif
                        
                        Buffer.RemoveAt(0);
                        Base++;
                        byte[] temporary = Packet.NextPacket;
                        if (temporary != null)
                        {
                            Buffer.Add(new Frame(ExpectedSequenceNumber++, temporary));
                        }
                    }
                    if (Buffer.Count < 1)
                    {
                        EndOfFile = true;
                        AckTimer.Stop();
                        AckTimer.Dispose();
                    }
                        
                }
                    
                else//PacketSender(Timer)
                {
                    //resend all timed out packets
                    List<Frame> dummy = Buffer.FindAll(x => (x.Counter == 0 && !x.Received));
                    foreach (var item in dummy)
                    {
#if DEBUG
                        Console.WriteLine("reSending packet # " + item.SequenceNumber);
#endif

                        if (R.Next(101)>Probability)
                            SendSocket.SendTo(item.Data, ClientAddress);
                        item.Counter = NumberOfInterval;
                    }
                    //minus the counter of each packet
                    foreach (var item in Buffer)
                    {
                        item.Counter--;
                    }
                }
            }
        }
        

    }
}
