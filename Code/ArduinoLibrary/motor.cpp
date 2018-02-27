/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu


*/

#include "motor.h"

motor::motor()
{

	m_shield = new Adafruit_MotorShield();
	m_motor = m_shield->getStepper(200, 2);

	m_shield->begin();
	m_motor->setSpeed(100);
}

void motor::move_test()
{

	m_motor->step(100, BACKWARD, MICROSTEP); //Microstep steps
}