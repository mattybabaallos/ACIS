#######
'''
Template Matching:
Written by: Kestutis Saltonas
Last Update: 1/5/2018
Summary:    Given an image and templates to compare to, the program will determine if the templates are found 
            in the image and the number of each template found in the image. This information will then be stored
            in an array.
Issues:     Known issues are as follow:
            - Threshold value needs to be tested against faulty chips to see if false positives occure.
Notes:      Things to consider:
            - Currently all templates are of default name tempx (where x is a number).
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

#Path location for templates:
path = 'C:/Users/Kestutis/Documents/PSU/Images/New_images/templates'                       
img_chip =  cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel_chip.jpg', 1)    #image of chip back
img = cv2.cvtColor(img_chip, cv2.COLOR_BGR2GRAY)
img_temp = img_chip.copy()
img_count = 4               #number of templates to be used
threshold = 0.9             #threshold for template matching (percent match)
found_count = []            #array that holds found count of each template

#load in all templates:
images = ()
for x in range(1, img_count+1):
    filename = "temp" + str(x) + ".png"                             #name of loaded file
    img_to_add = cv2.imread(os.path.join(path,filename))            #location of file
    img_to_add = cv2.cvtColor(img_to_add, cv2.COLOR_BGR2GRAY)
    images_L = list(images)
    images_L.append(img_to_add)
    images = tuple(images_L)

#run the loop for each template
for x in range(0, img_count):
    if(x >= 0 and x<200):                   #change the mask color for each template run to avoid duplicate counts
        color = [200,0,255-x]
    if(x >= 200 and x<400):
        color = [0,0,255-x]
    if(x >= 400 and x<600):
        color = [255-x,0,0]
    pic = images[x]                         #select template
    w,h = pic.shape[::-1]
    res = cv2.matchTemplate(img, pic, eval('cv2.TM_CCOEFF_NORMED'))
    loc = np.where(res>= threshold)         #select points above threshold
    for pt in zip(*loc[::-1]):              #plot selected points, divided in half to avoid overlap and color fill
        if(x == 0):
            cv2.rectangle(img_temp, pt, (int(round(pt[0]+w/2)), int(round(pt[1]+h/2))), color, cv2.FILLED)
        if(x == 1):
            cv2.rectangle(img_temp, pt, (int(round(pt[0]+w/2)), int(round(pt[1]+h/2))), color, cv2.FILLED)
        if(x == 2):
            cv2.rectangle(img_temp, pt, (int(round(pt[0]+w/2)), int(round(pt[1]+h/2))), color, cv2.FILLED)
        if(x == 3):
            cv2.rectangle(img_temp, pt, (int(round(pt[0]+w/2)), int(round(pt[1]+h/2))), color, cv2.FILLED)
    color_array1 = np.array(color, dtype="uint16")
    color_array2 = np.array(color, dtype="uint16")
    mask = cv2.inRange(img_temp, color_array1, color_array2)                        #mask based off selected color in range (currently only one color is used) 
    _,contours,_ = cv2.findContours(mask, cv2.RETR_LIST, cv2.CHAIN_APPROX_NONE)     #find all non masked shapes
    value = len(contours)                                                           #count instances of shapes
    found_count.append(value)                                                       #store value


#Check Results: print number of found templates and image
for x in range(0, img_count):
    print("found_count_",x,": ",found_count[x],)                
cv2.namedWindow("Image:", cv2.WINDOW_NORMAL)
cv2.imshow("Image:", img_temp)

cv2.waitKey(0)
cv2.destroyAllWindows()
