using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Stitching;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.IO;

namespace CV
{
    public class ImageStitching
    {
        public void stitich()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\TestImages\";
            var rightImagePath = path + "right.jpg";
            var leftImagePath = path + "left.jpg";

            var images = new VectorOfMat();
            images.Push(CvInvoke.Imread(leftImagePath, ImreadModes.AnyColor));
            images.Push(CvInvoke.Imread(rightImagePath, ImreadModes.AnyColor));


            var result = new Mat();
            var stitcher = new Stitcher(Stitcher.Mode.Scans, true);
            Stitcher.Status stitchStatus = stitcher.Stitch(images, result);
            CvInvoke.Imwrite(path + "new_image.jpg", result);
            Console.WriteLine(stitchStatus);

        }
    }
}
