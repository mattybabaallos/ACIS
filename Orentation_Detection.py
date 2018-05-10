#######
'''
Orientation Detection and Adjustment:
Written by: Kestutis Saltonas
Last Update: 1/5/2018
Summary:    Given images, the program will determine if it is the front of the chip or the back of the chip, 
            rotate the images to the correct orientation, and label both images before saving them. This 
            program requires a template of the intel logo.
Issues:     Known issues are as follow:
            - no way to distinguish between chip images that differ in size (x-axis is larger than y-axis)
            vs (x-axis is smaller than y-axis). currently an "odd" flag is being used to differ between the
            two... an automated system would be better.
            - program depends on intel logo, currently a method to adjust the size of the logo is implemented,
            however if the ratios are diffrent between image templates, this will be an issue. If logo size is
            standard across all templates or ratio size can be adjusted for each template then problem is solved.
Notes:      Things to consider:
            - if output is not as expected, check values of size_x, size_y, and odd.
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
import os
from matplotlib import pyplot as plt

#Path location to save the newly orintated image:
path = 'C:/Users/Kestutis/Documents/PSU/Images/New_images'

#intel logo size estimation: 
size_x = 0.38   # 0.38 for img_chip and img_chip_odd, 0.38 for img_chip_HR
size_y = 0.29   # 0.25 for img_chip and img_chip_odd, 0.29 for img_chip_HR

#load images:
img_intel = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel_logo.png', 0)
img_chip = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Chip_front.jpg', 0)
img_chip_odd =  cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Chip_front_odd.jpg', 0)
img_chip_HR = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Image_stitching\\Chip_front_HR.jpg', 0)
img_back = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Chip_back.jpg', 0)
img_chip_color = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Chip_front.jpg', 1)
img_chip_odd_color =  cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Chip_front_odd.jpg', 1)
img_chip_HR_color = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Image_stitching\\Chip_front_HR.jpg', 1)
img_back_color = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Chip_back.jpg', 1)

#######
#switch between images if needed: (for testing)
#HR chip:
img_chip = img_chip_HR
img_chip_color = img_chip_HR_color
new_chip = img_chip_HR_color.copy()

#Regular chip:
#new_chip = img_chip_color.copy()
new_back = img_back_color.copy()

#odd chip:
#img_chip = img_chip_odd
#img_chip_color = img_chip_odd_color
#new_chip = img_chip_odd_color.copy()
#######

odd = 0                                         #odd = 0 if X-axis is smaller than Y-axis. Otherwise odd = 1
threshold = 0.85                                #matching threshold. recommended values of 0.8 to 0.9
match = 0                                       #if results are higher than threshold, value is toggled
degrees = 0                                     #degrees to rotate image. 0, 90, 180, 270.
wrong = 0                                       #degree patern used and ended up being incorrect (testing)
found = 0                                       #number of matching results. Should be 1 after program runs (testing)

rows,cols = img_chip.shape
#print("rows:",rows,"cols:",cols,)
if(rows > cols):
    temp = cols
    cols = rows
    rows = temp
rows2, cols2 =img_intel.shape
#resize intel logo image:
img_intel = cv2.resize(img_intel, (0,0), fx=(rows*size_y)/rows2, fy=(cols*size_x)/cols2) 
#convert image to black and white:
(thresh, img_intel) = cv2.threshold(img_intel, 128, 255, cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)

#######
#check images are correct: 
#cv2.imshow("chip", img_chip)
#cv2.imshow("chip_colored", new_chip)
#cv2.imshow("img_back", img_back)
#cv2.imshow("img_back_colored", img_back_color)
#######

#create intel image templates, each rotated 90 degrees:
rows, cols = img_intel.shape
#print("rows:",rows,"cols:",cols,)
if(rows > cols):
    small_value = cols
    cols = rows
else:
    small_value = rows
    rows = cols
#90 degrees:
M_90 = cv2.getRotationMatrix2D((cols/2,rows/2),90,1)
img_intel_90 = cv2.warpAffine(img_intel,M_90,(cols, rows))
img_intel_90 = img_intel_90[0:cols, 0:small_value]
#180 degrees:
M_180 = cv2.getRotationMatrix2D((cols/2,rows/2),180,1)
img_intel_180 = cv2.warpAffine(img_intel,M_180,(cols,rows))
img_intel_180 = img_intel_180[(cols-small_value):cols, 0:cols]
#270 degrees:
M_270 = cv2.getRotationMatrix2D((cols/2,rows/2),270,1)
img_intel_270 = cv2.warpAffine(img_intel,M_270,(cols,rows))
img_intel_270 = img_intel_270[0:cols, (cols-small_value):cols]

#######
#check images are correct:
#cv2.imshow("Intel", img_intel)
#cv2.imshow("Intel_90", img_intel_90)
#cv2.imshow("Intel_180", img_intel_180)
#cv2.imshow("Intel_270", img_intel_270)
########

pictures = ['img_intel', 'img_intel_90', 'img_intel_180', 'img_intel_270']
image = ['img_chip', 'img_back']
for img2 in image:
    for pic in pictures:
        w,h = eval(pic).shape[::-1]
        res = cv2.matchTemplate(eval(img2), eval(pic), eval('cv2.TM_CCOEFF_NORMED'))
        min_val, max_val, min_loc, max_loc = cv2.minMaxLoc(res)
        top_left = max_loc
        bottom_right = (top_left[0] +w, top_left[1]+h)
        if(max_val >= threshold):
            if(img2 == 'img_chip'):
                cv2.rectangle(img_chip_color, top_left, bottom_right, 255, 2)
            if(img2 == 'img_back'):
                cv2.rectangle(img_back_color, top_left, bottom_right, 255, 2)
            match = 1
            if(pic == 'img_intel_90'):
                degrees = 270
            if(pic == 'img_intel_180'):
                degrees = 180
            if(pic == 'img_intel_270'):
                degrees = 90
    #Check which images matched and did not (testing):
        if(match == 0):
            print("No match! Degrees:",wrong,)
            wrong+=90
        if(match == 1):
            print("Match! Degrees:",wrong, " Img:",img2,)
            wrong+=90
            found+=1
            match = 0
            if(degrees == 0):
                img = img_intel
            if(degrees == 270):
                img = img_intel_90
            if(degrees == 180):
                img = img_intel_180
            if(degrees == 90):
                img = img_intel_270
        if(wrong > 270):
            wrong = 0

#rotate new_chip image based on found degrees:
new_image = ['new_chip', 'new_back']
for new_img in new_image:
    #new = eval(new_img)
    if (new_img == 'new_chip'):
        rows, cols = img_chip.shape 
    if (new_img == 'new_back'):
        rows, cols = img_back.shape
    if(rows > cols):
        small_value = cols
        cols = rows
    else:
        small_value = rows
        rows = cols 
    M = cv2.getRotationMatrix2D((cols/2,rows/2),degrees,1)
    new = cv2.warpAffine(eval(new_img),M,(cols, rows))

    #adjust the new_chip image after rotate:
    #odd is 0 if X-axis is larger than Y-axis for image, otherwise odd is 1.
    if(degrees == 0):
        if(odd == 0):
            new = new[0:small_value, 0:cols]
        else:
            new = new[0:cols, 0:small_value]
    if(degrees == 90):
        if(odd == 0):
            new = new[(cols-small_value):cols, 0:cols]
        else:
            new = new[0:cols, 0:small_value]
    if(degrees == 180):
        if(odd == 0):
            new = new[(cols-small_value):cols, 0:cols]
        else:
            new = new[0:cols, (cols-small_value):cols]
    if(degrees == 270):
        if(odd == 0):
            new = new[0:small_value, 0:cols]
        else:
            new = new[0:cols, (cols-small_value):cols]
    if (new_img == 'new_chip'):
        new_chip = new 
    if (new_img == 'new_back'):
        new_back = new

#if a match is found, display it:
print("found:",found,)                       #check to make sure only one match (testing)
if(found >= 1):
    #plt.subplot(131), plt.imshow(img, cmap = 'gray')
    #plt.title('Matching Result')
    plt.subplot(121), plt.imshow(img_chip_color, cmap = 'gray')
    plt.title('Detected Point')
    plt.subplot(122), plt.imshow(new_chip, cmap = 'gray')
    plt.title('New Chip')
    plt.suptitle('cv.TM_CCOEFF_NORMED')
    plt.show()
    cv2.imwrite(os.path.join(path, 'New_Chip.png'), new_chip)
    cv2.imwrite(os.path.join(path, 'New_Back.png'), new_back)

cv2.waitKey(0)
cv2.destroyAllWindows()
