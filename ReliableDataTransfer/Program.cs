using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Utility;
using SelectiveRepeat;
using System.Diagnostics;
namespace ReliableDataTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int k =  int.Parse(Console.ReadLine().Trim());
            if (k==0)
            {
                sw.Start();
                RunServer();
                sw.Stop();
            }
            else
            {
                sw.Start();   
                RunClient();
                sw.Stop();
            }
            Console.WriteLine("Done "+ sw.Elapsed.Ticks);
            Console.ReadLine();
        }

        static void RunServer()
        {
            Console.WriteLine("Insert file name");
            string Path = Console.ReadLine();
            int PacketSize;
            do
            {
                Console.WriteLine("Insert the width of packet");
            } while (!int.TryParse(Console.ReadLine().Trim(),out PacketSize));
            PacketTrunk pt = new PacketTrunk(PacketSize);
            using (FileStream stream = File.Open(Path,FileMode.Open))
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
            int ClientPort;
            do
            {
                Console.WriteLine("Insert Client pory=t");
            } while (!int.TryParse(Console.ReadLine().Trim(), out ClientPort));
            int PacketSize;
            do
            {
                Console.WriteLine("Insert PacketSize");
            } while (!int.TryParse(Console.ReadLine().Trim(), out PacketSize));
            byte[] ServerIp = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                do
                {
                    Console.WriteLine("Insert "+i+"byte of server address");
                } while (!byte.TryParse(Console.ReadLine().Trim(), out ServerIp[i]));
            }
            int ServerPort;
            do
            {
                Console.WriteLine("Insert ServerPort");
            } while (!int.TryParse(Console.ReadLine().Trim(), out ServerPort));
            byte WindowSize;
            do
            {
                Console.WriteLine("Insert Windows Size");
            } while (!byte.TryParse(Console.ReadLine().Trim(), out WindowSize));
            Console.WriteLine("Insert Name of File to write the result");
            string File = Console.ReadLine();
            Receiver R = new Receiver(ClientPort,PacketSize,
                new System.Net.IPAddress(ServerIp), ServerPort,WindowSize);
            R.Receive();
            MemoryStream ms = R.ReceivedData.GetData();
            using (FileStream stream = new FileStream(File,FileMode.OpenOrCreate))
            {
                ms.WriteTo(stream);
                stream.Close();
            }
            ms.Dispose();
            
        }
    }
}
