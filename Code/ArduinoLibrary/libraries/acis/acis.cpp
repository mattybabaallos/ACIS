/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu

*/

#include "acis.h"

acis::acis()
{
	shields = new Adafruit_MotorShield * [NUMBER_SHIELD];
	for (int i = 0; i < NUMBER_SHIELD; ++i)
		shields[i] = NULL;

	motors = new motor * [NUMBER_MOTORS];
	for (int i = 0; i < NUMBER_MOTORS; ++i)
		motors[i] = NULL;

	shields[0] = new Adafruit_MotorShield(SHIELD_ZERO_ADDRESS);
	shields[1] = new Adafruit_MotorShield(SHIELD_ONE_ADDRESS);
	shields[2] = new Adafruit_MotorShield(SHIELD_TWO_ADDRESS);

	motors[X_AXIS_TOP] = new motor(shields[0]->getStepper(MOTOR_STEPS, X_AXIS_TOP_CHANNEL), MAX_X_TOP_LENGTH);
	motors[X_AXIS_BOTTOM] = new motor(shields[0]->getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL),MAX_X_BOTTOM_LENGTH);

	motors[Z_AXIS_TOP] = new motor(shields[1]->getStepper(MOTOR_STEPS, Z_AXIS_TOP_CHANNEL),MAX_Z_TOP_LENGTH);
	motors[Z_AXIS_BOTTOM] = new motor(shields[1]->getStepper(MOTOR_STEPS, Z_AXIS_BOTTOM_CHANNEL),MAX_Z_BOTTOM_LENGTH);

	motors[Y_AXIS] = new motor(shields[2]->getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL),MAX_Y_LENGTH);
}

acis::~acis()
{
	for (int i = 0; i < NUMBER_SHIELD; ++i)
		delete shields[i];
	for (int i = 0; i < NUMBER_MOTORS; ++i)
		delete motors[i];
}

int acis::move()
{
	return 0;
}

int acis::stop(int motor_id)
{
	working_motor = motors[motor_id];
	if (!working_motor)
		return 0;
	working_motor->stop();
	return 1;
}