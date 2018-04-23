using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Services
{
    public class CameraControl
    {
        private VideoCapture videocapture;
        private Mat frame;

        public CameraControl()
        {
            videocapture = new VideoCapture(0);
            frame = new Mat();
        }
        public VideoCapture Videocapture
        {
            get { return videocapture; }
        }

        public Mat Frame
        {
            get { return frame; }
            set { frame = value; }
        }

        public void Capture()
        {
            videocapture.Start();
        }
    }
}
