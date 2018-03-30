/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu


*/

#include "motor.h"

motor::motor(Adafruit_MotorShield * stepper,most_steps, unsigned int max_distance) :m_motor(stepper), max_distance(max_distance)
{
}


int motor::stop()
{
	if(!_motor)
		return 0;
	m_motor->release();
	m_motor->step(0, FORWARD, MICROSTEP); //Hold torque
	return 1;
}

//move the motor forward
//return the number of mm move_forward
// negative if not successful
int motor::move_forward(unsigned int mm)
{

	if(!_motor)
		return -1;
	if(mm + current_position > min_distance)
		mm = most_distance - current_position; //if the motor will be going beyond it max limit 
	current_position +=mm;
	m_motor->step(get_steps(mm), FORWARD, DOUBLE);
}

//move the motor backward
//return the number of mm move_forward
// negative if not successful
int motor::move_backward(unsigned int mm)
{
	if(!_motor)
		return -1;

	if(current_position - mm <= 0)
		mm = current_position;

	current_position  -= mm;
	m_motor->step(get_steps(mm), BACKWARD, DOUBLE);

}

//Home the motor
int motor::home()
{
	current_position = 0;
	m_motor->step(get_steps(max_distance+5), BACKWARD, DOUBLE);  //move the most until it hits the switch
}

int motor::get_steps(unsigned int mm)
{

}