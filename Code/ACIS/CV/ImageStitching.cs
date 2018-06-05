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
        /* Stitch two Images together that are assumed to be in the same Column: */
        static Mat Stitch_image_h(Mat img1, Mat img2, Point max_loc, double overlap)
        {

            if (img1.Size.Width != img2.Size.Width)
            {
                var temp = 0.0;
                if (img1.Size.Width > img2.Size.Width)
                {
                    temp = img1.Size.Width - img2.Size.Width;
                    CvInvoke.CopyMakeBorder(img2, img2, 0, 0, Convert.ToInt32(Math.Round(temp / 2)), Convert.ToInt32(Math.Round(temp / 2)), BorderType.Constant);
                }
                else
                {
                    temp = img2.Size.Width - img1.Size.Width;
                    CvInvoke.CopyMakeBorder(img1, img1, 0, 0, Convert.ToInt32(Math.Round(temp / 2)), Convert.ToInt32(Math.Round(temp / 2)), BorderType.Constant);
                }
            }

            Image<Bgr, Byte> first_image = img1.ToImage<Bgr, byte>();
            Image<Bgr, Byte> second_image = img2.ToImage<Bgr, byte>();
            var new_width = Math.Abs(max_loc.X - Convert.ToInt32(Math.Round(img1.Size.Width * (.10)))) + img1.Size.Width + 1;
            var new_hight = Math.Abs(max_loc.Y - Convert.ToInt32(Math.Round(img1.Size.Height * (.10)))) + img1.Size.Height + img2.Size.Height - Convert.ToInt32(Math.Round(img2.Size.Height * overlap));
            Image<Bgr, Byte> imageResult = new Image<Bgr, byte>(new_width, new_hight);

            var y_val = max_loc.Y - Convert.ToInt32(Math.Round(img1.Size.Height * (.10)));
            var x_val = max_loc.X - Convert.ToInt32(Math.Round(img1.Size.Width * (.10)));

            /* ******************************************************* */
            /* First Image: */
            /* ******************************************************* */

            /* img1 doesnt start at (0,0) */
            if (y_val < 0)
            {
                for (int x = 0; x < img1.Size.Width; x++)
                {
                    for (int y = 0; y < img1.Size.Height; y++)
                    {
                        imageResult[y + Math.Abs(y_val), x] = first_image[y, x];
                    }
                }
            }
            /*  img1 starts at (0,0) */
            else
            {
                for (int x = 0; x < img1.Size.Width; x++)
                {
                    for (int y = 0; y < img1.Size.Height; y++)
                    {
                        imageResult[y, x] = first_image[y, x];
                    }
                }
            }
//            CvInvoke.NamedWindow("stitched", NamedWindowType.Normal);
//            CvInvoke.Imshow("stitched", imageResult);
//            CvInvoke.WaitKey();
//            CvInvoke.DestroyAllWindows();

            /* ******************************************************* */
            /* Second Image: */
            /* ******************************************************* */

            /* img2 starts above img1 */
            if (y_val < 0)
            {
                for (int x = 0; x < img2.Size.Width; x++)
                {
                    for (int y = 0; y < img2.Size.Height; y++)
                    {
                        //imageResult[y, x+Math.Abs(x_val)+ Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap)))] = second_image[y, x];
                        imageResult[y + y_val + Convert.ToInt32(Math.Round(img1.Size.Height * (1 - overlap))), x] = second_image[y, x];
                    }
                }
            }
            /*  img2 starts at below img1 */
            else
            {
                for (int x = 0; x < img2.Size.Width; x++)
                {
                    for (int y = 0; y < img2.Size.Height; y++)
                    {
                        //imageResult[y+Math.Abs(y_val), x+Math.Abs(x_val)+ Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap)))] = second_image[y, x];
                        imageResult[y + y_val + Convert.ToInt32(Math.Round(img1.Size.Height * (1 - overlap))), x] = second_image[y, x];
                    }
                }
            }


            return imageResult.Mat;
        }

        /* Stitch two Images together that are assumed to be in the same Row: */
        static Mat Stitch_image_w(Mat img1, Mat img2, Point max_loc, double overlap)
        {
            Image<Bgr, Byte> first_image = img1.ToImage<Bgr, byte>();
            Image<Bgr, Byte> second_image = img2.ToImage<Bgr, byte>();
            var new_width = Math.Abs(max_loc.X - Convert.ToInt32(Math.Round(img1.Size.Width * (.10)))) + img1.Size.Width + img2.Size.Width - Convert.ToInt32(Math.Round(img2.Size.Width * overlap));
            var new_hight = Math.Abs(max_loc.Y - Convert.ToInt32(Math.Round(img1.Size.Height * (.10)))) + img1.Size.Height;
            Image<Bgr, Byte> imageResult = new Image<Bgr, byte>(new_width, new_hight);

            var y_val = max_loc.Y - Convert.ToInt32(Math.Round(img1.Size.Height * (.10)));
            var x_val = max_loc.X - Convert.ToInt32(Math.Round(img1.Size.Width * (.10)));

            Console.WriteLine("Stitching info:");
            Console.WriteLine(y_val + "and" + x_val);
            Console.WriteLine(new_hight + "and" + new_width);
            Console.WriteLine(img1.Height + "and" + img1.Width);
            Console.WriteLine(img2.Height + "and" + img2.Width);
            Console.WriteLine(max_loc.X);

            /* ******************************************************* */
            /* First Image: */
            /* ******************************************************* */

            /* img1 doesnt start at (0,0) */
            if (y_val < 0)
            {
                for (int x = 0; x < img1.Size.Width; x++)
                {
                    for (int y = 0; y < img1.Size.Height; y++)
                    {
                        imageResult[y + Math.Abs(y_val), x] = first_image[y, x];
                    }
                }
            }
            /*  img1 starts at (0,0) */
            else
            {
                for (int x = 0; x < img1.Size.Width; x++)
                {
                    for (int y = 0; y < img1.Size.Height; y++)
                    {
                        imageResult[y, x] = first_image[y, x];
                    }
                }
            }
            //CvInvoke.NamedWindow("stitched", NamedWindowType.Normal);
            //CvInvoke.Imshow("stitched", imageResult);
            //CvInvoke.WaitKey();
            //CvInvoke.DestroyAllWindows();

            /* ******************************************************* */
            /* Second Image: */
            /* ******************************************************* */

            /* img2 starts above img1 */
            if (y_val < 0)
            {
                for (int x = 0; x < img2.Size.Width; x++)
                {
                    for (int y = 0; y < img2.Size.Height; y++)
                    {
                        //imageResult[y, x+Math.Abs(x_val)+ Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap)))] = second_image[y, x];
                        imageResult[y, x + x_val + Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap)))] = second_image[y, x];
                    }
                }
            }
            /*  img2 starts at below img1 */
            else
            {
                for (int x = 0; x < img2.Size.Width; x++)
                {
                    for (int y = 0; y < img2.Size.Height; y++)
                    {
                        //imageResult[y+Math.Abs(y_val), x+Math.Abs(x_val)+ Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap)))] = second_image[y, x];
                        imageResult[y, x + x_val + Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap)))] = second_image[y, x];
                    }
                }
            }

            /*
            CvInvoke.NamedWindow("img1", NamedWindowType.Normal);
            CvInvoke.Imshow("img1", img1);
            CvInvoke.NamedWindow("img2", NamedWindowType.Normal);
            CvInvoke.Imshow("img2", img2);*/
            //CvInvoke.NamedWindow("stitched", NamedWindowType.Normal);
            //CvInvoke.Imshow("stitched", imageResult);
            //CvInvoke.WaitKey();
            //CvInvoke.DestroyAllWindows();




            return imageResult.Mat;
        }

        /* Load all images in given path with set bounds of number of Rows and Columns: */
        public List<Mat> Load_images(string path, int seg_R, int seg_C, string name)
        {
            List<Mat> images = new List<Mat>();
            List<Image<Bgr, Byte>> sourceImages = new List<Image<Bgr, Byte>>();

            for (int y = 0; y < seg_R; y++)
            {
                for (int x = 0; x < seg_C; x++)
                {
                    var filename = name + y + x + ".jpg";
                    //Console.WriteLine(filename);
                    var img_to_add = CvInvoke.Imread(path + "/" + filename, ImreadModes.Color);
                    images.Add(img_to_add);
                    Image<Bgr, Byte> img_add = img_to_add.ToImage<Bgr, Byte>();
                    sourceImages.Add(img_add);

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


            var overlap = 0.4;         //amount of assumed overlap for the photos 
            var good_overlap = 0.4;
            var good_max_val = 0.1;
            List<Mat> final = new List<Mat>();

            /* Stithing based on width! */
            for (int k = 0; k < seg_R; k++)
            {
                var img_stitched_w = new Mat();
                var done = new Mat();
                for (int y = 0; y < seg_C - 1; y++)
                {
                    var img2 = new Mat();
                    var img1 = new Mat();
                    CvInvoke.CvtColor(images[seg_C * k + y], img1, ColorConversion.Bgr2Gray);
                    CvInvoke.CvtColor(images[seg_C * k + (y + 1)], img2, ColorConversion.Bgr2Gray);
                    overlap = 0.40;
                    double min_val = 0; double max_val = 0; Point min_loc = new Point(); Point max_loc = new Point();

                    /* Find the overlap ratio that gives the best match: */
                    while (overlap >= 0.4 && overlap <= 0.7)
                    {
                        Rectangle test_crop_region_img1 = new Rectangle(Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap))), 0, Convert.ToInt32(Math.Round(overlap * img1.Size.Width)), img2.Size.Height);
                        Rectangle test_crop_region_img2 = new Rectangle(0, 0, Convert.ToInt32(Math.Round(overlap * img2.Size.Width)), img2.Size.Height);
                        Rectangle test_crop_region_test = new Rectangle(0, 0, Convert.ToInt32(Math.Round(overlap * img2.Size.Width)), img2.Size.Height);
                        var test_img1_temp = Crop_image(img1, test_crop_region_img1);
                        var test_img2_temp = Crop_image(img2, test_crop_region_img2);



                        CvInvoke.CopyMakeBorder(test_img1_temp, test_img1_temp, Convert.ToInt32(Math.Round(img1.Size.Height * (.10))), Convert.ToInt32(Math.Round(img1.Size.Height * (.10))),
                            Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), BorderType.Constant);

                        var res = new Mat();


                        //CvInvoke.Canny(img2_temp, img2_temp, 300, 10, 3);
                        //CvInvoke.Canny(img1_temp, img1_temp, 300, 10, 3);

                        CvInvoke.MatchTemplate(test_img1_temp, test_img2_temp, res, TemplateMatchingType.CcoeffNormed);
                        CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc);

                        Console.WriteLine("value_height: " + Convert.ToInt32(Math.Round(img1.Size.Height * (.10))) + "  value width: " + Convert.ToInt32(Math.Round(img1.Size.Width * (.10))));
                        Console.WriteLine(max_val);
                        Console.WriteLine(max_loc);

                        /*
                        CvInvoke.NamedWindow("img1", NamedWindowType.Normal);
                        CvInvoke.Imshow("img1", img1);
                        CvInvoke.NamedWindow("img2", NamedWindowType.Normal);
                        CvInvoke.Imshow("img2", img2);
                        CvInvoke.WaitKey();
                        CvInvoke.DestroyAllWindows();
                        */

                        if (good_max_val < max_val)
                        {
                            good_overlap = overlap;
                        }
                        overlap += .05;
                    }

                    overlap = good_overlap - .03;
                    var temp = good_overlap;


                    while (overlap >= temp - .03 && overlap <= temp + .03)
                    {
                        Rectangle test_crop_region_img1 = new Rectangle(Convert.ToInt32(Math.Round(img1.Size.Width * (1 - overlap))), 0, Convert.ToInt32(Math.Round(overlap * img1.Size.Width)), img2.Size.Height);
                        Rectangle test_crop_region_img2 = new Rectangle(0, 0, Convert.ToInt32(Math.Round(overlap * img2.Size.Width)), img2.Size.Height);
                        Rectangle test_crop_region_test = new Rectangle(0, 0, Convert.ToInt32(Math.Round(overlap * img2.Size.Width)), img2.Size.Height);
                        var test_img1_temp = Crop_image(img1, test_crop_region_img1);
                        var test_img2_temp = Crop_image(img2, test_crop_region_img2);



                        CvInvoke.CopyMakeBorder(test_img1_temp, test_img1_temp, Convert.ToInt32(Math.Round(img1.Size.Height * (.10))), Convert.ToInt32(Math.Round(img1.Size.Height * (.10))),
                            Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), BorderType.Constant);

                        var res = new Mat();


                        //CvInvoke.Canny(img2_temp, img2_temp, 300, 10, 3);
                        //CvInvoke.Canny(img1_temp, img1_temp, 300, 10, 3);

                        CvInvoke.MatchTemplate(test_img1_temp, test_img2_temp, res, TemplateMatchingType.CcoeffNormed);
                        CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc);
                        Console.WriteLine("value_height: " + Convert.ToInt32(Math.Round(img1.Size.Height * (.10))) + "  value width: " + Convert.ToInt32(Math.Round(img1.Size.Width * (.10))));
                        Console.WriteLine(max_val);
                        Console.WriteLine(max_loc);

                        if (good_max_val < max_val)
                        {
                            good_overlap = overlap;
                        }
                        overlap += .005;

                    }


                    Rectangle crop_region_img1 = new Rectangle(Convert.ToInt32(Math.Round(img1.Size.Width * (1 - good_overlap))), 0, Convert.ToInt32(Math.Round(good_overlap * img1.Size.Width)), img2.Size.Height);
                    Rectangle crop_region_img2 = new Rectangle(0, 0, Convert.ToInt32(Math.Round(good_overlap * img2.Size.Width)), img2.Size.Height);
                    Rectangle crop_region_test = new Rectangle(0, 0, Convert.ToInt32(Math.Round(good_overlap * img2.Size.Width)), img2.Size.Height);
                    var img1_temp = Crop_image(img1, crop_region_img1);
                    var img2_temp = Crop_image(img2, crop_region_img2);



                    CvInvoke.CopyMakeBorder(img1_temp, img1_temp, Convert.ToInt32(Math.Round(img1.Size.Height * (.10))), Convert.ToInt32(Math.Round(img1.Size.Height * (.10))),
                        Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), BorderType.Constant);

                    var result = new Mat();


                    //CvInvoke.Canny(img2_temp, img2_temp, 300, 10, 3);
                    //CvInvoke.Canny(img1_temp, img1_temp, 300, 10, 3);

                    CvInvoke.MatchTemplate(img1_temp, img2_temp, result, TemplateMatchingType.CcoeffNormed);
                    CvInvoke.MinMaxLoc(result, ref min_val, ref max_val, ref min_loc, ref max_loc);
                    Mat imageResult = new Mat();
                    //CvInvoke.NamedWindow("img1", NamedWindowType.Normal);
                    //CvInvoke.Imshow("img1", img1_temp);
                    //CvInvoke.NamedWindow("img2", NamedWindowType.Normal);
                    //CvInvoke.Imshow("img2", img2_temp);
                    //CvInvoke.WaitKey();
                    //CvInvoke.DestroyAllWindows();
                    imageResult = Stitch_image_w(images[seg_C * k + y], images[seg_C * k + (y + 1)], max_loc, good_overlap);
                    done = imageResult.Clone();

                }
                /* For debugging purposes */
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                final.Add(done);
            }


            /* check images! */
            // final.Reverse();
            int current = 0;
            for (int i = 0; i < seg_R; i++)
            {
                if (current < final[i].Size.Width)
                    current = final[i].Size.Width;

                //CvInvoke.NamedWindow("Image_final", NamedWindowType.Normal);
                //CvInvoke.Imshow("Image_final", final[i]);
                //CvInvoke.WaitKey();
                //CvInvoke.DestroyAllWindows();

            }


            /* Stitching based on height! */
            /* Reset values: */
            overlap = 0.4;         //amount of assumed overlap for the photos 
            good_overlap = 0.4;
            good_max_val = 0.1;


            var finished = final[0];
            for (int x = 0; x < seg_R - 1; x++)
            {
                var img2 = final[x + 1];
                var img1 = finished;
                double min_val = 0; double max_val = 0; Point min_loc = new Point(); Point max_loc = new Point();


                /* Find the overlap ratio that gives the best match: */
                while (overlap >= 0.4 && overlap <= 0.7)
                {
                    Rectangle test_crop_region_img1 = new Rectangle(0, Convert.ToInt32(Math.Round(img1.Size.Height * (1 - overlap))), img1.Size.Width, img1.Size.Height);
                    Rectangle test_crop_region_img2 = new Rectangle(0, 0, img2.Size.Width, Convert.ToInt32(Math.Round((1 - overlap) * img2.Size.Height)));
                    Rectangle test_crop_region_test = new Rectangle(0, 0, img2.Size.Width, Convert.ToInt32(Math.Round((1 - overlap) * img2.Size.Height)));
                    var test_img1_temp = Crop_image(img1, test_crop_region_img1);
                    var test_img2_temp = Crop_image(img2, test_crop_region_img2);



                    CvInvoke.CopyMakeBorder(test_img1_temp, test_img1_temp, Convert.ToInt32(Math.Round(img1.Size.Height * (.15))), Convert.ToInt32(Math.Round(img1.Size.Height * (.15))),
                        Convert.ToInt32(Math.Round(img1.Size.Width * (.15))), Convert.ToInt32(Math.Round(img1.Size.Width * (.15))), BorderType.Constant);

                    var res = new Mat();


                    //CvInvoke.Canny(img2_temp, img2_temp, 300, 10, 3);
                    //CvInvoke.Canny(img1_temp, img1_temp, 300, 10, 3);

                    CvInvoke.MatchTemplate(test_img1_temp, test_img2_temp, res, TemplateMatchingType.CcoeffNormed);
                    CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc);

                    Console.WriteLine("value_height: " + Convert.ToInt32(Math.Round(img1.Size.Height * (.10))) + "  value width: " + Convert.ToInt32(Math.Round(img1.Size.Width * (.10))));
                    Console.WriteLine(max_val);
                    Console.WriteLine(max_loc);

                    /*
                    CvInvoke.NamedWindow("img1", NamedWindowType.Normal);
                    CvInvoke.Imshow("img1", img1);
                    CvInvoke.NamedWindow("img2", NamedWindowType.Normal);
                    CvInvoke.Imshow("img2", img2);
                    CvInvoke.WaitKey();
                    CvInvoke.DestroyAllWindows();
                    */

                    if (good_max_val < max_val)
                    {
                        good_overlap = overlap;
                    }
                    overlap += .05;
                }

                Rectangle crop_region_img1 = new Rectangle(0, Convert.ToInt32(Math.Round(img1.Size.Height * (1 - good_overlap))), img1.Size.Width, img1.Size.Height);
                Rectangle crop_region_img2 = new Rectangle(0, 0, img2.Size.Width, Convert.ToInt32(Math.Round((1 - good_overlap) * img2.Size.Height)));
                Rectangle crop_region_test = new Rectangle(0, 0, img2.Size.Width, Convert.ToInt32(Math.Round((1 - good_overlap) * img2.Size.Height)));
                var img1_temp = Crop_image(img1, crop_region_img1);
                var img2_temp = Crop_image(img2, crop_region_img2);



                CvInvoke.CopyMakeBorder(img1_temp, img1_temp, Convert.ToInt32(Math.Round(img1.Size.Height * (.10))), Convert.ToInt32(Math.Round(img1.Size.Height * (.10))),
                    Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), Convert.ToInt32(Math.Round(img1.Size.Width * (.10))), BorderType.Constant);

                var result = new Mat();


                //CvInvoke.Canny(img2_temp, img2_temp, 300, 10, 3);
                //CvInvoke.Canny(img1_temp, img1_temp, 300, 10, 3);

                CvInvoke.MatchTemplate(img1_temp, img2_temp, result, TemplateMatchingType.CcoeffNormed);
                CvInvoke.MinMaxLoc(result, ref min_val, ref max_val, ref min_loc, ref max_loc);
                Mat imageResult = new Mat();
                //CvInvoke.NamedWindow("img1", NamedWindowType.Normal);
                //CvInvoke.Imshow("img1", img1_temp);
                //CvInvoke.NamedWindow("img2", NamedWindowType.Normal);
                //CvInvoke.Imshow("img2", img2_temp);
                //CvInvoke.WaitKey();
                //CvInvoke.DestroyAllWindows();
                imageResult = Stitch_image_h(img1, img2, max_loc, good_overlap);
                finished = imageResult.Clone();


            }



            return finished;
        }

    }
}
