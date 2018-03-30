/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu

*/

#ifndef MOTOR_H
#define MOTOR_H

#include "common.h"

class motor
{
  public:
	motor(Adafruit_StepperMotor * stepper);
	int move();
	int stop();
	int home();

  protected:
	Adafruit_StepperMotor * m_motor;
};

#endif /* MOTOR_H */