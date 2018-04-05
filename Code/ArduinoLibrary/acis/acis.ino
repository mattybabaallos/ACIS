#include "acis.h"
#include "switch.h"

acis _acis;
char buffer[BYTES_TO_READ];
limit_switch sw[NUMBER_SWITCHES];

void setup()
{
  Serial.begin(9600);
  pinMode(X_TOP_SWICH_PIN, INPUT);

  // Attach an interrupt to the ISR vector, capture interrupt at falling edge
  attachInterrupt(digitalPinToInterrupt(X_TOP_SWICH_PIN), X_TOP_ISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(Y_SWICH_PIN), Y_ISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(Z_TOP_SWICH_PIN), Z_TOP_ISR, FALLING);
}

void loop()
{
  if (Serial.available() == BYTES_TO_READ)
  {
    // read the incoming byte:
    Serial.readBytes(buffer, BYTES_TO_READ);
    _acis.process(buffer);
    Serial.write(buffer);
  }
}

void X_TOP_ISR()
{
  if (sw[X_AXIS_TOP].pressed(X_TOP_SWICH_PIN))
    _acis.stop(X_AXIS_TOP);
}

void Y_ISR()
{
  if (sw[Y_AXIS].pressed(Y_SWICH_PIN))
    _acis.stop(Y_AXIS);
}

void Z_TOP_ISR()
{
  if (sw[Z_AXIS_TOP].pressed(Z_TOP_SWICH_PIN))
    _acis.stop(Z_AXIS_TOP);
}