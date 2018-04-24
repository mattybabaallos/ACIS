﻿using Data;
using Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace UI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor[] m_motors;

        /***********Added for camrea************/
        private CameraControl m_camera;
        private string m_saveFolder;
        private string m_imagePath;
        /***************************************/

        private string m_selected_port = string.Empty;
        private object _lock = new object();
        private bool m_updateUI = false;
        private event EventHandler ScrollView;

        private int m_cpu_scanned;
        private int m_total_cpu_scanned;
        private int m_y_axis_dividers_count;
        private float m_progress;

        private CancellationTokenSource m_scan_cancel;
        private AutoResetEvent m_waitHandle = new AutoResetEvent(false);

        public ICommand HomeAllCommand { get { return new Command(e => true, this.HomeAll); } }
        public ICommand HomeXTopCommand { get { return new Command(e => true, this.HomeXTop); } }
        public ICommand HomeXBottomCommand { get { return new Command(e => true, this.HomeXBottom); } }
        public ICommand HomeYCommand { get { return new Command(e => true, this.HomeY); } }
        public ICommand CaptureCommand { get { return new Command(e => true, this.CaptureCPU); } }

        public ICommand StartScan { get { return new Command(e => true, this.Scan); } }
        public ICommand StopScan { get { return new Command(e => true, this.Stop); } }



        //private List<string> m_error = new List<string>();

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl(m_waitHandle);
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_motors[i] = new Motor();
            }
            /***********Added for camrea************/
            m_camera = new CameraControl();
            m_camera.Videocapture.ImageGrabbed += SaveImage;
            m_saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACIS");
            if (!Directory.Exists(m_saveFolder))
                Directory.CreateDirectory(m_saveFolder);
            m_imagePath = "";
            /***************************************/
            m_cpu_scanned = 0;
            m_total_cpu_scanned = 0;
            m_y_axis_dividers_count = 0;
            m_progress = 0;
            BindingOperations.EnableCollectionSynchronization(ErrorMessages, _lock); //This is needed to update the collection

            m_scan_cancel = new CancellationTokenSource();
            //HomeAllButton = new HomeCommand(this);
            //updatePorts();
        }

        /***********Added for camrea************/
        private void CaptureCPU()
        {
            m_camera.Capture();
        }
        private void SaveImage(object sender, EventArgs e)
        {
            try
            {
                m_camera.Videocapture.Retrieve(m_camera.Frame);
                m_camera.Videocapture.Stop();
                string path = m_saveFolder + "\\" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".jpg";
                m_camera.Frame.Save(path);
                ImagePath = path;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string ImagePath
        {
            get { return m_imagePath; }
            set
            {
                m_imagePath = value;
                OnPropertyChanged(this, "ImagePath");
            }
        }
        public string SaveFolder
        {
            get { return m_saveFolder; }
            set
            {
                m_saveFolder = value;
                if (!Directory.Exists(m_saveFolder))
                    Directory.CreateDirectory(m_saveFolder);
                OnPropertyChanged(this, "SaveFolder");
            }
        }
        /***************************************/
        public int CPU_Scanned
        {
            get { return m_cpu_scanned; }
            set
            {
                m_cpu_scanned = value;
                OnPropertyChanged(this, "CPU_Scanned");
            }
        }

        private void HomeAll()
        {
            ErrorMessages.Add("Homing all");
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_arduinoControl.SendCommandBlocking((byte)i, (byte)ArduinoFunctions.HOME, 0);
            }

        }

        private void HomeXTop()
        {
            m_arduinoControl.SendCommand(Motors.X_AXIS_TOP, ArduinoFunctions.HOME, 0);
        }
        private void HomeXBottom()
        {
            m_arduinoControl.SendCommand(Motors.X_AXIS_BOTTOM, ArduinoFunctions.HOME, 0);
        }

        public void HomeY()
        {
            m_arduinoControl.SendCommand(Motors.Y_AXIS, ArduinoFunctions.HOME, 0);
        }

        /// <summary>
        /// This function always run on the background and only update the COM list if there is any changes
        /// </summary>
        private async void updatePorts()
        {
            ObservableCollection<string> CurrentPorts;
            while (true)
            {
                await Task.Delay(2000);
                await Task.Run(() =>
                {
                    CurrentPorts = new ObservableCollection<string>(SerialPort.GetPortNames());

                    //only update port list when it changes
                    if (Ports.SequenceEqual(CurrentPorts) != true)
                    {
                        Ports = new ObservableCollection<string>(CurrentPorts);

                        if (Ports.Count == 0)
                            IsPortConnected = false;
                        else
                            IsPortConnected = true;
                    }
                    //++CPU_Scanned;
                    //Progress = ((float)CPU_Scanned / (float)Constants.CPU_TO_SCAN) * 100;
                    //Console.WriteLine(CPU_Scanned);
                    //Console.WriteLine(Progress);
                });
            }
        }

        public bool IsPortConnected
        {
            get { return true; }
            set
            {
                m_updateUI = value;
                OnPropertyChanged(this, "IsPortConnected");
            }
        }

        public float Progress
        {
            get
            {
                return m_progress;
            }
            set
            {
                m_progress = value;
                OnPropertyChanged(this, "Progress");
            }
        }

        public void Stop()
        {
            m_scan_cancel.Cancel();
            if (m_scan_cancel.IsCancellationRequested)
                ErrorMessages.Add("canceling");
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

        public async void Scan()
        {
            var task = new Task(
            () =>{
                //Home the motors.
                HomeAll();
          
                //Move the X axis cameras to the begging of the tray
                m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY);
                m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY);
                while (m_y_axis_dividers_count < Constants.Y_AXIS_DIVIDERS)
                {
                    do
                    {
                        while (m_motors[(int)Motors.X_AXIS_TOP].Position < Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR) //Scan for one row 
                        {
                            //Take Pictures here......TODO

                            //Step the X axis camera to the next position
                            m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                            m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                        }

                        //Step the X axis cameras back start of tray 
                        m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FROM_START_OF_TRAY_TO_MIDDLE_BAR);
                        m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY);

                        m_arduinoControl.SendCommandBlocking(Motors.Y_AXIS, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y);

                    } while (!m_motors[(int)Motors.Y_AXIS].Stopped);
                    ++CPU_Scanned;
                    Progress = ((float)CPU_Scanned / (float)Constants.CPU_TO_SCAN) * 100;
                }

                HomeAll();
                m_y_axis_dividers_count = 0;


                m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR);
                m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR);

                while (m_y_axis_dividers_count < Constants.Y_AXIS_DIVIDERS)
                {
                    do
                    {
                        while (m_motors[(int)Motors.X_AXIS_TOP].Position < Constants.DISTANCE_FROM_HOME_TO_END_OF_TRAY) //Scan for one row 
                        {
                            //Take Pictures here......TODO

                            //Step the X axis camera to the next position
                            m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                            m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                        }

                        //Step the X axis cameras back start of tray 
                        m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_TOP, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FRPM_MIDDLE_BAR_TO_END_TRAY);
                        m_arduinoControl.SendCommandBlocking(Motors.X_AXIS_BOTTOM, ArduinoFunctions.MOVE_BACKWARD, Constants.DISTANCE_FRPM_MIDDLE_BAR_TO_END_TRAY);

                        m_arduinoControl.SendCommandBlocking(Motors.Y_AXIS, ArduinoFunctions.MOVE_FORWARD, Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y);

                    } while (!m_motors[(int)Motors.Y_AXIS].Stopped);
                    ++CPU_Scanned;
                    Progress = ((float)CPU_Scanned / (float)Constants.CPU_TO_SCAN) * 100;
                }
                
            }, m_scan_cancel.Token);
            task.Start();
            await task;
           
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

            m_waitHandle.Set();

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
            set
            {
                m_arduinoControl.PortList = new ObservableCollection<string>(value);
                OnPropertyChanged(this, "Ports");

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
                OnPropertyChanged(this, "SelectedPort");

            }

        }




        //ICommands
        public ICommand HomeAllButton { get; set; }



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