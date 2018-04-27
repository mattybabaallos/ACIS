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
using System.Drawing;

namespace CV
{
    public class ImageStitching
    {
        /* Load all images in given path with set bounds of number of Rows and Columns: */
        public List<Mat> Load_images(string path, int seg_R, int seg_C, string name)
        {
            List<Mat> images = new List<Mat>();
            for (int y = 0; y < seg_R; y++)
            {
                for (int x = 0; x < seg_C; x++)
                {
                    var filename = name + y + x + ".jpg";
                    //Console.WriteLine(filename);
                    var img_to_add = CvInvoke.Imread(path + "/" + filename, ImreadModes.Color);
                    images.Add(img_to_add);

                }
            }
            return images;
        }

        /* Crops Images based on ROI. Returns cropped Image: */
        static Mat Crop_image(Mat input, Rectangle crop_region)
        {
            Image<Gray, Byte> buffer_im = input.ToImage<Gray, byte>();
            buffer_im.ROI = crop_region;
            Image<Gray, byte> cropped_im = buffer_im.Copy();
            return cropped_im.Mat;
        }

        /* Stitch two Images together that are assumed to be in the same Row: */
        static Mat Stitch_image_w(Mat img1, Mat imgT, Size img1_size, Size imgT_size, Point max_loc, int new_temp_h, int size_to_crop)
        {
            Image<Bgr, Byte> first_image = img1.ToImage<Bgr, byte>();
            Image<Bgr, Byte> second_image = imgT.ToImage<Bgr, byte>();
            Image<Bgr, Byte> imageResult = new Image<Bgr, byte>(imgT_size.Width + size_to_crop, new_temp_h);

            for (int yy = 0; yy < img1_size.Width; yy++)
            {
                for (int x = 0; x < img1_size.Height; x++)
                {
                    imageResult[x, yy] = first_image[x, yy];     //ROW,COL
                }
            }
            for (int yy = max_loc.X; yy < imgT_size.Width + max_loc.X; yy++)
            {
                for (int x = 0; x < imgT_size.Height; x++)
                {
                    imageResult[x, yy] = second_image[x, yy - max_loc.X];     //ROW,COL  size_to_crop instead of max_loc.X
                }
            }
            return imageResult.Mat;
        }

        /* Stitch two Images together that are assumed to be in the same Column: */
        static Mat Stitch_image_h(Mat img1, Mat imgT, Size img1_size, Size imgT_size, Point max_loc, int new_temp_h, int size_to_crop)
        {
            Image<Bgr, Byte> first_image = img1.ToImage<Bgr, byte>();
            Image<Bgr, Byte> second_image = imgT.ToImage<Bgr, byte>();
            Image<Bgr, Byte> imageResult = new Image<Bgr, byte>(imgT_size.Width, new_temp_h + size_to_crop);

            for (int yy = 0; yy < img1_size.Width; yy++)
            {
                for (int x = 0; x < img1_size.Height; x++)
                {
                    imageResult[x, yy] = first_image[x, yy];     //ROW,COL
                }
            }

            for (int yy = 0; yy < imgT_size.Width; yy++)
            {
                for (int x = size_to_crop; x < imgT_size.Height + size_to_crop; x++)
                {
                    imageResult[x, yy] = second_image[x - size_to_crop, yy];     //ROW,COL  size_to_crop instead of max_loc.X
                }
            }
            return imageResult.Mat;
        }

