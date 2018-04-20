#include "acis.h"
#include "switch.h"

Adafruit_MotorShield shield_0(SHIELD_ZERO_ADDRESS);
Adafruit_MotorShield shield_1(SHIELD_ONE_ADDRESS);
Adafruit_MotorShield shield_2(SHIELD_TWO_ADDRESS);
acis _acis(&shield_0, &shield_1, &shield_2);
char buffer[BUFFER_SIZE];
limit_switch sw[NUMBER_SWITCHES];
void setup()
{

  Serial.println(_acis.init());
  Serial.begin(9600);

  // Attach an interrupt to the ISR vector, capture interrupt at falling edge
  attachInterrupt(digitalPinToInterrupt(X_TOP_SWICH_PIN), X_TOP_ISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(X_BOTTOM_SWICH_PIN), X_BOTTOM_ISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(Y_SWICH_PIN), Y_ISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(Z_TOP_SWICH_PIN), Z_TOP_ISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(Z_BOTTOM_SWICH_PIN), Z_BOTTOM_ISR, FALLING);
}

void loop()
{
  if (Serial.available() >= BYTES_TO_READ)
  {
    // read the incoming byte:
    Serial.readBytes(buffer, BYTES_TO_READ);
    _acis.process(buffer);
  Serial.write(buffer,BUFFER_SIZE);
  }
}

void X_TOP_ISR()
{

  if (sw[X_AXIS_TOP].pressed(X_TOP_SWICH_PIN))
  {
    interrupts();
    _acis.send_back(buffer,X_AXIS_TOP,STOP,STOP_INTERRUPT,0);
    _acis.stop(X_AXIS_TOP);
  }
}

void X_BOTTOM_ISR()
{
  if (sw[X_AXIS_BOTTOM].pressed(X_BOTTOM_SWICH_PIN))
  {
    interrupts();
    _acis.send_back(buffer,X_AXIS_BOTTOM,STOP,STOP_INTERRUPT,0);
    _acis.stop(X_AXIS_BOTTOM);
  }
}

void Y_ISR()
{
  if (sw[Y_AXIS].pressed(Y_SWICH_PIN))
  {
    interrupts();
    _acis.send_back(buffer,Y_AXIS,STOP,STOP_INTERRUPT,0);
    _acis.stop(Y_AXIS);
  }
}

void Z_TOP_ISR()
{
  if (sw[Z_AXIS_TOP].pressed(Z_TOP_SWICH_PIN))
  {
    interrupts();
    _acis.send_back(buffer,Z_AXIS_TOP,STOP,STOP_INTERRUPT,0);
    _acis.stop(Z_AXIS_TOP);
  }
}

void Z_BOTTOM_ISR()
{
  if (sw[Z_AXIS_BOTTOM].pressed(Z_BOTTOM_SWICH_PIN))
  {
    interrupts();
    _acis.send_back(buffer,Z_AXIS_BOTTOM,STOP,STOP_INTERRUPT,0);
    _acis.stop(Z_AXIS_BOTTOM);
  }
}
