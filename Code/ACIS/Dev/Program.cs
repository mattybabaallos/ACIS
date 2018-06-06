using Emgu.CV;
using Emgu.CV.ML;
using Services;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;

namespace Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            var cam1T = new Thread(() => {
                var frame1 = new Mat();
                var cam1 = new VideoCapture(0);
                cam1.Start();
                while (true)
                {
                    cam1.Read(frame1);
                    CvInvoke.Imshow("Cam1", frame1);
                    CvInvoke.WaitKey(1);
                }
                CvInvoke.DestroyAllWindows();
            });

            var cam2T = new Thread(() => {
                var frame1 = new Mat();
                var cam1 = new VideoCapture(1);
                cam1.Start();
                while (true)
                {
                    cam1.Read(frame1);
                    CvInvoke.Imshow("Cam2", frame1);
                    CvInvoke.WaitKey(1);
                }
                CvInvoke.DestroyAllWindows();
            });

            var cam3T = new Thread(() => {
                var frame1 = new Mat();
                var cam1 = new VideoCapture(2);
                cam1.Start();
                while (true)
                {
                    cam1.Read(frame1);
                    CvInvoke.Imshow("Cam3", frame1);
                    CvInvoke.WaitKey(1);
                }
            });

            cam1T.Start();
            Thread.Sleep(2000);
     //       cam1T.Abort();
            cam2T.Start();
            Thread.Sleep(2000);
       //     cam2T.Abort();
            cam3T.Start();

        }
    }
}
