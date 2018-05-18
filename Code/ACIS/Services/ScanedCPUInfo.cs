using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace Services
{
    public class ScannedCPUInfo
    {
        public ScannedCPUInfo(String barcode, String imagePath, String folderPath)
        {
            CPUBarcode = barcode;
            CPUImagePath = imagePath;
            CPUFolderPath = folderPath;
    }

        public String CPUBarcode { get; set; }

        public String CPUImagePath { get; set; }

        public String CPUFolderPath { get; set; }
    }
}
