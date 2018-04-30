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
	motor();
	int init_motor(Adafruit_StepperMotor *stepper, int max_distance);
	int move_forward(float mm);
	int move_backward(float mm);
	int stop();
	int home();

protected:
	float get_steps(float mm);
	float get_mm(float steps);
	int step(int steps, int direction, int style);
	bool m_stop;
	float m_current_position;
	float m_max_distance; // in mm
	Adafruit_StepperMotor *m_motor;
};

#endif /* MOTOR_H */
