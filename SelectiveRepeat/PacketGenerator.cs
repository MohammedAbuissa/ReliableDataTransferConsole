using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Utility
{
    public class PacketTrunk
    {
        private int PacketSize;
        private int LastAccess = 0;
        private List<byte[]> Trunk { get; set; }
        public byte[] NextPacket {
            get
            {
                if (LastAccess < Trunk.Count - 1)
                    return Trunk[LastAccess++];
                else
                    return null;
            }
        }
        public int NumberofPackets { get { return Trunk.Count; } }
        public PacketTrunk(int PacketSize)
        {
            this.PacketSize = PacketSize;
            Trunk = new List<byte[]>();
            Trunk.Add(new byte[PacketSize]);
        } 

        public void AddData(byte[] Dataium)
        {
            Trunk.Add(Dataium);
        }
        public void AddData(Stream DataStream)
        {
            using (Stream reader = DataStream)
            {
                while(reader.Read(Trunk.Last(), 0, Trunk.Last().Length) > 0)
                    Trunk.Add(new byte[PacketSize]);
            }
        }
        public MemoryStream GetData()
        {
            MemoryStream ms = new MemoryStream();
            foreach(byte[] packet in Trunk)
            {
                ms.Write(packet, 0, packet.Length);
            }
            return ms;
        }
        public void Reset()
        {
            LastAccess = 0;
        }
    }
}
