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
        public byte[] Packet { get; set; }
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
        public Frame(byte SequenceNumber)
        {
            Counter = 0;
            this.SequenceNumber = SequenceNumber;
            Received = false;
        }
        public Frame(byte SequenceNumber, byte[] Packet):this(SequenceNumber)
        {
            this.Packet = Packet;
            Data = new byte[Packet.Length + 1];
            Data[0] = SequenceNumber;
            for (int i = 0; i < Packet.Length; i++)
            {
                Data[i + 1] = Packet[i];
            }
        }
        
    }
}
