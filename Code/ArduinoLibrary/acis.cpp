/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu
Kestutis Saltonas kestutis@pdx.edu
Thao Tran thao23@pdx.edu
William Boyd boydwil@pdx.edu

*/

#include "acis.h"

acis::acis()
{
	m_top_x_camera_motor = new motor();
	m_top_z_camera_motor = NULL;
}

void acis::move_test()
{

	m_top_x_camera_motor->move_test();
}