/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu

This file will include all be the main hearder file.
It will include all #includes and #defines
*/

#ifndef COMMON_H
#define COMMON_H

#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_MotorShield.h>

#define SHIELD_ZERO_ADDRESS 0x00
#define SHIELD_ONE_ADDRESS 0x00
#define SHIELD_TWO_ADDRESS 0x00

#define X_AXIS_TOP_CHANNEL 1
#define X_AXIS_BOTTOM_CHANNEL 2
#define Z_AXIS_TOP_CHANNEL 1
#define Z_AXIS_BOTTOM_CHANNEL 2
#define Y_AXIS_CHANNEL 1



#define MOTOR_STEPS 200
#define MOTOR_SPEED 100

#define X_SWICH_PIN 2
#define Y_SWICH_PIN 3

#define NUMBER_MOTORS 5
#define NUMBER_SHIELD 3

#define STEP_TO_DEGREE_CONST  0.204

enum motors
{
    X_AXIS_TOP,
    X_AXIS_BOTTOM,
    Z_AXIS_TOP,
    Z_AXIS_BOTTOM,
    Y_AXIS
};

#endif /* COMMON_H */
