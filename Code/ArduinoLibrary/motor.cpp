/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu


*/

#include "motor.h"

motor::motor(Adafruit_MotorShield * stepper)
{
	m_motor = stepper;
}


int motor::stop()
{
	if(!_motor)
		return 0;
	m_motor->release();
	return 1;
}


int motor::move()
{

	//m_motor->step(20, BACKWARD, MICROSTEP); //Microstep steps
}


int motor::home()
{

}