﻿using Data;
using Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;

namespace UI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor[] m_motors;

        private string m_selected_port = string.Empty;

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
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
            int device = -1, status = -1, op = -1 , distance = -1;
            m_arduinoControl.ReciveCommand(ref device, ref op, ref status, ref distance);
            Process(device, op, status, distance);
        }

        public void Process(int device, int op, int status, int distance)
        {
            if(status > 0 )
            {
                //Error happened
            }


            if (op == (int)ArduinoFunctions.MOVE_FORWARD)
            {
                m_motors[device].Position -= distance;
            }
            else if (op == (int)ArduinoFunctions.MOVE_BACKWARD)
            {
                m_motors[device].Position -= distance;
            }
            else if (op == (int)ArduinoFunctions.HOME)
            {
                m_motors[device].Position -= distance;
            }
            else if (op == (int)ArduinoFunctions.STOP)
            {
                m_motors[device].Position -= distance;

            }
        }

        public ObservableCollection<string> Ports
        {
            get
            {
                return m_arduinoControl.PortList;
            }
        }

        public string SelectedPort
        {
            get
            {
                if (string.IsNullOrEmpty(m_selected_port))
                    return Ports.Count > 0 ? Ports[0] : "No port";
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}