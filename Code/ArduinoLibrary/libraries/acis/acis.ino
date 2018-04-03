
#include "acis.h"

acis *m_acis;
void setup()
{
	m_acis = new acis();
}

void loop()
{
	delay(3000);
	m_acis->move();
}
