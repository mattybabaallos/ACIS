using Data;
using Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
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
        private bool m_updateUI = false;
        private event EventHandler ScrollView;

        private int m_cpu_scanned;
        private int m_total_cpu_scanned;
        private int m_y_axis_dividers_count;

        //private List<string> m_error = new List<string>();

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_motors[i] = new Motor();
                m_motors[i].Position = 44;
            }
            m_cpu_scanned = 0;
            m_total_cpu_scanned = 0;
            m_y_axis_dividers_count = 0;
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

            //This will spin up a thread that will update the UI
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                m_home.ScrollViewer.ScrollToBottom();
            }));
        }

        public void Scan()
        {
            //Home the motors.
            HomeAll();


            //Move the X axis cameras to the begging of the tray
            m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY);
            m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY);

            while (m_y_axis_dividers_count < Constants.Y_AXIS_DIVIDERS)
            {
                do
                {
                    while (m_motors[(int)Motors.X_AXIS_TOP].Position < Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR) //Scan for one row 
                    {
                        //Take Pictures here......TODO

                        //Step the X axis camera to the next position
                        m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                        m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                    }

                    //Step the X axis cameras back start of tray 
                    m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FROM_START_OF_TRAY_TO_MIDDLE_BAR);
                    m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY);

                    m_arduinoControl.SendCommand(Motors.Y_AXIS, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y);

                } while (!m_motors[(int)Motors.Y_AXIS].Stopped);
                ++m_cpu_scanned;
            }

            HomeAll();
            m_y_axis_dividers_count = 0;


            m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR);
            m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR);

            while (m_y_axis_dividers_count < Constants.Y_AXIS_DIVIDERS)
            {
                do
                {
                    while (m_motors[(int)Motors.X_AXIS_TOP].Position < Constants.DISTANCE_FROM_HOME_TO_END_OF_TRAY) //Scan for one row 
                    {
                        //Take Pictures here......TODO

                        //Step the X axis camera to the next position
                        m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                        m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                    }

                    //Step the X axis cameras back start of tray 
                    m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FRPM_MIDDLE_BAR_TO_END_TRAY);
                    m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FRPM_MIDDLE_BAR_TO_END_TRAY);

                    m_arduinoControl.SendCommand(Motors.Y_AXIS, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y);

                } while (!m_motors[(int)Motors.Y_AXIS].Stopped);
                ++m_cpu_scanned;
            }

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
                UpdateMotorsUiElements(device, distance);
                return;
            }

            if (device == (int)Motors.Y_AXIS)
                ++m_y_axis_dividers_count;
            m_motors[device].Stopped = true;
        }

        private void UpdateMotorsUiElements(int device, int distance)
        {
            switch (device)
            {
                case (int)Motors.X_AXIS_TOP:
                    OnPropertyChanged(this, "XTopPosition");
                    ErrorMessages.Add("X TOP motor moved to loaction" + distance);
                    break;
                case (int)Motors.X_AXIS_BOTTOM:
                    OnPropertyChanged(this, "XBottomPosition");
                    break;
                case (int)Motors.Y_AXIS:
                    OnPropertyChanged(this, "YPosition");
                    break;
                case (int)Motors.Z_AXIS_TOP:
                    OnPropertyChanged(this, "ZTopPosition");
                    break;
                case (int)Motors.Z_AXIS_BOTTOM:
                    OnPropertyChanged(this, "ZBottomPosition");
                    break;


                default:
                    break;
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