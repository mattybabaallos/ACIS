
#include "acis.h"

acis m_acis;
char buffer[2];
void setup()
{
  Serial.begin(9600);
}

void loop()
{
  if (Serial.available() == 2)
  {
    // read the incoming byte:
    Serial.readBytes(buffer, BYTES_TO_READ);
    m_acis.process(buffer);
    Serial.write(buffer);
  }
}



void pin_ISR() {



}
