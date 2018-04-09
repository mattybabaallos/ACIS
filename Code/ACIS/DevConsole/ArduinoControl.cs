using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevConsole
{
    class ArduinoControl
    {
        private SerialPort port;
        public ArduinoControl() { }

        public void Connect()
        {
            var list = SerialPort.GetPortNames();
            port = new SerialPort(list[0], 9600);
            port.Open();
            port.Write(SendCommand(0,1,10),0, 2);

        }

        public byte[] SendCommand(byte device, byte op, byte distance)
        {
            byte [] command = new byte[2];
            byte temp = 0;
            temp = (byte)(device & (byte)CrateMask(0,3));
            temp = (byte)((byte)(op << 3) | (byte)CrateMask(0, 3));
            command[0] = temp;
            command[1] = (byte)distance;
            return command;
        }

        private uint CrateMask(int start, int end)
        {

            int i;
            uint mask = 0;
            uint one = 1; // used because default magic # is int

            for (i = start; i <= end; ++i)
            {
                mask |= (one << i);
            }
            return mask;
        }

    }
}
