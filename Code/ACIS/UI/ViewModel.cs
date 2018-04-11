using Data;
using Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
            //m_arduinoControl.Connect();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
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
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}