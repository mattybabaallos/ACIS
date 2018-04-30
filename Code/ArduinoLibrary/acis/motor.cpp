/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#include "motor.h"

motor::motor() : m_stop(true), m_current_position(0), m_max_distance(0), m_motor(NULL)
{
}

int motor::init_motor(Adafruit_StepperMotor *stepper, int max_distance)
{
	if (!stepper)
	{
		return COULD_NOT_PERFORM_OPERATION;
	}
	m_motor = stepper;
	m_max_distance = max_distance;
	return 1;
}

int motor::stop()
{
	if (!m_motor)
	{
		return INVALID_DEVICE;
	}
	m_motor->release();
	m_stop = true;
	return SUCCESS;
}

//move the motor forward
//return the number of mm move_forward
// negative if not successful
int motor::move_forward(float mm)
{
	if (!m_motor)
	{
		return INVALID_DEVICE;
	}
	m_stop = false;
	if (mm + m_current_position > m_max_distance)
	{
		mm = m_max_distance - m_current_position; //if the motor will be going beyond it max limit
	}
	m_current_position += get_mm(step(get_steps(mm), FORWARD, DOUBLE));
	m_motor->release();
	return m_current_position;
}

//move the motor backward
//return the number of mm move_forward
// negative if not successful
int motor::move_backward(float mm)
{

	if (!m_motor)
	{
		return INVALID_DEVICE;
	}
	m_stop = false;
	if ((float)(m_current_position - mm) <= 0)
	{
		mm = m_current_position;
	}
	m_current_position -= get_mm(step(get_steps(mm), BACKWARD, DOUBLE));
	m_motor->release();
	return m_current_position;
}

//Home the motor
int motor::home()
{
	if (!m_motor)
	{
		return INVALID_DEVICE;
	}
	m_stop = false;
	step(get_steps(MAX_X_TOP_LENGTH), BACKWARD, DOUBLE); //move the most until it hits the switch
	m_stop = false;
	step(get_steps(5), FORWARD, DOUBLE);
	m_motor->release();
	m_current_position = 0;
	return m_current_position;
}

int motor::step(int steps, int direction, int style)
{
	int temp = 0;
	while (!m_stop && steps > 0)
	{
		m_motor->onestep(direction, style);
		//delayMicroseconds(1000);
		--steps;
		++temp;
	}
	m_stop = true;
	return temp;
}

float motor::get_steps(float mm)
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

float motor::get_mm(float steps)
{
	return steps * STEP_TO_DEGREE_CONST;
}
