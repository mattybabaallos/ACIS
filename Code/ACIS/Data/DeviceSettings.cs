using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;

namespace Data
{
    public class DeviceSettings : ApplicationSettingsBase
    {
        public DeviceSettings(): base("DeviceSettings")
        {

        }

        [UserScopedSetting()]
        [DefaultSettingValue("55")]
        public int DistanceFromStartOfTrayToMiddleBar
        {
            get
            {
                return ((int)(this["DistanceFromStartOfTrayToMiddleBar"]));
            }
            set
            {
                this["DistanceFromStartOfTrayToMiddleBar"] = value;
                OnPropertyChanged(this, "DistanceFromStartOfTrayToMiddleBar");
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("80")]
        public int DistanceFromHomeToTray
        {
            get
            {
                return ((int)(this["DistanceFromHomeToTray"]));
            }
            set
            {
                this["DistanceFromHomeToTray"] = value;
                OnPropertyChanged(this, "DistanceFromHomeToTray");

            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("142")]
        public int DistanceFromHomeToTrayMiddleBar
        {
            get
            {
                return ((int)(this["DistanceFromHomeToTrayMiddleBar"]));
            }
            set
            {
                this["DistanceFromHomeToTrayMiddleBar"] = value;
                OnPropertyChanged(this, "DistanceFromMiddleBarToEndTray");

            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("60")]
        public int DistanceFromMiddleBarToEndTray
        {
            get
            {
                return ((int)(this["DistanceFromMiddleBarToEndTray"]));
            }
            set
            {
                this["DistanceFromMiddleBarToEndTray"] = value;
                OnPropertyChanged(this, "DistanceFromMiddleBarToEndTray");


            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("230")]
        public int DistanceFromHomeToEndOfTray
        {
            get
            {
                return ((int)(this["DistanceFromHomeToEndOfTray"]));
            }
            set
            {
                this["DistanceFromHomeToEndOfTray"] = value;
                OnPropertyChanged(this, "DistanceFromHomeToEndOfTray");

            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("140")]
        public int DistanceFromHomeToTrayY
        {
            get
            {
                return ((int)(this["DistanceFromHomeToTrayY"]));
            }
            set
            {
                this["DistanceFromHomeToTrayY"] = value;
                OnPropertyChanged(this, "DistanceFromHomeToTrayY");

            }
        }


        [UserScopedSetting()]
        [DefaultSettingValue("10")]
        public int DistanceToMovePerImageX
        {
            get
            {
                return ((int)(this["DistanceToMovePerImageX"]));
            }
            set
            {
                this["DistanceToMovePerImageX"] = value;
                OnPropertyChanged(this, "DistanceToMovePerImageX");

            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("10")]
        public int DistanceToMovePerImageY
        {
            get
            {
                return ((int)(this["DistanceToMovePerImageY"]));
            }
            set
            {
                this["DistanceToMovePerImageY"] = value;
                OnPropertyChanged(this, "DistanceToMovePerImageY");

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
