/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#ifndef ACIS_H
#define ACIS_H

#include "common.h"
#include "motor.h"

class acis
{
  public:
	acis();
	~acis();
	int move();
	int stop(int motor_id);

  private:
	Adafruit_MotorShield shield_0;
	Adafruit_MotorShield shield_1;
	Adafruit_MotorShield shield_2;
	motor motors[NUMBER_MOTORS];
	motor * working_motor;
};

#endif /* ACIS_H */