        /* Stitch all Images given together.
         * Assumes order of Images is given: */
        public Mat Stitching_images(List<Mat> images, int seg_R, int seg_C)
        {
            /*  Testing to see if an Image can be loaded in using Emgu.CV: */
            /*
            var path = "C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel\\Stiched_Image.jpg";
            var image = CvInvoke.Imread(path, ImreadModes.AnyColor);
            CvInvoke.NamedWindow("Image", NamedWindowType.Normal);
            CvInvoke.Imshow("Image", image);
            CvInvoke.WaitKey();
            CvInvoke.DestroyAllWindows();
            */


            var overlap = 0.65;         //amount of assumed overlap for the photos  
            List<Mat> final = new List<Mat>();


            for (int k = 0; k < seg_R; k++)
            {
                var img_start = images[seg_C * k];
                var img_start_size = img_start.Size;
                var done = new Mat();
                done = img_start;
                for (int y = 0; y < seg_C - 1; y++)
                {
                    var img_color = done;
                    var template_color = images[seg_C * k + (y + 1)];
                    var img = new Mat();
                    var tempalte = new Mat();
                    CvInvoke.CvtColor(img_color, img, ColorConversion.Bgr2Gray);
                    var img2 = img;
                    CvInvoke.CvtColor(template_color, tempalte, ColorConversion.Bgr2Gray);
                    var new_temp = tempalte;
                    var new_temp_w = 0;                             //template new width after overlap taken into account
                    var new_temp_h = 0;                             //template new hight after overlap taken into account


                    var template_size = template_color.Size;
                    var img_size = img.Size;
                    new_temp_w = Convert.ToInt32(Math.Round(template_size.Width - (template_size.Width * overlap)));
                    if (template_size.Height > img_size.Height)
                        new_temp_h = template_size.Height;
                    else
                        new_temp_h = img_size.Height;
                    var end_w = new_temp_w + img_size.Width;

                    Rectangle crop_region = new Rectangle(0, 0, new_temp_w, new_temp_h);
                    var cropped_im = Crop_image(new_temp, crop_region);



                    var res = new Mat();
                    double min_val = 0; double max_val = 0; Point min_loc = new Point(); Point max_loc = new Point();
                    CvInvoke.MatchTemplate(img2, cropped_im, res, TemplateMatchingType.CcoeffNormed);
                    CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc);
                    Point bottom_right = new Point
                    {
                        X = max_loc.X + template_size.Width,
                        Y = max_loc.Y + template_size.Height
                    };
                    //crop region adjust: (0,0,template_size.Width,new_temp_h)
                    {
                        crop_region.X = 0;
                        crop_region.Y = 0;
                        crop_region.Width = template_size.Width;
                        crop_region.Height = new_temp_h;
                    }
                    var imgT = template_color; //Crop_image(template_color, crop_region);
                    var img1 = done;
                    var img1_size = img1.Size;
                    var imgT_size = imgT.Size;


                    Image<Bgr, Byte> first_image = img1.ToImage<Bgr, byte>();
                    Image<Bgr, Byte> second_image = imgT.ToImage<Bgr, byte>();
                    Mat imageResult = new Mat();
                    imageResult = Stitch_image_w(img1, imgT, img1_size, imgT_size, max_loc, new_temp_h, max_loc.X);


                    var cnt = new VectorOfMat();

                    var imageResult_bw = imageResult.Clone();
                    var hierarchy = new Mat();
                    CvInvoke.CvtColor(imageResult, imageResult_bw, ColorConversion.Bgr2Gray);
                    CvInvoke.Threshold(imageResult_bw, imageResult_bw, 1, 255, ThresholdType.Binary);
                    CvInvoke.FindContours(imageResult_bw, cnt, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);


                    /* Crop black boarders out: */
                    /*  ADD in Cropping here! */



                    done = imageResult.Clone();
                }
                final.Add(done);
            }
            final.Reverse();
            int current = 0;
            for (int i = 0; i < seg_R; i++)
            {
                if (current < final[i].Size.Width)
                    current = final[i].Size.Width;

                /*
                CvInvoke.NamedWindow("Image", NamedWindowType.Normal);
                CvInvoke.Imshow("Image", final[i]);
                CvInvoke.WaitKey();
                CvInvoke.DestroyAllWindows();
                */
            }

