using Data;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;


namespace Services
{
    public class ArduinoControl
    {
        private SerialPort port;
        public ArduinoControl()
        {
            port = new SerialPort();
        }

        public void Connect()
        {
            port = new SerialPort(PortList[0], Constants.BAUD_RATE);
            port.DtrEnable = true;
            port.ReceivedBytesThreshold = Constants.NUMBER_OF_BYTES_TO_RECEIVE;
            port.Open();
            IsConnected = true;
        }

        public bool IsConnected { get; set; }

        public void Connect(string portName)
        {
            port = new SerialPort(portName, Constants.BAUD_RATE);
            port.DtrEnable = true;
            port.ReceivedBytesThreshold = Constants.NUMBER_OF_BYTES_TO_RECEIVE;
            port.Open();
            IsConnected = true;
        }

        public void Close()
        {
            port.Close();
        }


        public event SerialDataReceivedEventHandler SerialDataReceived
        {
            add { port.DataReceived += value; }
            remove { port.DataReceived -= value; }
        }

        public ObservableCollection<string> PortList = new ObservableCollection<string>(SerialPort.GetPortNames());

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
            temp = (byte)(device & (byte)Common.CrateMask(0, 2));
            temp = (byte)((op << 3) | temp);
            command[0] = temp;
            command[1] = (byte)distance;

            port.Write(command, 0, Constants.NUMBER_OF_BYTES_TO_SEND);
            return command;
        }

        public byte[] SendCommand(Motors motor, ArduinoFunctions op, byte distance)
        {
            return SendCommand((byte)motor, (byte)op, distance);
        }


        public void ReciveCommand(ref int device, ref int op ,ref int status, ref int distance)
        {
            if (port.BytesToRead >= Constants.NUMBER_OF_BYTES_TO_RECEIVE)
            {
                byte[] buffer = new byte[Constants.NUMBER_OF_BYTES_TO_RECEIVE];
                port.Read(buffer, 0, Constants.NUMBER_OF_BYTES_TO_RECEIVE);
                device = (int)(buffer[0] & Common.CrateMask(0, 2));
                op = (int)(buffer[0] >> 3);
                distance = buffer[1];
                status = buffer[2];
            }
        }


    }
}
