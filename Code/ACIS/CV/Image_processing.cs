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
