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
	motors[X_AXIS_BOTTOM].init_motor(m_shield_0->getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL), MAX_X_BOTTOM_LENGTH);

	//motors[Z_AXIS_TOP].init_motor(shield_1.getStepper(MOTOR_STEPS, Z_AXIS_TOP_CHANNEL), MAX_Z_TOP_LENGTH);
	//motors[Z_AXIS_BOTTOM].init_motor(shield_1.getStepper(MOTOR_STEPS, Z_AXIS_BOTTOM_CHANNEL), MAX_Z_BOTTOM_LENGTH);

	motors[Y_AXIS].init_motor(m_shield_1->getStepper(MOTOR_STEPS, Y_AXIS_CHANNEL), MAX_Y_LENGTH);

	m_shield_0->begin();
	m_shield_1->begin();
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
	return working_motor->home();
}

int acis::process(char *buffer)
{
	unsigned int device = 0;
	unsigned int function = 0;
	unsigned int mm = 0;
	int temp = -1;

	if (!buffer)
		return send_back(buffer, device, function, COULD_NOT_PROCESS_BUFFER, 0);
	temp = decode(buffer, device, function, mm);
	if (temp < 0)
		return send_back(buffer, function, device, temp, 0);
	if (function == HOME)
		temp = home(device);
	else if (function == MOVE_FORWARD)
		temp = move_forward(device, mm);
	else if (function == MOVE_BACKWARD)
		temp = move_backward(device, mm);
	else if (function == STOP)
		temp = stop(device);
	else
		temp = COULD_NOT_DECODE_BYTES;
	if (temp < 0)
	{
		return send_back(buffer, device, function, temp, 0);
	}
	return send_back(buffer, device, function, SUCCESS, temp);
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

int acis::send_back(char *buffer, unsigned int device, unsigned int op, unsigned int status_code, unsigned int new_state)
{
	if (!buffer)
		return COULD_NOT_PERFORM_OPERATION;
	buffer[0] = (device | (op << 3));
	buffer[1] = new_state;
	buffer[2] = status_code;
	return status_code;
}
