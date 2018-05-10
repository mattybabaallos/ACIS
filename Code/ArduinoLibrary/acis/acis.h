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
	acis(Adafruit_MotorShield *shield_0, Adafruit_MotorShield *shield_1, Adafruit_MotorShield *shield_2,  led * leds);
	int init();
	int move_forward(unsigned int motor_id, unsigned int mm);
	int move_backward(unsigned int motor_id, unsigned int mm);
	int stop(unsigned int motor_id);
	int home(unsigned int motor_id);
	int process(unsigned char *buffer);
	int send_back(unsigned char *buffer, unsigned int device, unsigned int op, unsigned int status_code, unsigned int new_state);

  private:
	int decode(unsigned char *buffer, unsigned int &device, unsigned int &function, unsigned int &mm);
	Adafruit_MotorShield *m_shield_0;
	Adafruit_MotorShield *m_shield_1;
	Adafruit_MotorShield *m_shield_2;
	motor motors[NUMBER_MOTORS];
	motor *working_motor;
	led * m_leds;
};

#endif /* ACIS_H */