            var finished = final[0];
            int size_to_crop = 0;
            for (int x = 0; x < seg_R - 1; x++)
            {
                var img_color = finished;
                var template_color = final[x + 1];
                var img = new Mat();
                var tempalte = new Mat();
                CvInvoke.CvtColor(img_color, img, ColorConversion.Bgr2Gray);
                var img2 = img;
                CvInvoke.CvtColor(template_color, tempalte, ColorConversion.Bgr2Gray);
                var new_temp = tempalte;
                var new_temp_w = 0;                             //template new width after overlap taken into account
                var new_temp_h = 0;                             //template new hight after overlap taken into account


                var template_size = template_color.Size;
                var img_size = img.Size;
                var diff = tempalte.Size.Width - img_color.Size.Width;
                //Console.WriteLine("value of diff:"+ diff);
                if (diff > 0)
                {
                    //var temp = Convert.ToInt32(Math.Round(decimal.Divide(diff, 2)));
                    MCvScalar value;
                    value.V0 = 0; value.V1 = 0; value.V2 = 0; value.V3 = 0;
                    CvInvoke.CopyMakeBorder(img, img2, 0, 0, 0, diff, BorderType.Constant, value);
                }
                else
                {
                    //var temp = Convert.ToInt32(Math.Round(decimal.Divide(-diff, 2)));
                    MCvScalar value;
                    value.V0 = 0; value.V1 = 0; value.V2 = 0; value.V3 = 0;
                    CvInvoke.CopyMakeBorder(template_color, template_color, 0, 0, 0, -diff, BorderType.Constant, value);
                }

                new_temp_h = Convert.ToInt32(Math.Round(img_size.Height - (template_size.Height * overlap)));
                new_temp_w = current;

                Rectangle crop_region = new Rectangle(0, 0, new_temp_w, new_temp_h);
                var cropped_im = Crop_image(new_temp, crop_region);



                var res = new Mat();
                double min_val = 0; double max_val = 0; Point min_loc = new Point(); Point max_loc = new Point();
                //Console.WriteLine("img2:"+img2.Size.Height+"&"+img2.Size.Width+"vs cropped_im"+cropped_im.Height+"&"+cropped_im.Width);
                CvInvoke.MatchTemplate(img2, cropped_im, res, TemplateMatchingType.CcoeffNormed);
                CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc);
                Point bottom_right = new Point
                {
                    X = max_loc.X + template_size.Width,
                    Y = max_loc.Y + template_size.Height
                };
                //crop region adjust: (0,0,template_size.Width,new_temp_h)
                {
                    crop_region.X = 0;
                    crop_region.Y = 0;
                    crop_region.Width = template_size.Width;
                    crop_region.Height = new_temp_h;
                }
                var imgT = template_color; //Crop_image(template_color, crop_region);
                var img1 = finished;
                var img1_size = img1.Size;
                var imgT_size = imgT.Size;


                Image<Bgr, Byte> first_image = img1.ToImage<Bgr, byte>();
                Image<Bgr, Byte> second_image = imgT.ToImage<Bgr, byte>();
                Mat imageResult = new Mat();
                size_to_crop = size_to_crop + max_loc.Y;
                if (x > 0)
                {
                    size_to_crop = img1.Size.Height - ((imgT.Size.Height) - (max_loc.Y));

                }
                imageResult = Stitch_image_h(img1, imgT, img1_size, imgT_size, max_loc, img1_size.Height, size_to_crop);


                var cnt = new VectorOfMat();
                var imageResult_bw = imageResult.Clone();
                CvInvoke.CvtColor(imageResult, imageResult_bw, ColorConversion.Bgr2Gray);
                CvInvoke.Threshold(imageResult_bw, imageResult_bw, 1, 255, ThresholdType.Binary);
                CvInvoke.FindContours(imageResult_bw, cnt, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                //Console.WriteLine("Test:"+cnt[0].Data.GetValue(0));

                /* Crop black boarders out: */
                /*  ADD in Cropping here! */
                //CvInvoke.DrawContours(imageResult, cnt, -1, new Emgu.CV.Structure.MCvScalar(0, 255, 0), 1);
                //imageResult.CopyTo(imageResult,cnt[0]);


                finished = imageResult.Clone();
                /*
                CvInvoke.NamedWindow("Image", NamedWindowType.Normal);
                CvInvoke.Imshow("Image", finished);
                CvInvoke.WaitKey();
                CvInvoke.DestroyAllWindows();
                */
            }

            return finished;
        }
    }
}
