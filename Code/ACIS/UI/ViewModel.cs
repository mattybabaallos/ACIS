using Data;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace UI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor[] m_motors;

        private string m_selected_port = string.Empty;
        private object _lock = new object();
        private bool m_updateUI = false;
        private event EventHandler ScrollView;

        //private List<string> m_error = new List<string>();

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_motors[i] = new Motor();
            }
            BindingOperations.EnableCollectionSynchronization(ErrorMessages, _lock); //This is needed to update the collection
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
            int device = -1, status = -1, op = -1, distance = -1;
            m_arduinoControl.ReciveCommand(ref device, ref op, ref status, ref distance);
            Process(device, op, status, distance);
            ErrorMessages.Add(DateTime.Now.Second.ToString());
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                m_home.ScrollViewer.ScrollToBottom();
            }));
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
            {
                m_motors[device].Position = distance;
                OnPropertyChanged(this, "XTopPosition");
            }


        }
        public ObservableCollection<string> Ports
        {
            get
            {
                return m_arduinoControl.PortList;
            }
        }
        public ObservableCollection<string> ErrorMessages { get; set; } = new ObservableCollection<string>();
        public int XTopPosition
        {
            get { return m_motors[(int)Motors.X_AXIS_TOP].Position; }
            set { }
        }
        public int XBottomPosition
        {
            get { return m_motors[(int)Motors.X_AXIS_BOTTOM].Position; }
            set { }
        }
        public int ZTopPosition
        {
            get { return m_motors[(int)Motors.Z_AXIS_TOP].Position; }
            set { }
        }
        public int ZBottomPosition
        {
            get { return m_motors[(int)Motors.Z_AXIS_BOTTOM].Position; }
            set { }
        }
        public int YPosition
        {
            get { return m_motors[(int)Motors.Y_AXIS].Position; }
            set { }
        }
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