/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu


NOTE: All of the Arduino was made statically allocated to avoid memory leak
*/

#include "acis.h"

acis::acis(Adafruit_MotorShield *shield_0, Adafruit_MotorShield *shield_1, Adafruit_MotorShield *shield_2) : m_shield_0(shield_0), m_shield_1(shield_1), m_shield_2(shield_2), working_motor(NULL)
{
}

int acis::init()
{
	motors[X_AXIS_TOP].init_motor(m_shield_0->getStepper(MOTOR_STEPS, X_AXIS_TOP_CHANNEL), MAX_X_TOP_LENGTH);
	//motors[X_AXIS_BOTTOM].init_motor(shield_0.getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL), MAX_X_BOTTOM_LENGTH);

	//  motors[Z_AXIS_TOP].init_motor(shield_1.getStepper(MOTOR_STEPS, Z_AXIS_TOP_CHANNEL), MAX_Z_TOP_LENGTH);
	//motors[Z_AXIS_BOTTOM].init_motor(shield_1.getStepper(MOTOR_STEPS, Z_AXIS_BOTTOM_CHANNEL), MAX_Z_BOTTOM_LENGTH);

	//motors[Y_AXIS].init_motor(shield_2.getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL), MAX_Y_LENGTH);

	m_shield_0->begin();
	//shield_2.begin();
	return 0;
}

int acis::move_forward(int unsigned motor_id, unsigned int mm)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->move_forward(mm);
}

int acis::move_backward(int unsigned motor_id, unsigned int mm)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->move_backward(mm);
}

int acis::stop(int unsigned motor_id)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->stop();
}

int acis::home(int unsigned motor_id)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	working_motor->home();
	return SUCCESS;
}

int acis::process(char *buffer)
{
	unsigned int device;
	unsigned int function;
	unsigned int mm;
	int temp;

	if (!buffer)
		return COULD_NOT_PERFORM_OPERATION;
	temp = decode(buffer, device, function, mm);
	if (temp < 0)
		return temp;
	if (function == HOME)
		temp = send_back(buffer, SUCCESS, home(device));
	else if (function == MOVE_FORWARD)
		temp = send_back(buffer, SUCCESS, move_forward(device, mm));
	else if (function == MOVE_BACKWARD)
		temp = send_back(buffer, SUCCESS, move_backward(device, mm));
	else if (function == STOP)
		temp = send_back(buffer, stop(device), 0);
	else
		temp = COULD_NOT_DECODE_BYTES;
	return temp;
}

int acis::decode(char *buffer, unsigned int &device, unsigned int &function, unsigned int &mm)
{
	if (!buffer)
		return COULD_NOT_PERFORM_OPERATION;
	char temp = buffer[0];
	device = create_mask(0, 2) & temp;
	function = (create_mask(3, 4) & temp) >> 3;
	mm = buffer[1];
	return SUCCESS;
}

int acis::send_back(char *buffer, unsigned int status_code, unsigned int new_state)
{
	if (!buffer)
		return COULD_NOT_PERFORM_OPERATION;
	buffer[0] = status_code;
	buffer[1] = new_state;
	return status_code;
}
