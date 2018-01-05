#######
'''
Camera Simulation and Image Stitching:
Written by: Kestutis Saltonas
Last Update: 1/5/2018
Summary:    Given an image, the program will segment the image as if taken by the camera in the device. These
            segmented images will then be saved at a specified path location and stored for later useage.
            The second part of the program will stitch the images together to form a single high-res picture.
Issues:     Known issues are as follow:
            - Currently default stitching class parameters are being run, this causes the image stitching to take
            a long time.
            - A cropping function has not yet been implemented to get ride of the padding around the image.
Notes:      Things to consider:
            - If the resolution of the image is to low, the image stitching class will not have enough points to
            match and an "not enough images" error will be seen. This will likly be fixed when the stitching 
            parameters are altered.
            - stitching with anything below 30% overlap is not advised, results are not good and often end in errors.
            - anything commnented with (testing) is used for testing purposes.
Version of Software:
            - Python 3.6
            - Matplotlib 2.1.1
            - Opencv 3.4.0.12
            - Numpy 1.13.3
'''
#######
import cv2
import numpy as np
from matplotlib import pylab as plt
import os

seg_R = 4                           #number of rows
seg_C = 5                           #number of colums

#part 1: image segmenter, simulates camera results.
def segment_images():
    #Path the images will be saved and variables for the image to be similarted as captured:
    path =  'C:/Users/Kestutis/Documents/PSU/Images/Image_stitching'
    img_chip_HR_color = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\New_images\\New_Chip.png', 1)
    img = cv2.cvtColor(img_chip_HR_color, cv2.COLOR_BGR2GRAY)
    overlap = 0.4                       #percent overlap between the images
    WHITE = [255,255,255]               #padding color default as white

    pic = dict()                        #variable to hold picture segments for writting
    for y in range(0, seg_C):
        pic[y] = dict()

    rows,cols = img.shape

    #add a padding layer around image:
    padding_R = (rows * overlap)/4
    padding_C = (cols * overlap)/4
    padding_R = int(round(padding_R))
    padding_C = int(round(padding_C))
    img = cv2.copyMakeBorder(img, padding_R, padding_R, padding_C, padding_C, cv2.BORDER_CONSTANT, value = WHITE)

    rows,cols = img.shape

    #calculate overlap:
    overlap_R = (rows * overlap)/seg_R
    overlap_C = (cols * overlap)/seg_C
    overlap_R = int(round(overlap_R))
    overlap_C = int(round(overlap_C))

    #calculate segment sizes:
    row = rows/seg_R
    col = cols/seg_C
    row = int(round(row))
    col = int(round(col))

    #take apart the loaded image based on the segments of rows and colums:
    for y in range(0, seg_R):
        for x in range(0, seg_C):
            upper_R = ((1+y)*row)+overlap_R
            upper_C = ((1+x)*col)+overlap_C
            if(upper_C > cols):
                upper_C = cols
            if(upper_R > rows):
                upper_R = rows
            if(x != 0):
                lower_C = overlap_C
            else:
                lower_C = 0
            if(y != 0):
                lower_R = overlap_R
            else:
                lower_R = 0
            pic[y][x] = img[(row*y)-(lower_R):upper_R,(col*x)-(lower_C):upper_C]
            filename = "img_" + str(y) + str(x) + ".png"                                #name the files
            cv2.imwrite(os.path.join(path, filename), pic[y][x])                        #store at path location
    return(0)


#part 2: image stitcher, stitches camera pictures together.
def stitch_images():
    #image location path:
    path =  'C:/Users/Kestutis/Documents/PSU/Images/Image_stitching'
    #load and store images into a tupel fom path location
    images = ()
    for y in range(0, seg_R):
        for x in range(0, seg_C):
            filename = "img_" + str(y) + str(x) + ".png"                    #name of loaded file
            img_to_add = cv2.imread(os.path.join(path,filename))            #location of file
            images_L = list(images)
            images_L.append(img_to_add)
            images = tuple(images_L)

    #create a stitcher using cv stitcher class:
    stitcher = cv2.createStitcher(False)
    #stitch all images together:
    result = stitcher.stitch(images)
    #view results (testing):
    #print(result[0])
    if(result[0] == 0):
        cv2.namedWindow("result", cv2.WINDOW_NORMAL)
        cv2.imshow("result", result[1])
    return(result[0])

#main function call:
def main():
    #if (segment_images() != 0):
    #    print("error!")
    res = stitch_images()
    if (res == 0):
        print("Done!")
    elif(res == 1):
        print("ERR_NEED_MORE_IMGS")
    elif(res == 2):
        print("ERR_HOMOGRAPHY_EST_FAIL")
    elif(res == 3):
        print("ERR_CAMERA_PARAMS_ADJUST_FAIL")


#call main:
if __name__ == "__main__":
    main()


cv2.waitKey(0)
cv2.destroyAllWindows()
