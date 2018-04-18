/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#ifndef LIMIT_SWITCH
#define LIMIT_SWITCH

#include "common.h"

class limit_switch
{
  public:
	limit_switch();
	bool pressed(int switch_pin_number);

  private:
	unsigned long last_interrupt_time;
	unsigned long interrupt_time;
};

#endif /* LIMIT_SWITCH */