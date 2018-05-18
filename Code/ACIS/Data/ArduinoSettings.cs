using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ArduinoSettings : ApplicationSettingsBase
    {
        public ArduinoSettings() : base("ArduinoSettings")
        {
        }

        [UserScopedSetting()]
        [DefaultSettingValue("9600")]
        public int BaudRate
        {
            get
            {
                return ((int)(this["BaudRate"]));
            }
            set
            {
                this["BaudRate"] = value;
                OnPropertyChanged(this, "BaudRate");

            }
        }

        protected void OnPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected event PropertyChangedEventHandler PropertyChanged;
    }
}
