using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;


namespace Services
{
    public class ArduinoControl
    {
        private SerialPort port;
        public ArduinoControl() { }

        public void Connect()
        {
            port = new SerialPort(PortList[0], Constants.BAUD_RATE);
            port.Open();
        }

        public void Connect(string portName)
        {
            port = new SerialPort(portName, Constants.BAUD_RATE);
            port.Open();
        }

        public void Close()
        {
            port.Close();
        }

        public List<string> PortList => SerialPort.GetPortNames().ToList();

        /// <summary>
        /// Construct the byte array to will be sent to the Arduino and send it.
        /// </summary>
        /// <param name="device"> The device id</param>
        /// <param name="op"> the operation id</param>
        /// <param name="distance"> the distance the device will travel if it's a stepper</param>
        /// <returns>byte[] that can sent to the Arduino</returns>
        public byte[] SendCommand(byte device, byte op, byte distance)
        {
            byte[] command = new byte[Constants.NUMBER_OF_BYTES_TO_SEND];
            byte temp = 0;
            temp = (byte)(device & (byte)CrateMask(0, 2));
            temp = (byte)((op << 3) | temp);
            command[0] = temp;
            command[1] = (byte)distance;

            port.Write(command, 0, Constants.NUMBER_OF_BYTES_TO_SEND);
            return command;
        }


        public void ReciveCommand(ref int device, ref int op, ref int distance)
        {
            if (port.BytesToRead >= Constants.NUMBER_OF_BYTES_TO_SEND)
            {
                byte[] buffer = new byte[Constants.NUMBER_OF_BYTES_TO_SEND];
                port.Read(buffer, 0, Constants.NUMBER_OF_BYTES_TO_SEND);
                byte temp = buffer[0];
                device = (int)(temp & CrateMask(0, 2));
                op = (int)(temp >> 3);
                distance = buffer[1];
            }
        }

        /// <summary>
        /// Create a bit mask.
        /// </summary>
        /// <param name="start">The start of the bit mask 0 inclusive</param>
        /// <param name="end"> The end of the bit mask 0 inclusive</param>
        /// <returns>the bit mask</returns>
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
