/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#ifndef ACIS_H
#define ACIS_H

#include "common.h"
#include "motor.h"
#include "led.h"

class acis
{
public:
	acis(Adafruit_MotorShield *shield_0, Adafruit_MotorShield *shield_1, led *top_leds, led * bottom_leds);
	int init();
	int move_forward(byte motor_id, long data);
	int move_backward(byte motor_id, long data);
	int stop(byte motor_id);
	int home(byte motor_id);
	int leds_off(byte led_id);
	int leds_on(byte led_id, long hex_color);
	int process(byte *buffer);
	int send_back(byte *buffer, byte device, byte function, long data, byte error_code);

private:
	int decode(byte *buffer, byte &device, byte &function, long &data);
	Adafruit_MotorShield *m_shield_0;
	Adafruit_MotorShield *m_shield_1;
	motor motors[NUMBER_MOTORS];
	motor *working_motor;
	led *m_leds;
};

#endif /* ACIS_H */
