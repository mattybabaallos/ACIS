using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UserSettings : ApplicationSettingsBase
    {
        public UserSettings() : base("UserSettings")
        {
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public String SavePath
        {
            get
            {
                var value = (String)(this["SavePath"]);

                if (String.IsNullOrEmpty(value))
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACIS");
                return value;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    this["SavePath"] = value;
                    if (!Directory.Exists(SavePath))
                        Directory.CreateDirectory(SavePath);
                    OnPropertyChanged(this, "SavePath");
                }
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
