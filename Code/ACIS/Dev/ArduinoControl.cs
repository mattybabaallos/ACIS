using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;


namespace Dev
{
    public class ArduinoControl
    {
        private SerialPort port;
        public ArduinoControl() { }

        public void Connect()
        {
            var list = SerialPort.GetPortNames();
            port = new SerialPort(list[0], 9600);
            port.Open();
            port.Write(SendCommand((byte)Motors.X_AXIS_TOP,(byte)ArduinoFunctions.MOVE_FORWARD,100),0, Constants.NUMBER_OF_BYTES_TO_SEND);
        }


        /// <summary>
        /// Construct the byte array to will be sent to the Arduino.
        /// </summary>
        /// <param name="device"> The device id</param>
        /// <param name="op"> the operation id</param>
        /// <param name="distance"> the distance the device will travel if it's a stepper</param>
        /// <returns>byte[] that can sent to the Arduino</returns>
        public byte[] SendCommand(byte device, byte op, byte distance)
        {
            byte [] command = new byte[2];
            byte temp = 0;
            temp = (byte)(device & (byte)CrateMask(0,2));
            temp = (byte)((op << 3) | temp);
            command[0] = temp;
            command[1] = (byte)distance;
            return command;
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
