using Data;
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
using CV;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor[] m_motors;

        /***********Added for camera************/
        private CameraControl m_camera;
        private string m_saveFolder;
        private string m_imagePath;
        private CameraCapture cameraCapture;

        private string m_selected_port = string.Empty;
        private object m_lock = new object();
        private bool m_updateUI = false;
        private bool _isScanable = true;

        private int m_cpu_scanned;
        private int m_y_axis_dividers_count;
        private float m_progress;
        private bool m_cpu_done;

        private CancellationTokenSource m_scan_cancel;
        private AutoResetEvent m_waitHandle;



        public ViewModel(Home home)
        {
            m_home = home;
            m_waitHandle = new AutoResetEvent(false);
            m_scan_cancel = new CancellationTokenSource();
            m_arduinoControl = new ArduinoControl(m_waitHandle, m_scan_cancel);
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_motors[i] = new Motor();
            }
            /***********Added for camera************/
            m_camera = new CameraControl();
            m_camera.Videocapture.ImageGrabbed += SaveImage;
            SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACIS");
            m_imagePath = "";

            XmlDocument xmlDoc = new XmlDocument(); 
            xmlDoc.Load("Setting.xml");
            XmlNodeList savePathNode = xmlDoc.GetElementsByTagName("savePath");

            if (!String.IsNullOrEmpty(savePathNode[0].InnerText))
                m_saveFolder = savePathNode[0].InnerText;
            else
                m_saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACIS");

            //validate the savePath
            try
            {
                if (!Directory.Exists(m_saveFolder))
                    Directory.CreateDirectory(m_saveFolder);
            }
            catch 
            {
                ErrorMessages.Add("Illegal save path");
            }

            m_imagePath = "";
            cameraCapture = new CameraCapture();

            m_cpu_scanned = 0;
            m_y_axis_dividers_count = 0;
            m_progress = 0;
            m_cpu_done = false;
            BindingOperations.EnableCollectionSynchronization(ErrorMessages, m_lock); //This is needed to update the collection
        }


        #region UiProprties 
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
                if (!String.IsNullOrEmpty(value))
                {
                    m_saveFolder = value;
                    if (!Directory.Exists(m_saveFolder))
                        Directory.CreateDirectory(m_saveFolder);
                    OnPropertyChanged(this, "SaveFolder");
                }

            }
        }

        public int CpuScanned
        {
            get { return m_cpu_scanned; }
            set
            {
                m_cpu_scanned = value;
                OnPropertyChanged(this, "CPU_Scanned");
            }
        }

        public ObservableCollection<ScannedCPUInfo> ScannedCPUCollection { get; set; } = new ObservableCollection<ScannedCPUInfo>();

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

        public void Process(int device, int function, int errorCode, int data)
        {
            if (errorCode > 0)
            {
                /*Error happened
                 * maybe print the error to the GUI and retry the command again. 
                 * Possible keep track if the errors, if it happens multiple times ask the user to perform a reboot
                 * or look into the issue.
                 * 
                */


            }

            //This is a respond to command sent from the main application
            if (function != (int)Functions.StopStepper && device < Constants.NUMBER_OF_MOTORS)
            {
                m_motors[device].Position = data;
                UpdateMotorsUiElements(device, data);
                m_waitHandle.Set();
            }

            else if (device == (int)Devices.YAxisCpuSwitch && function == (int)Functions.StopStepper && errorCode == (int)Errors.StopInterrupt)
            {
                ++m_y_axis_dividers_count;
                m_cpu_done = true;

            }

            else if (function == (int)Functions.StopStepper && device < Constants.NUMBER_OF_MOTORS)
                m_motors[device].Stopped = true;
            else
                ErrorMessages.Add("Couldn't decode message from scanner");
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

        public string SelectedPort
        {
            get
            {
                if (string.IsNullOrEmpty(m_selected_port) && !m_arduinoControl.IsConnected)
                {
                    if (Ports.Count > 0)
                    {
                        m_arduinoControl.Connect(Ports[0]);
                        m_arduinoControl.SerialDataReceived += PortDataReceived;
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
                m_arduinoControl.SerialDataReceived += PortDataReceived;
                OnPropertyChanged(this, "SelectedPort");

            }

        }

        public int XTopPosition
        {
            get { return m_motors[(int)Devices.XAxisTopMotor].Position; }
            set { }
        }
        public int XBottomPosition
        {
            get { return m_motors[(int)Devices.XAxisBottomMotor].Position; }
            set { }
        }
        public int YPosition
        {
            get { return m_motors[(int)Devices.YAxisMotor].Position; }
            set { }
        }

        #endregion

        #region Privates

        private async void Scan()
        {
            try
            {
                _isScanable = false;
                if (m_scan_cancel.IsCancellationRequested)
                    CreateNewCancellationToken();

                await Task.Run(
                () =>
                {
                    //Scan the first column
                    MoveToStartOfColumn(Constants.DISTANCE_FROM_HOME_TO_TRAY, Constants.DISTANCE_FROM_HOME_TO_TRAY_Y);
                    cameraCapture.Init_camera(24 / Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y, Constants.CPU_WIDTH / Constants.DISTANCE_TO_MOVE_PER_IMAGE_X, SaveFolder, CpuScanned.ToString());
                    while (m_y_axis_dividers_count < Constants.Y_AXIS_DIVIDERS)
                    {
                        do
                        {
                            ScanRow(Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR);
                            MoveStartOfRow(Constants.DISTANCE_FROM_START_OF_TRAY_TO_MIDDLE_BAR, Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y);

                        } while (!m_cpu_done);
                        UpdateScanVariables();
                    }

                    m_y_axis_dividers_count = 0;

                    //Scan the second column
                    MoveToStartOfColumn(Constants.DISTANCE_FROM_HOME_TO_TRAY_MIDDLE_BAR, Constants.DISTANCE_FROM_HOME_TO_TRAY_Y);
                    cameraCapture.Init_camera(24 / Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y, Constants.CPU_WIDTH / Constants.DISTANCE_TO_MOVE_PER_IMAGE_X, SaveFolder, DateTime.Now.ToString());
                    while (m_y_axis_dividers_count < Constants.Y_AXIS_DIVIDERS)
                    {
                        do
                        {
                            ScanRow(Constants.DISTANCE_FROM_HOME_TO_END_OF_TRAY);
                            MoveStartOfRow(Constants.DISTANCE_FRPM_MIDDLE_BAR_TO_END_TRAY, Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y);

                        } while (!m_cpu_done);
                        UpdateScanVariables();
                    }

                });

                _isScanable = true;
            }
            catch
            {
                ErrorMessages.Add("Scan canceled");
                return;
            }

        }

        private void UpdateScanVariables()
        {
            cameraCapture.Init_camera(24 / Constants.DISTANCE_TO_MOVE_PER_IMAGE_Y, Constants.CPU_WIDTH / Constants.DISTANCE_TO_MOVE_PER_IMAGE_X, SaveFolder, DateTime.Now.ToString());
            ++CpuScanned;
            m_cpu_done = false;
            Progress = ((float)CpuScanned / (float)Constants.CPU_TO_SCAN) * 100;
        }

        private void MoveStartOfRow(int x, int y)
        {

            //Step the X axis cameras back start of tray 
            m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.MoveStepperBackward, (byte)x);
            m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.MoveStepperBackward, (byte)x);

            //Move Y axis to next step
            m_arduinoControl.SendCommandBlocking(Devices.YAxisMotor, Functions.MoveStepperBackward, (byte)y);
        }

        private void ScanRow(int xPosition)
        {
            while (m_motors[(int)Devices.XAxisTopMotor].Position < xPosition) //Scan for one row 
            {
                cameraCapture.Take_picture();
                ImagePath = cameraCapture.FileName;

                /**TODO*** 
                ScannedCPUCollection.Add(new ScannedCPUInfo(***CPU barcode here***, ***CPU Image Path here***, ***CPU Folder here***)); 
                **********/

                //Step the X axis camera to the next position
                m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.MoveStepperForward, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
                m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.MoveStepperForward, Constants.DISTANCE_TO_MOVE_PER_IMAGE_X);
            }
        }

        private void MoveToStartOfColumn(int x, int y)
        {
            //Home the motors.
            HomeAll();

            //Move the X axis cameras to the begging of the tray
            m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.MoveStepperForward, (byte)x);
            m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.MoveStepperForward, (byte)x);
            m_arduinoControl.SendCommandBlocking(Devices.YAxisMotor, Functions.MoveStepperForward, (byte)y);

        }

        private void Stop()
        {
            m_scan_cancel.Cancel();
            if (m_scan_cancel.IsCancellationRequested)
                ErrorMessages.Add("canceling");
        }

        private void HomeAll()
        {
            ErrorMessages.Add("Homing all");
            //for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            //{
            //    m_arduinoControl.SendCommandBlocking((byte)i, (byte)ArduinoFunctions.HOME, 0);
            //}
            m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.HomeStepper, 0);
            m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.HomeStepper, 0);
            m_arduinoControl.SendCommandBlocking(Devices.YAxisMotor, Functions.HomeStepper, 0);

        }

        private void ViewCPU(object path)
        {
            Window imgWindow = new Window();
            imgWindow.Height = 300;
            imgWindow.Width = 300;
            imgWindow.Title = path.ToString();
            BitmapImage btm = new BitmapImage(new Uri(path.ToString(), UriKind.Relative));
            Image img = new Image();
            img.Source = btm;
            imgWindow.Content = img;
            imgWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            imgWindow.Show();
        }
        private void BrowseCPUFolder(object path)
        {
            System.Windows.Forms.OpenFileDialog cpuDialog = new System.Windows.Forms.OpenFileDialog();
            cpuDialog.Title = path.ToString();
            cpuDialog.InitialDirectory = path.ToString();
            cpuDialog.ShowDialog();
        }


        private void HomeXTop()
        {
            m_arduinoControl.SendCommand(Devices.XAxisTopMotor, Functions.HomeStepper, 0);
        }
        private void HomeXBottom()
        {
            m_arduinoControl.SendCommand(Devices.XAxisBottomMotor, Functions.HomeStepper, 0);
        }
        private void HomeY()
        {
            m_arduinoControl.SendCommand(Devices.YAxisMotor, Functions.HomeStepper, 0);
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
                });
            }
        }

        private void Browse()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            SaveFolder = dialog.SelectedPath;
        }
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

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int device = -1, status = -1, fucntion = -1, data = -1, errorCode = -1;
            m_arduinoControl.ReciveCommand(ref device, ref fucntion, ref status, ref data,ref errorCode);
            Process(device, fucntion, status, data);

            //This will spin up a thread that will update the UI
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                m_home.ScrollViewer.ScrollToBottom();
            }));
        }

        private void CreateNewCancellationToken()
        {
            m_scan_cancel = new CancellationTokenSource();
            m_arduinoControl.Cancellation = m_scan_cancel;
        }

        private void UpdateMotorsUiElements(int device, int distance)
        {
            switch (device)
            {
                case (int)Devices.XAxisTopMotor:
                    OnPropertyChanged(this, "XTopPosition");
                    ErrorMessages.Add("X TOP motor moved to location " + distance);
                    break;
                case (int)Devices.XAxisBottomMotor:
                    OnPropertyChanged(this, "XBottomPosition");
                    ErrorMessages.Add("X Bottom motor moved to location " + distance);

                    break;
                case (int)Devices.YAxisMotor:
                    OnPropertyChanged(this, "YPosition");
                    ErrorMessages.Add("Y motor moved to location " + distance);

                    break;
                default:
                    break;
            }
        }

        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ICommands

        //ICommands
        public ICommand HomeAllCommand { get { return new Command(e => true, this.HomeAll); } }
        public ICommand HomeXTopCommand { get { return new Command(e => true, this.HomeXTop); } }
        public ICommand HomeXBottomCommand { get { return new Command(e => true, this.HomeXBottom); } }
        public ICommand HomeYCommand { get { return new Command(e => true, this.HomeY); } }
        public ICommand CaptureCommand { get { return new Command(e => true, this.CaptureCPU); } }

        public ICommand StartScan { get { return new Command(e => true, this.Scan); } }
        public ICommand StopScan { get { return new Command(e => true, this.Stop); } }
        public ICommand BrowseCommand { get { return new Command(e => true, this.Browse); } }

        public ICommand ViewCPUCommand { get { return new ParameterCommand(e => true, path => this.ViewCPU(path)); } }
        public ICommand BrowseCPUFolderCommand { get { return new ParameterCommand(e => true, path => this.BrowseCPUFolder(path)); } }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;



    }
}