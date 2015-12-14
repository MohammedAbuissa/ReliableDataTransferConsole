using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Utility;
using SelectiveRepeat;
namespace ReliableDataTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            int k =  int.Parse(Console.ReadLine().Trim());
            if (k==0)
            {
                RunServer();
            }
            else
            {
                RunClient();
            }
            Console.ReadLine();
        }

        static void RunServer()
        {
            int PacketSize;
            do
            {
                Console.WriteLine("Insert the width of packet");
            } while (!int.TryParse(Console.ReadLine().Trim(),out PacketSize));
            PacketTrunk pt = new PacketTrunk(PacketSize);
            using (FileStream stream = File.Open(@"Packet.txt",FileMode.Open))
            {
                pt.AddData(stream);
            }
            byte NumberOfInterval;
            do
            {
                Console.WriteLine("Insert the number of clock cycles to timeout");
            } while (!byte.TryParse(Console.ReadLine().Trim(), out NumberOfInterval));
            int ServerPort;
            do
            {
                Console.WriteLine("Insert Server port number");
            } while (!int.TryParse(Console.ReadLine().Trim(), out ServerPort));
            byte[] ClientAddress = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                do
                {
                    Console.WriteLine("Insert the " + i + "byte of the Client Address");
                } while (!byte.TryParse(Console.ReadLine().Trim(), out ClientAddress[i]));
            }
            int ClientPort;
            do
            {
                Console.WriteLine("Insert Client port number");
            } while (!int.TryParse(Console.ReadLine().Trim(), out ClientPort));
            byte WindowSize;
            do
            {
                Console.WriteLine("Insert the window size");
            } while (!byte.TryParse(Console.ReadLine().Trim(), out WindowSize));
            double Probability;
            do
            {
                Console.WriteLine("Insert probability");
            } while (!double.TryParse(Console.ReadLine().Trim(), out Probability));
            int Seed;
            do
            {
                Console.WriteLine("Insert Seed");
            } while (!int.TryParse(Console.ReadLine().Trim(), out Seed));

            Sender S = new Sender(pt, NumberOfInterval, ServerPort,
                new System.Net.IPAddress(ClientAddress), ClientPort
                , WindowSize, Probability, Seed);
        }
        static void RunClient()
        {
            Receiver R = new Receiver(5001, 22, 
                new System.Net.IPAddress(new byte[] { 127,0,0,1}), 5000, 2);
            R.Receive();
            MemoryStream ms = R.ReceivedData.GetData();
            using (FileStream stream = new FileStream("Result.txt",FileMode.OpenOrCreate))
            {
                ms.WriteTo(stream);
                stream.Close();
            }
            ms.Dispose();
        }
    }
}
