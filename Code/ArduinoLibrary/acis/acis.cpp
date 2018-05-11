/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu


NOTE: All of the Arduino was made statically allocated to avoid memory leak
*/

#include "acis.h"

acis::acis(Adafruit_MotorShield *shield_0, Adafruit_MotorShield *shield_1, led *leds) : m_shield_0(shield_0), m_shield_1(shield_1), working_motor(NULL), m_leds(leds)
{
}

int acis::init()
{
	motors[X_AXIS_TOP].init_motor(m_shield_0->getStepper(MOTOR_STEPS, X_AXIS_TOP_CHANNEL), MAX_X_TOP_LENGTH);
	motors[X_AXIS_BOTTOM].init_motor(m_shield_0->getStepper(MOTOR_STEPS, X_AXIS_BOTTOM_CHANNEL), MAX_X_BOTTOM_LENGTH);
	motors[Y_AXIS].init_motor(m_shield_1->getStepper(MOTOR_STEPS, Y_AXIS_CHANNEL), MAX_Y_LENGTH);

	m_shield_0->begin();
	m_shield_1->begin();
	return 0;
}

int acis::move_forward(byte motor_id, long data)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->move_forward(data);
}

int acis::move_backward(byte motor_id, long data)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->move_backward(data);
}

int acis::stop(byte motor_id)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->stop();
}

int acis::home(byte motor_id)
{
	working_motor = &motors[motor_id];
	if (!working_motor)
		return INVALID_DEVICE;
	return working_motor->home();
}

int acis::leds_on(byte led_id, long hex_color)
{

	return 0;
}


int acis::leds_off(byte led_id)
{
	return 0;
}

int acis::process(byte *buffer)
{
	byte device = 0;
	byte function = 0;
	long data = 0;
	int temp = -1;

	if (!buffer)
		return send_back(buffer, device, function, 0, COULD_NOT_PROCESS_BUFFER);
	temp = decode(buffer, device, function, data);
	if (temp < 0)
		return send_back(buffer, function, device, 0, temp);
	if (function == HOME)
		temp = home(device);
	else if (function == MOVE_FORWARD)
		temp = move_forward(device, data);
	else if (function == MOVE_BACKWARD)
		temp = move_backward(device, data);
	else if (function == STOP)
		temp = stop(device);
	else
		temp = COULD_NOT_DECODE_BYTES;
	if (temp < 0)
	{
		return send_back(buffer, device, function, temp, 0);
	}
	return send_back(buffer, device, function, temp, SUCCESS);
}

int acis::decode(byte *buffer, byte &device, byte &function, long &data)
{
	if (!buffer)
		return COULD_NOT_PERFORM_OPERATION;
	device = buffer[0];
	function = buffer[1];
	data = (((buffer[4] << 8) << 8) | (buffer[3] << 8) | buffer[2]);
	return SUCCESS;
}

int acis::send_back(byte *buffer, byte device, byte function, long data, byte error_code)
{
	if (!buffer)
		return COULD_NOT_PERFORM_OPERATION;
	buffer[0] = device;
	buffer[1] = function;
	buffer[2] = data;
	buffer[3] = (data >> 8);
	buffer[4] = ((data >> 8) >> 8);
	buffer[6] = error_code;
	return SUCCESS;
}
