using Data;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows.Data;

namespace UI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor[] m_motors;

        private string m_selected_port = string.Empty;
        private object _lock = new object();
        int i = 0;

        //private List<string> m_error = new List<string>();

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];

            BindingOperations.EnableCollectionSynchronization(ErrorMessages, _lock);
            //ErrorMessages = "this a test error message \n \n ashdfjk\n ahsdj\n fklha\n slkd\n \n ashdfjk\n ahsdj\n fklha\n slkd\n fhalks a\n hs\n df asjdf\n h akjk\n jalsd\n hflas";
        }

        public void HomeAll()
        {
           
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_arduinoControl.SendCommand((byte)i, (byte)ArduinoFunctions.HOME, 0);
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Process code here.
            int device = -1, status = -1, op = -1, distance = -1;
            ++i;
            var port = (SerialPort)sender;
            ErrorMessages.Add(i.ToString());
            //m_arduinoControl.ReciveCommand(ref device, ref op, ref status, ref distance);
            //Process(device, op, status, distance);
        }

        public void Process(int device, int op, int status, int distance)
        {

            if (status > 0)
            {
                /*Error happened
                 * maybe print the error to the GUI and retry the command again. 
                 * Poissble keep track if the errors, if it happens mutiple times ask the user to perform a reboot
                 * or look into the issue.
                 * 
                */
            }

            if (op != (int)ArduinoFunctions.STOP)
                m_motors[device].Position = distance;

        }

        public ObservableCollection<string> Ports
        {
            get
            {
                return m_arduinoControl.PortList;
            }
        }

        public ObservableCollection<string> ErrorMessages { get; set; } = new ObservableCollection<string>();
        public string SelectedPort
        {
            get
            {
                if (string.IsNullOrEmpty(m_selected_port) && !m_arduinoControl.IsConnected)
                {
                    if (Ports.Count > 0)
                    {
                        m_arduinoControl.Connect(Ports[0]);
                        m_arduinoControl.SerialDataReceived += Port_DataReceived;
                        m_selected_port = Ports[0];
                        return Ports[0];
                    }
                    else
                        return "No port";
                }
                return m_selected_port;
            }
            set
            {
                //Set the new port
                m_selected_port = value;
                m_arduinoControl.Close();
                m_arduinoControl.Connect(m_selected_port);
                m_arduinoControl.SerialDataReceived += Port_DataReceived;
            }

        }

        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;



    }
}