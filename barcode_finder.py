
import pylibdmtx
import cv2
import numpy as np
import os
from matplotlib import pyplot as plt
from pylibdmtx.pylibdmtx import decode

img_color = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel\\barcode.jpg', 1)    #picture with the barcode in it.
template1 = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel\\barcode2.jpg', 1)
template2 = cv2.imread('C:\\Users\\Kestutis\\Documents\\PSU\\Images\\Intel\\barcode1.jpg', 1)
gray = cv2.cvtColor(img_color, cv2.COLOR_BGR2GRAY)
template_gray1 = cv2.cvtColor(template1, cv2.COLOR_BGR2GRAY)
template_gray2 = cv2.cvtColor(template2, cv2.COLOR_BGR2GRAY)
temp = img_color.copy()

found = None
H, W = temp.shape[:2]
tH1, tW1 = template_gray1.shape[:2]
tH2, tW2 = template_gray2.shape[:2]


pictures = ['template_gray1', 'template_gray2']
image = ['gray']
for img2 in image:
    for pic in pictures:
        for scale in np.linspace(.2, 2, 30)[::-1]:
            if pic == 'template_gray1':
                resized = cv2.resize(eval(pic), (0,0), fx=(scale), fy=(scale))
            if pic == 'template_gray2':
                resized = cv2.resize(eval(pic), (0,0), fx=(scale), fy=(scale))
            r = template_gray1.shape[1] / float(resized.shape[1])
            #print("ratio:",r)
            w,h = resized.shape[::-1]
            res = cv2.matchTemplate(eval(img2), resized, eval('cv2.TM_CCOEFF_NORMED'))
            min_val, max_val, min_loc, max_loc = cv2.minMaxLoc(res)
            top_left = max_loc
            bottom_right = (top_left[0] +w, top_left[1]+h)
            #print("max_value:",max_val)
            if found is None or max_val > found[0]:
                found = (max_val, top_left, bottom_right)

#print(found[0])
#print(found[1], found[2])

offset = 0
img_box = gray[found[1][1]-offset:found[2][1]+offset, found[1][0]-offset:found[2][0]+offset]
(thresh, img_box) = cv2.threshold(img_box, 100, 255, cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)
cv2.imshow("img_box", img_box)
code = decode(img_box)
print(code)

cv2.rectangle(img_color, found[1], found[2], 255, 2)
cv2.namedWindow('img', cv2.WINDOW_NORMAL)
cv2.imshow('img', img_color)
#plt.imshow(img_color), plt.show()

cv2.waitKey(0)
cv2.destroyAllWindows()