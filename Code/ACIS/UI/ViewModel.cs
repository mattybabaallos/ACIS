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

        public ViewModel(Home home)
        {
            m_home = home;
            m_arduinoControl = new ArduinoControl();
            m_arduinoControl.Connect();
            m_motors = new Motor[Constants.NUMBER_OF_MOTORS];
        }
    }
}