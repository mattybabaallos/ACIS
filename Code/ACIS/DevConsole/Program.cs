using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV;

namespace DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var sititcher = new ImageStitching();
           // sititcher.stitich();

            var barcode = new Barcode();
            barcode.ReadBarcode();
            //Console.ReadLine();
        }
    }
}
