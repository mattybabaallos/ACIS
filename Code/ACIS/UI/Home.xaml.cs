using NLog;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget() { FileName = "logs.txt", Name = "logfile" };

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Error, logfile));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Info, logfile));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, logfile));

            LogManager.Configuration = config;
            DataContext = new ViewModel(this);



        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
