using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            var ard = new ArduinoControl();
            ard.Connect();
        }
    }
}
