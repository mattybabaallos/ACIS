using Data;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Services
{
    public class ArduinoControl
    {
        private SerialPort port;
        private AutoResetEvent m_autoEvent; //Thead synchrontion
        public ArduinoControl(AutoResetEvent autoEvent, CancellationTokenSource cancellation)
        {
            m_autoEvent = autoEvent;
            port = new SerialPort();
            Cancellation = cancellation;
        }

        public CancellationTokenSource Cancellation { get; set; }

        public void Connect()
        {
            port = new SerialPort(PortList[0], Constants.BAUD_RATE);
            port.DtrEnable = true;
            port.ReceivedBytesThreshold = Constants.BUFFER_SIZE;
            port.Open();
            IsConnected = true;
        }

        public bool IsConnected { get; set; }

        public void Connect(string portName)
        {
            port = new SerialPort(portName, Constants.BAUD_RATE);
            port.DtrEnable = true;
            port.ReceivedBytesThreshold = Constants.BUFFER_SIZE;
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
        /// <param name="function"> the operation id</param>
        /// <param name="data"> the data the device will travel if it's a stepper</param>
        /// <returns>byte[] that can sent to the Arduino</returns>
        public byte[] SendCommand(byte device, byte function, int data)
        {
            byte[] command = new byte[Constants.BUFFER_SIZE];
            command[0] = device;
            command[1] = function;
            command[2] = (byte)data;
            command[3] = (byte)(data >> 8);
            command[4] = (byte)(data >> 16);
            command[5] = 0;
            port.Write(command, 0, Constants.BUFFER_SIZE);
            return command;
        }

        public byte[] SendCommand(Devices motor, Functions function, int data)
        {
            return SendCommand((byte)motor, (byte)function, data);
        }

        public byte[] SendCommandBlocking(Devices motor, Functions function, int data)
        {
            var val = SendCommand((byte)motor, (byte)function, data);
            m_autoEvent.WaitOne();
            Cancellation.Token.ThrowIfCancellationRequested();
            return val;

        }

        public byte[] SendCommandBlocking(byte device, byte function, int data)
        {
            var val = SendCommand(device, function, data);
            m_autoEvent.WaitOne();
            Cancellation.Token.ThrowIfCancellationRequested();
            return val;
        }


        public void ReciveCommand(ref int device, ref int function ,ref int status, ref int data, ref int errorCode)
        {
            if (port.BytesToRead >= Constants.BUFFER_SIZE)
            {
                byte[] buffer = new byte[Constants.BUFFER_SIZE];
                port.Read(buffer, 0, Constants.BUFFER_SIZE);
                device = buffer[0];
                function = buffer[1];
                data = 0;
                data = ((data | buffer[4]) << 16) | ((data | buffer[3]) << 8) | (data | buffer[2]);
                errorCode = buffer[5];
            }
        }


    }
}
