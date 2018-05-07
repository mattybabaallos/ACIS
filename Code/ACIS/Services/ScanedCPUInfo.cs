using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace Services
{
    public class ScannedCPUInfo
    {
        private String cpu_barcode;
        private String cpu_imagePath;
        private String cpu_folderPath;

        public ScannedCPUInfo(String barcode, String imagePath, String folderPath)
        {
            CPUBarcode = barcode;
            CPUImagePath = imagePath;
            CPUFolderPath = folderPath;
    }

        public String CPUBarcode
        {
            get { return cpu_barcode; }
            set { cpu_barcode = value; }
        }

        public String CPUImagePath
        {
            get { return cpu_imagePath; }
            set
            { cpu_imagePath = value; }
        }

        public String CPUFolderPath
        {
            get { return cpu_folderPath; }
            set { cpu_folderPath = value; }
        }
    }
}
