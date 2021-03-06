/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#include "switch.h"

limit_switch::limit_switch() : last_interrupt_time(0), interrupt_time(0)
{
}

bool limit_switch::pressed(int switch_pin_number)
{
	bool pressed = false;
	interrupt_time = millis();
	// If interrupts come faster than 200ms, assume it's a bounce and ignore
	if (interrupt_time - last_interrupt_time > 50)
	{
		if (!digitalRead(switch_pin_number))
		{
			pressed = true;
		}
	}
	last_interrupt_time = interrupt_time;
	return pressed;
}

bool limit_switch::pressed(int switch_pin_number, int time_to_wait)
{
	bool pressed = false;
	interrupt_time = millis();
	// If interrupts come faster than 200ms, assume it's a bounce and ignore
	if (interrupt_time - last_interrupt_time >time_to_wait)
	{
		if (!digitalRead(switch_pin_number))
		{
			pressed = true;
		}
	}
	last_interrupt_time = interrupt_time;
	return pressed;
}