#######
'''
Barcode detection and reading V1:
Written by: Kestutis Saltonas
Last Update: 2/9/2018
Summary:    Given an image, the program will locate possible locations of the barcode and scan them. Once the
            barcode is found it will decode and extract the code. The primary use of this function is for 
            finding/reading datamatrix codes.
Notes:      Things to consider:
            - The picture quality needs to be high enough that the barcode can be read.
Version of Software:
            - Python 3.6
            - Matplotlib 2.1.1
            - Opencv 3.4.0.12
            - Numpy 1.13.3
            - pylibdmtx
'''
#######

import pylibdmtx
import cv2
import numpy as np
import os
from matplotlib import pyplot as plt
from pylibdmtx.pylibdmtx import decode

img_color = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel\\barcode.jpg', 1)    #picture with the barcode in it.
gray = cv2.cvtColor(img_color, cv2.COLOR_BGR2GRAY)
temp = img_color.copy()

# compute the Scharr gradient magnitude representation of the images
# in both the x and y direction
gradX = cv2.Sobel(gray, ddepth = cv2.cv2.CV_32F, dx = 1, dy = 0, ksize = -1)
gradY = cv2.Sobel(gray, ddepth = cv2.cv2.CV_32F, dx = 0, dy = 1, ksize = -1)
 
# subtract the y-gradient from the x-gradient
gradient2 = cv2.subtract(gradX, gradY)
gradient = cv2.convertScaleAbs(gradient2)
 
# blur and threshold the image
#blurred = cv2.blur(gradient, (4, 4))
(_, thresh) = cv2.threshold(gradient, 235, 255, cv2.THRESH_BINARY)

# construct a closing kernel and apply it to the thresholded image
kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))                                          #change this
closed = cv2.morphologyEx(thresh, cv2.MORPH_CLOSE, kernel)
 
# perform a series of erosions and dilations
closed = cv2.erode(closed, None, iterations = 5)
closed = cv2.dilate(closed, None, iterations = 30)
 
# find the contours in the thresholded image
(_,cnts, _) = cv2.findContours(closed.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

if len(cnts) == 0:
    print("none")

if len(cnts) != 0:
    c = sorted(cnts, key = cv2.contourArea, reverse = True)[0]
    rect = cv2.minAreaRect(c)
    box = np.int0(cv2.boxPoints(rect))
    print(box)
    cv2.drawContours(img_color, [box], -1, (0, 0, 255), 2)
img = temp[int(box[1][1]):int(box[0][1]),int(box[0][0]):int(box[3][0])]

barcode = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel\\chip.jpg', 1)
#barcode = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\barcode_test4.png', 0)
img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
(thresh, img) = cv2.threshold(img, 100, 255, cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)

cv2.namedWindow('img', cv2.WINDOW_NORMAL)
cv2.imshow('img', closed)
cv2.namedWindow('img2', cv2.WINDOW_NORMAL)
cv2.imshow('img2', img_color)
#cv2.imshow('temp', template)
#plt.imshow(img_color[:,:,[2,1,0]]), plt.show()

cv2.waitKey(0)
cv2.destroyAllWindows()

code = decode(img)
print(code)