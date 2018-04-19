/* Kestutis Saltonas
 * Image Processing for ACIS Project:
 * Requires:
 * - Emgu
 * - DataMatrix.net
 * Variables:
 * - Overlap
 * - linspace parameters
 * - template matching of barcode parameters
 * - Seg_R and Seg_C
 * - Image paths
 * - Barcode Image offset
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using DataMatrix.net;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Image_processing
{
    /* All functions relating to capturing and saving an image: */
    class Camera_capture
    {
        /* Number of Images taken: */
        private int Image_count;

        /* Image number for columns: */
        private int Image_number_C;

        /* Image number for rows: */
        private int Image_number_R;

        /* Image number for columns: */
        private int seg_C;

        /* Image number for rows: */
        private int seg_R;

        /* Path to save Images: */
        private string img_save_path;

        /* Base name for the file: */
        private string file_name;

        /* Camera Settings: */
        public void Init_camera(int R, int C, string save_path, string name)
        {
            Image_count = 0;
            Image_number_R = 0;
            Image_number_C = 0;
            seg_C = C;
            seg_R = R;
            img_save_path = save_path;
            file_name = name;
        }

        /* Determin which Image number: */
        private string Image_prefix()
        {
            var prefix = "/" + file_name + Image_number_R + Image_number_C + ".jpg";
            Image_number_C++;
            if( Image_number_C >= seg_C)
            {
                Image_number_C = 0;
                Image_number_R++;
            }
            return prefix;
        }

        /* Take picture: */
        public int Take_picture()
        {
            if ((seg_C * seg_R) <= Image_count)
                return 1;                           //Error, asking for more photos than requrested at init.

            var prefix = Image_prefix();
            
            var image = new Mat();
            var capture = new VideoCapture();
            image = capture.QueryFrame();

            image.Save(img_save_path + prefix);

            Image_count++;

            return 0;
        }
    }

    /* All functions relating to Stitching given Images together: */
    class Image_stitcher
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

    /* All functions relating to Finding and Decoding 2D matrix barcodes: */
    class Barcode
    {
        /* Used to calculate lineraly spaced steps: */
        static double Linspace_step(double x1, double x2, int num)
        {
            double step = (x2 - x1) / (num - 1);
            return step;
        }

        /* Crops Image based on ROI. Returns cropped Image: */
        static Mat Crop_image(Mat input, Rectangle crop_region)
        {
            Image<Gray, Byte> buffer_im = input.ToImage<Gray, byte>();
            buffer_im.ROI = crop_region;
            Image<Gray, byte> cropped_im = buffer_im.Copy();
            CvInvoke.Threshold(cropped_im, cropped_im, 50, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);
            return cropped_im.Mat;
        }

        /* struct to hold return information of MinMax function: */
        struct MinMax_info
        {
            public double max_val;
            public Point top_left;
            public Point bottom_right;
            public bool is_none;
        };

        /* Finds barcode in a given Image:
         * Returns Mat containing barcode only. */
        public Mat Find_barcode(string img_path)
        {
            var img = CvInvoke.Imread(img_path, ImreadModes.Color);
            /*  Load in templates: */
            List<Mat> template = new List<Mat>();
            template.Add(CvInvoke.Imread("C:/Users/Kestutis/Documents/PSU/Images/Intel/barcode1.jpg", ImreadModes.Color));
            template.Add(CvInvoke.Imread("C:/Users/Kestutis/Documents/PSU/Images/Intel/barcode2.jpg", ImreadModes.Color));
            var resized = new Mat();
            var res = new Mat();
            /* Barcode location Information initilization: */
            MinMax_info found;
            found.is_none = true;
            found.max_val = 0;
            found.bottom_right = new Point(0, 0);
            found.top_left = new Point(0, 0);
            /* set up linspace loop for templates scaling: */
            double start_val = .2;
            double stop_val = 4;
            double step_val = Linspace_step(start_val, stop_val, 10);
            double min_val = 0; double max_val = 0; Point min_loc = new Point(); Point max_loc = new Point();
            /* Loop through scale increments to find barcode location: */
            for (int i = 0; i < template.Count; i++)
            {
                for (double scale = start_val; scale < stop_val; scale += step_val)
                {
                    CvInvoke.Resize(template[i], resized, new System.Drawing.Size(Convert.ToInt32(scale * template[i].Size.Width), Convert.ToInt32(scale * template[i].Size.Height)));
                    if (template[i].Size.Height < img.Size.Height && template[i].Size.Width < img.Size.Width)
                    {
                        CvInvoke.MatchTemplate(img, resized, res, TemplateMatchingType.CcoeffNormed);
                        CvInvoke.MinMaxLoc(res, ref min_val, ref max_val, ref min_loc, ref max_loc, null);
                        if (found.is_none == true || max_val > found.max_val)
                        {
                            found.is_none = false;
                            found.max_val = max_val;
                            found.top_left = max_loc;
                            found.bottom_right.X = max_loc.X + resized.Size.Width;
                            found.bottom_right.Y = max_loc.Y + resized.Size.Height;
                        }
                    }
                }
            }
            int offset = 10;
            Rectangle img_box = new Rectangle(found.top_left.X - offset, found.top_left.Y - offset, found.bottom_right.X - found.top_left.X + offset, found.bottom_right.Y - found.top_left.Y + offset);
            return Crop_image(img, img_box);
        }

        /* Decodes given barcode Image:
         * Returns decoded string if barcode can be read,
         * Otherwise returns empty string.*/
        public string Barcode_decoder(Mat img)
        {
            DmtxImageDecoder decoder = new DmtxImageDecoder();
            var list = decoder.DecodeImage(img.Bitmap, 1, new TimeSpan(0, 0, 1));
            if (list.Count > 0)
                return list[0];
            else
                return "";

        }

        /* Used for debugging the Barcode class: */
        private void Barcode_finding_run (string img_path)
        {
            /* Loading in image: */
            var img = new Mat();
            img = CvInvoke.Imread(img_path, ImreadModes.Color);
            /* Image of barcode: */
            var barcode = new Mat();


            barcode = Find_barcode(img_path);
            var barcode_string = Barcode_decoder(barcode);
            Console.WriteLine("Decoded Barcode is:" + barcode_string);


            /* View Barcode Image: */
            CvInvoke.NamedWindow("Barcode", NamedWindowType.Normal);
            CvInvoke.Imshow("Barcode", barcode); /*
            CvInvoke.NamedWindow("Given img", NamedWindowType.Normal);
            CvInvoke.Imshow("Given img", img); */
            CvInvoke.WaitKey();
            CvInvoke.DestroyAllWindows();

            barcode.Save("C:/Users/Kestutis/Documents/PSU/Images/Intel/barcode_test.jpg");

            /* For debugging purposes: */
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }

    class Object_detection
    {

    }


    /* Used for debugging interface between all Image_processing classes and a pseudo main: */
    class Testing_Program
    {
        static void Main(string[] args)
        {
            /* *********************************************************************************************************************************************************** */
            //Camera image capture:
            /* *********************************************************************************************************************************************************** */
            var seg_R = 3;              //number of rows
            var seg_C = 4;              //number of colums 
            var save_path = "C:/Users/Kestutis/Documents/PSU/Images/Image_stitching/camera_cap_test";     //path to images to stitch
            var file_name = "Testing_pic";

            var cam = new Camera_capture();
            cam.Init_camera(seg_R, seg_C, save_path, file_name);                                           //initilize camera parameters... Must do for each new CPU.

            /* Simulate taking pictures with a look and wait... normaly would just be picture function call: */
            for (int y = 0; y < seg_R; y++)
            {
                for (int x = 0; x < seg_C; x++)
                {
                    var cam_taken = cam.Take_picture();                     //Picture function call. Returns 0 if everything works, else returns 1.
                                                                            //Need to add save_path checking or save_path must be dynamicly created.

                    Console.WriteLine("Values: " + y + x);
                    Console.WriteLine("Press enter to Continue... Value returned: " + cam_taken);
                    Console.ReadLine();

                    /* An error occured, break loop: */
                    if (cam_taken == 1)
                    {
                        y = seg_R + 100;
                        x = seg_C + 100;
                    }
                }
            }

            /* For debugging purposes */
            Console.WriteLine("Press enter to continue to Image stitching section...");
            Console.ReadLine();



            /* *********************************************************************************************************************************************************** */
            // Image stitching: 
            /* *********************************************************************************************************************************************************** */
            var path_images = "C:/Users/Kestutis/Documents/PSU/Images/Image_stitching";     //path to images to stitch, Currently not in sync with camera function.
            file_name = "test";                                                             //base name of files to load in, Currently not in sync with camera function.

            var test_stitcher = new Image_stitcher();
            var images = test_stitcher.Load_images(path_images, seg_R, seg_C, file_name);
            var finished = test_stitcher.Stitching_images(images, seg_R, seg_C);
            
            // Check stitched image: 
            CvInvoke.NamedWindow("Image", NamedWindowType.Normal);
            CvInvoke.Imshow("Image", finished);
            CvInvoke.WaitKey();
            CvInvoke.DestroyAllWindows();

            /* For debugging purposes */
            Console.WriteLine("Press enter to continue to Barcode section...");
            Console.ReadLine();



            /* *********************************************************************************************************************************************************** */
            // Barcode Detection and Decoding: 
            /* *********************************************************************************************************************************************************** */
            var path = "C:/Users/Kestutis/Documents/PSU/Images/Intel/barcode.jpg";      //path of stitched image location
            var test_barcode = new Barcode();
            //var found_barcode = false;
            string testing;

            /* Checking each image for barcode... Far to slow is multiple images; additional second per image: */
            /*
            for(int i=0; i<images.Count; i++)
            {
                testing = test_barcode.Barcode_decoder(images[i]);
                if (testing != "")
                    found_barcode = true;
                if (found_barcode == true)
                {
                    Console.WriteLine("Testing:" + testing);
                    break;
                }
            } */
            
            testing = test_barcode.Barcode_decoder(test_barcode.Find_barcode(path));
            Console.WriteLine("Testing:" + testing); 

            /* Needs to be implemented somehow: */
            //if (testing != "")
            //Capture image of barcode location and try again:


            /* For debugging purposes: */
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
