

#ifndef ACIS_H
#define ACIS_H

#include "common.h"
#include "motor.h"

class acis
{
  public:
	acis();
	void move_test();
	int stop();

  private:
	Adafruit_MotorShield ** shields;
	motor ** motors;
	motor * working_motor;
};

#endif /* ACIS_H */