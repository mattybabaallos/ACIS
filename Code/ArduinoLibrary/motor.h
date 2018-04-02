/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu

*/

#ifndef MOTOR_H
#define MOTOR_H

#include "common.h"

class motor
{
  public:
	motor(Adafruit_StepperMotor * stepper, unsigned int max_distance);
	int move_forward(unsigned int mm);
	int move_backword(unsigned int mm);
	int stop();
	int home();

  protected:
	int get_steps(unsigned int mm);
	unsigned int current_position;
	unsigned int max_distance;// in mm
	unsigned int min_distance;// in mm
	Adafruit_StepperMotor * m_motor;
};

#endif /* MOTOR_H */
