

#ifndef ACIS_H
#define ACIS_H

#include "common.h"
#include "motor.h"

class acis
{
  public:
	acis();
	~acis();
	int move();
	int stop(int motor_id);

  private:
	Adafruit_MotorShield ** shields;
	motor ** motors;
	motor * working_motor;
};

#endif /* ACIS_H */