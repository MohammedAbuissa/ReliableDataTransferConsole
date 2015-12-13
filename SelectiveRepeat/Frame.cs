using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectiveRepeat
{
    class Frame
    {
        public byte SequenceNumber { get; private set; }
        private byte[] Packet;
        private byte _Counter;
        public byte Counter
        {
            get
            {
                return _Counter;
            }
            set
            {
                if (value >= 0)
                    _Counter = value;
            }
        }
        public bool Received { get; set; }
        public byte[] Data { get; private set; }
        public Frame(byte SequenceNumber, byte[] Packet)
        {
            Counter = 0;
            this.SequenceNumber = SequenceNumber;
            this.Packet = Packet;
            Received = false;
            Data = new byte[Packet.Length + 1];
            Data[0] = SequenceNumber;
            for (int i = 0; i < Packet.Length; i++)
            {
                Data[i + 1] = Packet[i];
            }
        }
        
    }
}
