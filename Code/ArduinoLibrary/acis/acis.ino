#include "acis.h"
#include "switch.h"
#include "sev_seg.h"

Adafruit_MotorShield shield_0(SHIELD_ZERO_ADDRESS);
Adafruit_MotorShield shield_1(SHIELD_ONE_ADDRESS);
Adafruit_NeoPixel pixels(NUMBER_LEDS, LEDS_PIN, NEO_GRB + NEO_KHZ800);
led leds(&pixels, NUMBER_LEDS);
acis acis(&shield_0, &shield_1, &leds);
byte buffer[BUFFER_SIZE];
limit_switch sw[NUMBER_SWITCHES];
sev_seg disp(9);
int i = 0;
void setup()
{
  acis.init();
  enable_pin_change_interrupt();
  Serial.begin(9600);
}

void loop()
{
  if (Serial.available() >= BYTES_TO_READ)
  {
    // read the incoming byte:
    Serial.readBytes(buffer, BYTES_TO_READ);
    disp.all_off();
    acis.process(buffer);
    disp.all_off();
    Serial.write(buffer, BUFFER_SIZE);
  }
}

void enable_pin_change_interrupt()
{

  PCICR |= 0b00000100;
  PCMSK2 |= 0b11111100; //enable pin change interrpt on pin 2 to 7;
  //Pullup all the pins on the port
  pinMode(2, INPUT_PULLUP);
  pinMode(3, INPUT_PULLUP);
  pinMode(4, INPUT_PULLUP);
  pinMode(5, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);
  pinMode(7, INPUT_PULLUP);
  interrupts();
}

ISR(PCINT2_vect)
{

  if (sw[X_AXIS_TOP].pressed(X_TOP_SWICH_PIN))
  {
    interrupts();
    acis.stop(X_AXIS_TOP);
    acis.send_back(buffer, X_AXIS_TOP, STOP, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
    disp.display(0);
  }

  if (sw[X_AXIS_BOTTOM].pressed(X_BOTTOM_SWICH_PIN))
  {
    interrupts();
    acis.stop(X_AXIS_BOTTOM);
    acis.send_back(buffer, X_AXIS_BOTTOM, STOP, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
    disp.display(1);
  }

  if (sw[Y_AXIS].pressed(Y_SWICH_PIN))
  {
    interrupts();
    acis.stop(Y_AXIS);
    acis.send_back(buffer, Y_AXIS, STOP, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);

    disp.display(2);
  }

  if (sw[Z_AXIS_TOP].pressed(Z_TOP_SWICH_PIN))
  {
    interrupts();
    acis.stop(Z_AXIS_TOP);
    acis.send_back(buffer, Z_AXIS_TOP, STOP, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }

  if (sw[Z_AXIS_BOTTOM].pressed(Z_BOTTOM_SWICH_PIN))
  {
    interrupts();
    acis.stop(Z_AXIS_BOTTOM);
    acis.send_back(buffer, Z_AXIS_BOTTOM, STOP, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }

  if (sw[Y_AXIS_CPU].pressed(Y_AXIS_CPU_SWITCH_PIN, 150))
  {
    interrupts();
    acis.send_back(buffer, Y_AXIS_CPU, STOP, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }
}
