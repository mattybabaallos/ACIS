using Data;
using Services;
using System.Collections.Generic;

namespace UI
{
    internal class ViewModel
    {
        private Home m_home;
        private ArduinoControl m_arduinoControl;
        private Motor [] m_motors;

        private string m_selected_port = string.Empty;

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl();
            //m_arduinoControl.Connect();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
        }


        public List<string> Ports
        {
            get
            {
                var portList = new List<string>() { "no ports" };

                return m_arduinoControl.PortList.Count > 0 ? m_arduinoControl.PortList : portList;
            }
        }

        public string SelectedPort
        {
            get
            {
                var serialString = "Serial port: ";
                return string.IsNullOrEmpty(m_selected_port)? serialString + Ports[0] : serialString + m_selected_port;
            }
            set
            {
                //Set the new port
                m_selected_port = value;
                m_arduinoControl.Close();
                m_arduinoControl.Connect(m_selected_port);
            }
            
        }
            
           
    }
}