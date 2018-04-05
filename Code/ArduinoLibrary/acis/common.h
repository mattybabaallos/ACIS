/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu

This file will include all be the main hearder file.
It will include all #includes and #defines
*/

#ifndef COMMON_H
#define COMMON_H

#include <Arduino.h>
#include <Wire.h>
#include "Adafruit_MotorShield.h"

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

#define MAX_X_TOP_LENGTH 3
#define MAX_X_BOTTOM_LENGTH 3
#define MAX_Z_TOP_LENGTH 3
#define MAX_Z_BOTTOM_LENGTH 3
#define MAX_Y_LENGTH 3

#define STEP_TO_DEGREE_CONST 0.204

#define BYTES_TO_READ 2










enum motors
{
    X_AXIS_TOP,
    X_AXIS_BOTTOM,
    Z_AXIS_TOP,
    Z_AXIS_BOTTOM,
    Y_AXIS
};

enum functions
{
    HOME,
    MOVE_FORWARD,
    MOVE_BACKWARD,
    STOP
};

enum errors
{
    SUCCESS = 0,
    RECEIVED_FEWER_THAN_TWO_BYTES = -1,
    INVALID_OPERATION = -2,
    INVALID_DEVICE = -3,
    COULD_NOT_PERFORM_OPERATION = -4,
    COULD_NOT_DECODE_BYTES = -5,
};

/*******************************************************************************
 * Create a bit mask for a given range of bits. start, end. (lsb,msb).
 *  - start   == int, Which bit from bit 0 to start the mask.
 *  - end     == int, Which bit greater than start to end the mask.
 *  - resMask == int, Where the resulting bit mask will be placed
 *  Example: start = 4, end = 9 and type size 32 bits [0:32],
 *           Resulting mask will be 1's on bits [5:9], the rest will be 0.
 ******************************************************************************/
inline uint16_t create_mask(int start, int end)
{
    int i;
    uint16_t mask = 0;
    uint16_t one = 1; // used because default magic # is int

    if (start > end)
    {
        //TODO: print error message
        //noerr_msg("create_mask: start > end, no mask was generated.");
    }

    for (i = start; i <= end; ++i)
    {
        mask |= (one << i);
    }
    return mask;
} /* end create_mask */

#endif /* COMMON_H */
