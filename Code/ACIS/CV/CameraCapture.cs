using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
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

        public string FileName => Image_prefix_filename();

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
            if (Image_number_C >= seg_C)
            {
                Image_number_C = 0;
                Image_number_R++;
            }
            return prefix;
        }

        private string Image_prefix_filename()
        {
            var prefix = "/" + file_name + Image_number_R + Image_number_C + ".jpg";
            return prefix;
        }

        /* Take picture: */
        public int Take_picture()
        {
            try
            {

                if ((seg_C * seg_R) <= Image_count)
                    return 1;                           //Error, asking for more photos than requrested at init.

                var prefix = Image_prefix();

                var image = new Mat();
                var capture = new VideoCapture();

                for (int i = 0; i < 5; ++i)
                {
                    capture.QueryFrame();
                }

                image = capture.QueryFrame();

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
