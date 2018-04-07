/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
*/

#ifndef ACIS_H
#define ACIS_H

#include "common.h"
#include "motor.h"

class acis
{
  public:
	acis(Adafruit_MotorShield * shield_0, Adafruit_MotorShield * shield_1, Adafruit_MotorShield * shield_2);
  int init();
	int move_forward(int unsigned motor_id, unsigned int mm);
	int move_backward(int unsigned motor_id, unsigned int mm);
	int stop(int unsigned motor_id);
	int home(int unsigned motor_id);
	int process(char * buffer);

  private:
	int decode(char *buffer, unsigned int & device, unsigned int & function, unsigned int & mm);
	int send_back(char * buffer,unsigned int status_code, unsigned int new_state);
Adafruit_MotorShield * m_shield_0;
Adafruit_MotorShield * m_shield_1;
Adafruit_MotorShield * m_shield_2;
	motor motors[NUMBER_MOTORS];
	motor * working_motor;
};

#endif /* ACIS_H */
