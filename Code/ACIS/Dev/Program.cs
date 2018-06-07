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
using CV;

namespace Dev
{
    class Program
    {
        static void Main(string[] args)
        {
           








           // var cam1T = new Thread(() => {
           //     var frame1 = new Mat();
           //     var cam1 = new VideoCapture(0);
           //     cam1.SetCaptureProperty(CapProp.FrameWidth, 320);
           //     cam1.SetCaptureProperty(CapProp.FrameHeight, 240);
           //     cam1.Start();
           //     while (true)
           //     {
           //         cam1.Read(frame1);
           //         CvInvoke.Imshow("Cam1", frame1);
           //         CvInvoke.WaitKey(1);
           //     }
           //     CvInvoke.DestroyAllWindows();
           // });

           // var cam2T = new Thread(() => {
           //     var frame1 = new Mat();
           //     var cam1 = new VideoCapture(1);
           //     cam1.SetCaptureProperty(CapProp.FrameWidth, 320);
           //     cam1.SetCaptureProperty(CapProp.FrameHeight, 240);
           //     cam1.Start();
           //     while (true)
           //     {
           //         cam1.Read(frame1);
           //         CvInvoke.Imshow("Cam2", frame1);
           //         CvInvoke.WaitKey(1);
           //     }
           //     CvInvoke.DestroyAllWindows();
           // });

           // var cam3T = new Thread(() => {
           //     var frame1 = new Mat();
           //     var cam1 = new VideoCapture(2);
           //     cam1.SetCaptureProperty(CapProp.FrameWidth, 320);
           //     cam1.SetCaptureProperty(CapProp.FrameHeight, 240);
           //     cam1.Start();
           //     while (true)
           //     {
           //         cam1.Read(frame1);
           //         CvInvoke.Imshow("Cam3", frame1);
           //         CvInvoke.WaitKey(1);
           //     }
           // });

           // var cam4T = new Thread(() => {
           //     var frame1 = new Mat();
           //     var cam1 = new VideoCapture(3);
           //     cam1.SetCaptureProperty(CapProp.FrameWidth, 320);
           //     cam1.SetCaptureProperty(CapProp.FrameHeight, 240);
           //     cam1.Start();
           //     while (true)
           //     {
           //         cam1.Read(frame1);
           //         CvInvoke.Imshow("Cam4", frame1);
           //         CvInvoke.WaitKey(1);
           //     }
           // });

           // cam1T.Start();
           // cam2T.Start();
           // cam3T.Start();
           //// cam4T.Start();

           // var frame1 = new Mat();
           // var cam1 = new VideoCapture(0);
           // cam1.Start();
           // for (int i = 0; i < 10; i++)
           // {
           //     cam1.Read(frame1);
           //     CvInvoke.Imshow("Cam1", frame1);
           //     CvInvoke.WaitKey(1);
           // }


           // cam1.Stop();
           // cam1.Dispose();

           // cam1 = new VideoCapture(1);
           // for (int i = 0; i < 10; i++)
           // {
           //     cam1.Read(frame1);
           //     CvInvoke.Imshow("Cam1", frame1);
           //     CvInvoke.WaitKey(1);
           // }


           // cam1.Stop();
           // cam1.Dispose();

           // cam1 = new VideoCapture(2);
           // for (int i = 0; i < 10; i++)
           // {
           //     cam1.Read(frame1);
           //     CvInvoke.Imshow("Cam1", frame1);
           //     CvInvoke.WaitKey(1);
           // }

            //var cam1T = new Thread(() => {
            //    var frame1 = new Mat();
            //    var cam1 = new VideoCapture(0);
            //    cam1.Start();
            //    while (true)
            //    {
            //        cam1.Read(frame1);
            //        CvInvoke.Imshow("Cam1", frame1);
            //        CvInvoke.WaitKey(1);
            //    }
            //    CvInvoke.DestroyAllWindows();
            //});

            //var cam2T = new Thread(() => {
            //    var frame1 = new Mat();
            //    var cam1 = new VideoCapture(1);
            //    cam1.Start();
            //    while (true)
            //    {
            //        cam1.Read(frame1);
            //        CvInvoke.Imshow("Cam2", frame1);
            //        CvInvoke.WaitKey(1);
            //    }
            //    CvInvoke.DestroyAllWindows();
            //});

            //var cam3T = new Thread(() => {
            //    var frame1 = new Mat();
            //    var cam1 = new VideoCapture(2);
            //    cam1.Start();
            //    while (true)
            //    {
            //        cam1.Read(frame1);
            //        CvInvoke.Imshow("Cam3", frame1);
            //        CvInvoke.WaitKey(1);
            //    }

            //cam1T.Start();
            //Thread.Sleep(2000);
            //cam1T.Abort();
            //cam2T.Start();
            //Thread.Sleep(2000);
            //cam2T.Abort();
            //cam3T.Start();

        }
    }
}
