

#ifndef ACIS_H
#define ACIS_H

#include "common.h"
#include "motor.h"

class acis
{
  public:
	acis();
	void move_test();

  private:
	motor *m_top_x_camera_motor;
	motor *m_top_z_camera_motor;
};

#endif /* ACIS_H */