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
#include <avr/interrupt.h>
#include <Wire.h>
#include "Adafruit_MotorShield.h"
#include "Adafruit_NeoPixel.h"

#define SHIELD_ZERO_ADDRESS 0x61
#define SHIELD_ONE_ADDRESS 0x62
#define SHIELD_TWO_ADDRESS 0x00

#define X_AXIS_TOP_CHANNEL 1
#define X_AXIS_BOTTOM_CHANNEL 2
#define Y_AXIS_CHANNEL 2

#define MOTOR_STEPS 200
#define MOTOR_SPEED 100

#define X_TOP_SWICH_PIN 2
#define X_BOTTOM_SWICH_PIN 3
#define Y_SWICH_PIN 4
#define Y_AXIS_CPU_SWITCH_PIN 5

#define LEDS_PIN 9

#define NUMBER_MOTORS 5
#define NUMBER_SHIELD 3
#define NUMBER_SWITCHES 6
#define NUMBER_LEDS 144

#define MAX_X_TOP_LENGTH 330
#define MAX_X_BOTTOM_LENGTH 330
#define MAX_Y_LENGTH 315

#define STEP_TO_DEGREE_CONST (0.11344640138 * 1.8)

#define BUFFER_SIZE 6

enum Devices
{
    X_AXIS_TOP_MOTOR,
    X_AXIS_BOTTOM_MOTOR,
    Y_AXIS_MOTOR,
    TOP_LEDS,
    BOTTOM_LEDS,
    Y_AXIS_CPU_SWITCH,
    DOOR_SWITCH,
    BOTTOM_SWITCH,
};

enum functions
{
    HOME_STEPPER,
    MOVE_STEPPER_FORWARD,
    MOVE_STEPPER_BACKWARD,
    STOP_STEPPER,
    TURN_ON_UPDATE_LEDS,
    TURN_OFF_LEDS
};

enum errors
{
    SUCCESS = 0,
    RECEIVED_FEWER_THAN_TWO_BYTES = -1,
    INVALID_OPERATION = -2,
    INVALID_DEVICE = -3,
    COULD_NOT_PERFORM_OPERATION = -4,
    COULD_NOT_DECODE_BYTES = -5,
    COULD_NOT_PROCESS_BUFFER = -6,
    STOP_INTERRUPT = -7
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
