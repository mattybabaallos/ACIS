/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#include "motor.h"

motor::motor() : current_position(0), max_distance(0), m_motor(NULL)
{
}

int motor::init_motor(Adafruit_StepperMotor *stepper, unsigned int max_distance)
{
	if (!stepper)
		return COULD_NOT_PERFORM_OPERATION;
	m_motor = stepper;
	max_distance = max_distance;
	return 1;
}

int motor::stop()
{
	if (!m_motor)
		return INVALID_DEVICE;
	m_motor->release();
	m_motor->step(0, FORWARD, MICROSTEP); //Hold torque
	return SUCCESS;
}

//move the motor forward
//return the number of mm move_forward
// negative if not successful
int motor::move_forward(unsigned int mm)
{

	if (!m_motor)
		return INVALID_DEVICE;
	if (mm + current_position > max_distance)
		mm = max_distance - current_position; //if the motor will be going beyond it max limit
	current_position += mm;
	m_motor->step(get_steps(mm), FORWARD, DOUBLE);
	return current_position;
}

//move the motor backward
//return the number of mm move_forward
// negative if not successful
int motor::move_backward(unsigned int mm)
{
	if (!m_motor)
		return INVALID_DEVICE;
	if (current_position - mm <= 0)
		mm = current_position;

	current_position -= mm;
	m_motor->step(get_steps(mm), BACKWARD, DOUBLE);
	return current_position;
}

//Home the motor
int motor::home()
{
	if (!m_motor)
		return INVALID_DEVICE;
	current_position = 0;
	m_motor->step(get_steps(max_distance + 5), BACKWARD, DOUBLE); //move the most until it hits the switch
	return current_position;
}

int motor::get_steps(unsigned int mm)
{
	/*
	 * To get the number of stepps needed to travel that number of mm
	 * we need to find the circumference of the pulley outside that hold
	 * time belt
	 * in our case that's 13mm. c = 13 * pi = 40.84mm
	 * now the number mm we need to travel divide by 40.84mm gives us
	 * how many revolutions we need. Then we know there are 200 steps per
	 * revolutions
	 * multiplying the two numbers give the number of steps we need.
	 * or
	 * we also know that motor has 50 steps and
	 * that 360/50 = 1.8 step/degree.
	 * For arc of 1 degree is the distance of traveled by 1 degree that is
	 * arc of 1 degree = 1 * pi/180 * (13/2) = 0.113 mm/degree
	 * to get the number of steps/mm we
	 * 1.8 degree/step * 0.113 mm/degree = 0.204 mm/steps
	 * if we divide the number of mm we travel by 0.204 we get the
	 * number of steps
	 */
	return mm / STEP_TO_DEGREE_CONST;
}
