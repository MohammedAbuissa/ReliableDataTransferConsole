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
        public List<byte[]> Trunk { get; private set; }
        public byte[] NextPacket {
            get
            {
                if (LastAccess < Trunk.Count - 1)
                    return Trunk[LastAccess++];
                else
                    return null;
            }
        }
        public PacketTrunk(int PacketSize)
        {
            this.PacketSize = PacketSize;
            Trunk = new List<byte[]>();
            Trunk.Add(new byte[PacketSize]);
        } 

        public void AddData(Stream DataStream)
        {
            using (Stream reader = DataStream)
            {
                while(reader.Read(Trunk.Last(), 0, Trunk.Last().Length) > 0)
                    Trunk.Add(new byte[PacketSize]);
            }
        }
        public void Reset()
        {
            LastAccess = 0;
        }
    }
}
