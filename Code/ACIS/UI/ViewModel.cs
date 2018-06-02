﻿using CV;
using Data;
using NLog;
using NLog.Targets;
using Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace UI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor[] m_motors;
        private bool cpu_done;
        private string m_imagePath;
        private CameraCapture cameraCapture;

        /**********Added for barcode*********/
        private Barcode m_barcode;
        private string m_decoded;

        /*********Added for Stitching************/
        private string m_StitchedPath;
        private ImageStitching m_stitcher;

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

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ViewModel(Home home)
        {
            m_home = home;
            m_waitHandle = new AutoResetEvent(false);
            cpu_done = false;
            m_scan_cancel = new CancellationTokenSource();
            m_arduinoControl = new ArduinoControl(m_waitHandle, m_scan_cancel);
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
            for (int i = 0; i < Constants.NUMBER_OF_MOTORS; ++i)
            {
                m_motors[i] = new Motor();
            }

            m_imagePath = "";
            cameraCapture = new CameraCapture();
            /**********Added for barcode*********/
            m_barcode = new Barcode();
            /*********Added for ImageStitching*********/
            m_stitcher = new ImageStitching();


        m_cpu_scanned = 0;
            m_y_axis_dividers_count = 0;
            m_progress = 0;
            m_cpu_done = false;
            UpdateLoggerPath();
            UsrSettings.PropertyChanged += UpdateLoggerPathEventHandler;
            DevSettingsProp.SettingChanging += ValidateDevSettings;
            BindingOperations.EnableCollectionSynchronization(ErrorMessages, m_lock); //This is needed to update the collection
            BindingOperations.EnableCollectionSynchronization(InfoMessages, m_lock);
            //ScannedCPUCollection.Add(new ScannedCPUInfo("Thao", "C:\\Users\\thaotran\\Desktop\\xsc_v1.0.6\\assets\\in.png", "C:\\Users\\thaotran\\Desktop\\xsc_v1.0.6\\assets"));
        }

        #region UiProprties 
        public List<Motors> MotorList{
            get { return Enum.GetValues(typeof(Motors)).Cast<Motors>().ToList(); }
        }
        public List<Functions> FunctionList
        {
            get { return Enum.GetValues(typeof(Functions)).Cast<Functions>().ToList(); }
        }
        public Motors SelectedMotor { get; set;}
        public Functions SelectedFunction { get; set; }
        public float Distance { get; set; }

        public DeviceSettings DevSettingsProp { get; } = new DeviceSettings();
        public UserSettings UsrSettings { get; } = new UserSettings();
        public ArduinoSettings ArdSettings { get; } = new ArduinoSettings();

        public ObservableCollection<ScannedCPUInfo> ScannedCPUCollection { get; set; } = new ObservableCollection<ScannedCPUInfo>();

        public ObservableCollection<string> ErrorMessages { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> InfoMessages { get; set; } = new ObservableCollection<string>();

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

        public string ImagePath
        {
            get { return m_imagePath; }
            set
            {
                m_imagePath = value;
                OnPropertyChanged(this, "ImagePath");
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

                LogError(Common.ErrorCodeToString(errorCode));
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
                LogError("Couldn't decode message from scanner");
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
                await Task.Run(
                () =>
                {
                    LogInfo("Start Scanning");
                    //Scan the first column
                    MoveToStartOfColumn(DevSettingsProp.DistanceFromHomeToTray, DevSettingsProp.DistanceFromHomeToTrayY);
                    cameraCapture.Init_camera(24 / DevSettingsProp.DistanceToMovePerImageY, Constants.CPU_WIDTH / DevSettingsProp.DistanceToMovePerImageX, UsrSettings.SavePath, "c" + CpuScanned.ToString()); //UsrSettings.SavePath
                    while (m_y_axis_dividers_count < DevSettingsProp.YaxisCpuDividers)
                    {
                        do
                        {
                            ScanRow(DevSettingsProp.DistanceFromHomeToTrayMiddleBar);
                            MoveStartOfRow(DevSettingsProp.DistanceFromStartOfTrayToMiddleBar, DevSettingsProp.DistanceToMovePerImageY);

                        } while (!m_cpu_done);
                        UpdateScanVariables();
                    }

                    m_y_axis_dividers_count = 0;

                    //Scan the second column
                    MoveToStartOfColumn(DevSettingsProp.DistanceFromHomeToTrayMiddleBar, DevSettingsProp.DistanceFromHomeToTrayY);
                    cameraCapture.Init_camera(24 / DevSettingsProp.DistanceToMovePerImageY, Constants.CPU_WIDTH / DevSettingsProp.DistanceToMovePerImageX, UsrSettings.SavePath, DateTime.Now.ToString());
                    while (m_y_axis_dividers_count < DevSettingsProp.YaxisCpuDividers)
                    {
                        do
                        {
                            ScanRow(DevSettingsProp.DistanceFromHomeToEndOfTray);
                            MoveStartOfRow(DevSettingsProp.DistanceFromMiddleBarToEndTray, DevSettingsProp.DistanceToMovePerImageY);

                        } while (!m_cpu_done);
                        UpdateScanVariables();
                    }

                });

                LogInfo("Done Scanning");
                _isScanable = true;
            }
            catch
            {
                if (m_scan_cancel.IsCancellationRequested)
                    CreateNewCancellationToken();
                LogError("Scan canceled");
                return;
            }

        }

        private void UpdateScanVariables()
        {
            cameraCapture.Init_camera(24 / DevSettingsProp.DistanceToMovePerImageY, Constants.CPU_WIDTH / DevSettingsProp.DistanceToMovePerImageX, UsrSettings.SavePath, DateTime.Now.ToString());
            var temp_img_list = m_stitcher.Load_images(UsrSettings.SavePath, 24 / DevSettingsProp.DistanceToMovePerImageY, Constants.CPU_WIDTH / DevSettingsProp.DistanceToMovePerImageX, "c" + CpuScanned.ToString());
            var temp_stitched = m_stitcher.Stitching_images(temp_img_list, 24 / DevSettingsProp.DistanceToMovePerImageY, Constants.CPU_WIDTH / DevSettingsProp.DistanceToMovePerImageX);
            m_decoded = m_barcode.Barcode_decoder(m_barcode.Find_barcode("Stitched_img_path"));

            //Create a folder for CPU: XML file + stitched image
            String cpu_folder_name = "ACIS" + "_" + m_decoded + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            String cpu_folder_path = Path.Combine(UsrSettings.SavePath, cpu_folder_name);
            String cpu_image_path = Path.Combine(cpu_folder_path, cpu_folder_name + ".jpg");
            Directory.CreateDirectory(cpu_folder_path);
            CreateXMLFile(cpu_folder_path, m_decoded);
            temp_stitched.Save(cpu_image_path);

            //Add to UI list
            ScannedCPUCollection.Add(new ScannedCPUInfo(m_decoded, cpu_image_path, cpu_folder_path)); 
  
            ++CpuScanned;
            m_cpu_done = false;
            Progress = ((float)CpuScanned / (float)DevSettingsProp.CpusToScan) * 100;
        }

        private void MoveStartOfRow(int x, int y)
        {

            //Step the X axis cameras back start of tray 
            m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.MoveStepperBackward, x);
            m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.MoveStepperBackward, x);

            //Move Y axis to next step
            m_arduinoControl.SendCommandBlocking(Devices.YAxisMotor, Functions.MoveStepperBackward, y);
        }

        private void ScanRow(int xPosition)
        {
            while (m_motors[(int)Devices.XAxisTopMotor].Position < xPosition) //Scan for one row 
            {
                cameraCapture.Take_picture();
                ImagePath = cameraCapture.FileName;          

                //Step the X axis camera to the next position
                m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.MoveStepperForward, DevSettingsProp.DistanceToMovePerImageX);
                m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.MoveStepperForward, DevSettingsProp.DistanceToMovePerImageX);
            }
        }

        private void MoveToStartOfColumn(int x, int y)
        {
            //Home the motors.
            HomeAll();

            //Move the X axis cameras to the begging of the tray
            m_arduinoControl.SendCommandBlocking(Devices.XAxisTopMotor, Functions.MoveStepperForward, x);
            m_arduinoControl.SendCommandBlocking(Devices.XAxisBottomMotor, Functions.MoveStepperForward, x);
            m_arduinoControl.SendCommandBlocking(Devices.YAxisMotor, Functions.MoveStepperForward, y);

        }

        private void OpenTrayAxis()
        {
            LogInfo("Open tray axis");
            m_arduinoControl.SendCommandBlocking(Devices.YAxisMotor, Functions.MoveStepperForward, 500);
        }

        private void Stop()
        {
            LogInfo("Stop Scanning");
            _isScanable = true;
            m_scan_cancel.Cancel();
            if (m_scan_cancel.IsCancellationRequested)
                LogInfo("Canceling Scan");
        }

        private void HomeAll()
        {
            LogInfo("Homing all");
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
            LogInfo("Open CPU image at " + path.ToString());
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

            if (cpuDialog.FileName.EndsWith(".jpg") | cpuDialog.FileName.EndsWith(".png")) //open image
            {
                ViewCPU(cpuDialog.FileName);
            }
            else //open XML file
            {
                StreamReader sr = new StreamReader(cpuDialog.FileName);
                MessageBox.Show(sr.ReadToEnd(), cpuDialog.FileName);
                sr.Close();
            }         
        }

        private void CreateXMLFile(String savePath, String CPUbarcode)
        {
            String fileName = "ACIS_" + CPUbarcode + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".xml";
            new XDocument(new XElement("ACIS", new XElement("barcode", CPUbarcode))).Save(Path.Combine(savePath, fileName));
        }
        private void HomeXTop()
        {
            LogInfo("Home X axis top");
            m_arduinoControl.SendCommand(Devices.XAxisTopMotor, Functions.HomeStepper, 0);
        }
        private void HomeXBottom()
        {
            LogInfo("Home X axix bottom");
            m_arduinoControl.SendCommand(Devices.XAxisBottomMotor, Functions.HomeStepper, 0);
        }
        private void HomeY()
        {
            LogInfo("Home Y axis");
            m_arduinoControl.SendCommand(Devices.YAxisMotor, Functions.HomeStepper, 0);
        }

        private void UpdateLoggerPath()
        {
            var logFileTaget = (FileTarget)LogManager.Configuration.AllTargets[0]; //Using index 0 because we only have one traget
            if (logFileTaget == null)
            {
                LogError("Could not update log file path. See the old path");
                return;
            }
            LogInfo("Update logger path to " + UsrSettings.SavePath + "\\log.txt");
            logFileTaget.FileName = UsrSettings.SavePath + "\\log.txt";
            LogManager.ReconfigExistingLoggers();
        }

        private void UpdateLoggerPathEventHandler(object sender, PropertyChangedEventArgs e)
        {
            UpdateLoggerPath();
        }

        private void ValidateDevSettings(object sender, SettingChangingEventArgs e)
        {
            if ((int)e.NewValue < 0)
            {     
                e.Cancel = true;
                MessageBox.Show("Negative Settings!");
                LogError("Negative settings");
            }
        }
            /// <summary>
            /// This function always run on the background and only update the COM list if there is any changes
            /// </summary>
            private async void updatePorts() {
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
            UsrSettings.SavePath = dialog.SelectedPath;
            LogInfo("Set save path to " + UsrSettings.SavePath);
        }
        private void Send()
        {
            m_arduinoControl.SendCommand((byte)SelectedMotor, (byte)SelectedFunction, (byte)Distance);
        }
        private void SaveDeviceSettings()
        {
            DevSettingsProp.Save();
            LogInfo("Saving device settings");
            int topLedColor = Int32.Parse(DevSettingsProp.TopLightsColor.Replace(@"#", ""), System.Globalization.NumberStyles.HexNumber);
            int bottomLedColor = Int32.Parse(DevSettingsProp.BottomLightsColor.Replace(@"#", ""), System.Globalization.NumberStyles.HexNumber);
            LogInfo("Turn on top LEDs with value " + DevSettingsProp.TopLightsColor); 
            LogInfo("Turn on bottom LEDs with value " + DevSettingsProp.BottomLightsColor);

            SendLEDCommand();
        }

        private void SendLEDCommand()
        {
            /***TO_DOUBLE_CHECK***/
            //m_arduinoControl.SendCommandBlocking(Devices.TopLeds, Functions.TurnOnUpdateLeds, topLedColor);
            //m_arduinoControl.SendCommandBlocking(Devices.BottomLeds, Functions.TurnOnUpdateLeds, bottomLedColor);
        }
        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int device = -1, status = -1, fucntion = -1, data = -1, errorCode = -1;
            m_arduinoControl.ReciveCommand(ref device, ref fucntion, ref status, ref data, ref errorCode);
            Process(device, fucntion, status, data);
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
                    LogError("X TOP motor moved to location " + distance);
                    break;
                case (int)Devices.XAxisBottomMotor:
                    OnPropertyChanged(this, "XBottomPosition");
                    LogError("X Bottom motor moved to location " + distance);

                    break;
                case (int)Devices.YAxisMotor:
                    OnPropertyChanged(this, "YPosition");
                    LogError("Y motor moved to location " + distance);

                    break;
                default:
                    break;
            }
        }

        private void LogInfo(string message)
        {
            InfoMessages.Add(message);
            logger.Info(message);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                m_home.InfoMessageScrollViewer.ScrollToBottom();
            }));
        }

        private void LogError(string message)
        {
            ErrorMessages.Add(message);
            logger.Error(message);
            //This will spin up a thread that will update the UI
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                m_home.ErrorMessageScrollViewer.ScrollToBottom();
            }));
        }

        protected void OnPropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ICommands
        //ICommands
        public ICommand HomeAllCommand { get { return new Command(e => true, HomeAll); } }
        public ICommand HomeXTopCommand { get { return new Command(e => true, HomeXTop); } }
        public ICommand HomeXBottomCommand { get { return new Command(e => true, HomeXBottom); } }
        public ICommand HomeYCommand { get { return new Command(e => true, HomeY); } }
        public ICommand StartScan { get { return new Command(e => _isScanable, Scan); } }
        public ICommand StopScan { get { return new Command(e => !_isScanable, Stop); } }
        public ICommand OpenTray { get { return new Command(e => true, OpenTrayAxis); } }
        public ICommand BrowseCommand { get { return new Command(e => true, Browse); } }
        public ICommand ViewCPUCommand { get { return new ParameterCommand(e => true, path => ViewCPU(path)); } }
        public ICommand BrowseCPUFolderCommand { get { return new ParameterCommand(e => true, path => BrowseCPUFolder(path)); } }
        public ICommand SaveDevSettingsCommand { get { return new Command(e => true, SaveDeviceSettings); } }
        public ICommand RestorDeveSettingsCommand { get { return new Command(e => true, delegate{DevSettingsProp.Reset(); LogInfo("Save user settings"); }); } }
        public ICommand SaveUserSettingsCommand { get { return new Command(e => true, delegate{UsrSettings.Save(); LogInfo("Save user settings"); }); } }
        public ICommand RestorUserSettingsCommand { get { return new Command(e => true, delegate{UsrSettings.Reset(); LogInfo("Reset user settings"); }); } }
        public ICommand SaveArduinoSettingsCommand { get { return new Command(e => true, delegate{ArdSettings.Save(); LogInfo("Save Arduino settings"); }); } }
        public ICommand RestorArdinoSettingsCommand { get { return new Command(e => true, delegate{ArdSettings.Reset(); LogInfo("Reset Arduino settings"); }); } }
        public ICommand SendCommand { get { return new Command(e => true, Send); } }
        #endregion
    }
}