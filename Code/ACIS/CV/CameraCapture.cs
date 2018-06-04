using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV
{
    public class CameraCapture
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

        /* Mask out edges of picture and keep focused middle */
        private Rectangle image_mask;

        /* Image resolution: */
        private Size img_res;      //resolution of cam

        /* Constructor */
        private VideoCapture capture = new VideoCapture(1);

        public string FileName => Image_prefix_filename();
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
            img_res = new Size(1920, 1080);
            var image_mask = new Rectangle(img_res.Width / 16, img_res.Height / 16, Convert.ToInt32(Math.Round(0.95 * img_res.Width)), Convert.ToInt32(Math.Round(0.95 * img_res.Height)));

            capture.SetCaptureProperty(CapProp.FrameWidth, 1920);
            capture.SetCaptureProperty(CapProp.FrameHeight, 1080);
        }

        /* Determin which Image number: */
        private string Image_prefix()
        {
            var prefix = "/" + file_name + Image_number_R + Image_number_C + ".jpg";
            Image_number_C++;
            if (Image_number_C >= seg_C)
            {
                Image_number_C = 0;
                Image_number_R++;
            }
            return prefix;
        }

        private string Image_prefix_filename()
        {
            var prefix = @"\" + file_name + Image_number_R + Image_number_C + ".jpg";
            return prefix;
        }

        /* Take picture: */
        public int Take_picture()
        {
            try
            {

                if ((seg_C * seg_R) < Image_count)
                    return 1;                           //Error, asking for more photos than requrested at init.

                var prefix = Image_prefix();

                var image = new Mat();
               // var capture = new VideoCapture();

                for (int i = 0; i < 5; ++i)
                {
                    capture.QueryFrame();
                }

                image = capture.QueryFrame();
               
                image = Crop_image(image, image_mask);

                image.Save(img_save_path + prefix);

                //Image_count++;

                return 0;
            }
            catch
            {
                return 1;
            }
        }
    }
}
