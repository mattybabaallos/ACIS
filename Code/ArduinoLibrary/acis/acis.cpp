/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu


NOTE: All of the Arduino was made statically allocated to avoid memory leak
*/

#include "acis.h"

acis::acis():shield_0(SHIELD_ZERO_ADDRESS),shield_1(SHIELD_ONE_ADDRESS),shield_2(SHIELD_TWO_ADDRESS),working_motor(NULL){
	motors[X_AXIS_TOP].init_motor(shield_0.getStepper(MOTOR_STEPS, X_AXIS_TOP_CHANNEL), MAX_X_TOP_LENGTH);
	motors[X_AXIS_BOTTOM].init_motor(shield_0.getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL), MAX_X_BOTTOM_LENGTH);

	motors[Z_AXIS_TOP].init_motor(shield_1.getStepper(MOTOR_STEPS, Z_AXIS_TOP_CHANNEL), MAX_Z_TOP_LENGTH);
	motors[Z_AXIS_BOTTOM].init_motor(shield_1.getStepper(MOTOR_STEPS, Z_AXIS_BOTTOM_CHANNEL), MAX_Z_BOTTOM_LENGTH);

	motors[Y_AXIS].init_motor(shield_2.getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL), MAX_Y_LENGTH);
}

int acis::move()
{
	return 0;
}

int acis::stop(int motor_id)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return 0;
	working_motor->stop();
	return 1;
}
