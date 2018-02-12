using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ZXing;
using System.Linq;
using DataMatrix.net;
using BarcodeLib.BarcodeReader;

namespace CV
{
    public class Barcode
    {
        public void ReadBarcode()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\TestImages\";
            var imagePath = path + "real.jpg";
            // var barcodeBitmap = (Bitmap)Bitmap.FromFile(imagePath);
            var image = CvInvoke.Imread(imagePath, ImreadModes.AnyColor);

            var gray = new Mat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
            CvInvoke.Imwrite(path + "gray.jpg", gray);

            var gradX = new Mat();
            CvInvoke.Sobel(gray, gradX, DepthType.Cv32F, 1, 0);
            CvInvoke.Imwrite(path + "grayx.jpg", gradX);

            var gradY = new Mat();
            CvInvoke.Sobel(gray, gradY, DepthType.Cv32F, 0, 1);
            CvInvoke.Imwrite(path + "grayy.jpg", gradY);


            var gradient = new Mat();
            CvInvoke.Subtract(gradX, gradY, gradient);
            CvInvoke.Imwrite(path + "gradient.jpg", gradient);

            var scaleAbs = new Mat();
            CvInvoke.ConvertScaleAbs(gradient, scaleAbs, 2, 0);
            CvInvoke.Imwrite(path + "scaleAbs.jpg", scaleAbs);

            var blured = new Mat();
            CvInvoke.Blur(gradient, blured, new Size(9, 9), new Point(-1, -1));
            CvInvoke.Imwrite(path + "blured.jpg", blured);


            var thrush = new Mat();
            CvInvoke.Threshold(scaleAbs, thrush, 235, 255, ThresholdType.Binary);
            CvInvoke.Imwrite(path + "thrush.jpg", thrush);


            var closed = new Mat();
            var kernal = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(2, 2), new Point());
            CvInvoke.MorphologyEx(thrush, closed, MorphOp.Close, kernal, new Point(), 25, BorderType.Default, new Emgu.CV.Structure.MCvScalar());
            CvInvoke.Imwrite(path + "closed.jpg", closed);


            CvInvoke.Erode(closed, closed, kernal, new Point(-1, -1), 10, BorderType.Default, new Emgu.CV.Structure.MCvScalar());
            CvInvoke.Imwrite(path + "Enode.jpg", closed);
            CvInvoke.Dilate(closed, closed, kernal, new Point(-1, -1), 20, BorderType.Default, new Emgu.CV.Structure.MCvScalar());
            CvInvoke.Imwrite(path + "Dilate.jpg", closed);


            var contours = new VectorOfMat();
            CvInvoke.FindContours(closed, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            

            CvInvoke.DrawContours(image, contours, -1, new Emgu.CV.Structure.MCvScalar(0, 255, 0),2);
            CvInvoke.Imwrite(path + "find.jpg", image);




            var mask = new Mat();
            CvInvoke.Resize(image, mask, contours[LargestArea(contours)].Size,3,3);
            CvInvoke.Imwrite(path + "final.jpg", mask);


           
            var bitMap = new Bitmap(Image.FromFile(path + "real1.png"));
            var reader = new ZXing.BarcodeReader();
            reader.Options.TryHarder = true;
            //reader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.DATA_MATRIX };

            reader.AutoRotate = true;
            var result = reader.Decode(bitMap);
            if (result != null)
            {
                Console.WriteLine(result.BarcodeFormat.ToString());
                Console.WriteLine(result.Text);
            }


            var decoder = new DmtxImageDecoder();
            var list = decoder.DecodeImage(bitMap);
           var list2=  decoder.DecodeImageMosaic(bitMap);

            var list3 = BarcodeLib.BarcodeReader.BarcodeReader.read(bitMap, BarcodeLib.BarcodeReader.BarcodeReader.DATAMATRIX);

        }


        int LargestArea(VectorOfMat vectorOfMat)
        {
            double largest = 0;
            int index = 0;
            double area = 0;
            for (int i = 0; i < vectorOfMat.Size; ++i)
            {
                area = CvInvoke.ContourArea(vectorOfMat[i]);
                if (largest < area)
                {
                    largest = area;
                    index = i;
                }

            }
            return index;
        }
    }
}
