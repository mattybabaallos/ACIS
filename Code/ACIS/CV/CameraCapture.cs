using Data;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace CV
{
    public class CameraCapture
    {
        
        /* Cam index in use: */
        private int Cam_Num;

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

        /* Mask out edges of picture and keep focused middle */
        private Rectangle image_mask;

        /* Image resolution: */
        private Size img_res;      //resolution of cam

        /* Constructor Top */
        // private VideoCapture Top_capture;

        /* Constructor Top */
        // private VideoCapture Bottom_capture;

        public VideoCapture Capture { get; set; }

        public int TopIndex { get; private set; }
        public int BottomIndex { get; private set; }

        public string Filepath => img_save_path + Image_prefix_filename();

        /* Crops Image based on ROI. Returns cropped Image: */
        static Mat Crop_image(Mat input, Rectangle crop_region)
        {
            Image<Bgr, Byte> buffer_im = input.ToImage<Bgr, Byte>();
            Console.WriteLine(buffer_im.Size);
            Console.WriteLine(input.Size);
            buffer_im.ROI = crop_region;
            Image<Bgr, Byte> cropped_im = buffer_im.Copy();
            return cropped_im.Mat;
        }

        public void CallobrateCameras()
        {
            TopIndex = FindCameraIndex(Devices.XAxisTopMotor);                   //1
            BottomIndex = FindCameraIndex(Devices.XAxisBottomMotor);             //2
            
            if(TopIndex < 0|| BottomIndex < 0)
            {
                /* Camera Callobration Error */
            }
        }

        private int FindCameraIndex(Devices camera)
        {
            /* If looking for: Top-0. Bottom-1.  */
            var cam_barcode = new Barcode();
            string cam_decoded;

            for (int i=0; i<4; i++)
            {
                try
                {
                    VideoCapture temp_capture = new VideoCapture(i);
                    temp_capture.SetCaptureProperty(CapProp.FrameWidth, 1920);
                    temp_capture.SetCaptureProperty(CapProp.FrameHeight, 1080);

                    var image = new Mat();
                    for (int k = 0; k < 1; ++k)
                    {
                        //image = temp_capture.QueryFrame();
                        temp_capture.Read(image);
                    }
                    temp_capture.Dispose();

                    /* Try to decode original image before datamatrix location (saves time): */
                    cam_decoded = cam_barcode.Barcode_decoder(image);
                    if (cam_decoded == "Top" && camera == Devices.XAxisTopMotor)
                    {
                        return i;
                    }
                    else if (cam_decoded == "Bottom" && camera == Devices.XAxisBottomMotor)
                    {
                        return i;
                    }


                    var imgMAT = cam_barcode.Find_TopBot_barcode(image);

                    CvInvoke.Imshow("imgMAT", imgMAT);
                    CvInvoke.Imshow("img", image);
                    CvInvoke.WaitKey();
                    CvInvoke.DestroyAllWindows();

                    cam_decoded = cam_barcode.Barcode_decoder(imgMAT);

                    if (cam_decoded == "Top" && camera == Devices.XAxisTopMotor)
                    {
                        return i;
                    }
                    else if (cam_decoded == "Bottom" && camera == Devices.XAxisBottomMotor)
                    {
                        return i;
                    }
                }
                catch
                {
                    //Console.WriteLine("No cam at num: " + i);
                } 
            }
            return -1;

        }

        private Bitmap GetTemplate(Devices camera)
        {
            if (camera == Devices.XAxisTopMotor)
                return Properties.Resources.Top;
            else if (camera == Devices.XAxisBottomMotor)
                return Properties.Resources.Bottom;
            else
                return null;
        }

        /* Camera Settings: */
        public void Init_camera(int R, int C, string save_path, string name)
        {
          //  Top_capture = new VideoCapture(Top_index);
          //  Bottom_capture = new VideoCapture(Bottom_index);


            Image_count = 0;
            Image_number_R = 0;
            Image_number_C = 0;
            seg_C = C;
            seg_R = R;
            img_save_path = save_path;
            file_name = name;
            img_res = new Size(1920, 1080);
            var image_mask = new Rectangle(img_res.Width / 16, img_res.Height / 16, Convert.ToInt32(Math.Round(0.95 * img_res.Width)), Convert.ToInt32(Math.Round(0.95 * img_res.Height)));

           // Top_capture.SetCaptureProperty(CapProp.FrameWidth, 1920);
           // Top_capture.SetCaptureProperty(CapProp.FrameHeight, 1080);
           // Bottom_capture.SetCaptureProperty(CapProp.FrameWidth, 1920);
           // Bottom_capture.SetCaptureProperty(CapProp.FrameHeight, 1080);
        }

        /* Determin which Image number: */
        private string Image_prefix(int cam_num)
        {
            /* cam_num:  0-Top, 1-Bottom */
            var prefix = "";
            if (cam_num == 0)
            {
                prefix = "/" + "Top" + file_name + Image_number_R + Image_number_C + ".jpg";
            }
            if (cam_num == 1)
            {
                prefix = "/" + "Bottom" + file_name + Image_number_R + Image_number_C + ".jpg";
                Image_number_C++;
                if (Image_number_C >= seg_C)
                {
                    Image_number_C = 0;
                    Image_number_R++;
                }
            }


            return prefix;
        }

        private string Image_prefix_filename()
        {
            var prefix = "";

            if (Cam_Num == 0)
            {
                prefix = @"\" + "Top" + file_name + Image_number_R + Image_number_C + ".jpg";
            }
            if (Cam_Num == 1)
            {
                prefix = @"\" + "Bottom" + file_name + Image_number_R + Image_number_C + ".jpg";
            }
            return prefix;
        }

        /* Take picture: */
        public int TakePicture(int cameraNumber)
        {
            try
            {
                if ((seg_C * seg_R) < Image_count)
                    return 1;                           //Error, asking for more photos than requrested at init.

                var prefix = Image_prefix(cameraNumber);

                var image = new Mat();

                for (int i = 0; i < 4; ++i)
                {
                    image = Capture.QueryFrame();
                }
                Capture.Read(image);

                image = Crop_image(image, image_mask);

                image.Save(img_save_path + prefix);

                return 0;
            }
            catch
            {
                return 1;
            }
        }
    }
}