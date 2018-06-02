#include "acis.h"
#include "switch.h"
#include "sev_seg.h"

Adafruit_MotorShield shield_0(SHIELD_ZERO_ADDRESS);
Adafruit_MotorShield shield_1(SHIELD_ONE_ADDRESS);
Adafruit_NeoPixel top_pixels(NUMBER_LEDS, TOP_LEDS_PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel bottom_pixels(NUMBER_LEDS, BOTTOM_LEDS_PIN, NEO_GRB + NEO_KHZ800);

led top_leds(&top_pixels, NUMBER_LEDS);
led bottom_leds(&bottom_pixels, NUMBER_LEDS);
acis acis(&shield_0, &shield_1, &top_leds, &bottom_leds);
byte buffer[BUFFER_SIZE];
limit_switch sw[NUMBER_SWITCHES];
int i = 0;
void setup()
{
  acis.init();
  enable_pin_change_interrupt();
  Serial.begin(9600);
  acis.home(X_AXIS_TOP_MOTOR);
  acis.home(X_AXIS_BOTTOM_MOTOR);
  acis.home(Y_AXIS_MOTOR);
  bottom_leds.set(0xffffff);
  top_leds.set(0xffffff);
}

void loop()
{
  if (Serial.available() >= BUFFER_SIZE)
  {
    // read the incoming byte:
    Serial.readBytes(buffer, BUFFER_SIZE);
    acis.process(buffer);
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

  if (sw[X_AXIS_TOP_MOTOR].pressed(X_TOP_SWICH_PIN))
  {
    interrupts();
    acis.stop(X_AXIS_TOP_MOTOR);
    acis.send_back(buffer, X_AXIS_TOP_MOTOR, STOP_STEPPER, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }

  if (sw[X_AXIS_BOTTOM_MOTOR].pressed(X_BOTTOM_SWICH_PIN))
  {
    interrupts();
    acis.stop(X_AXIS_BOTTOM_MOTOR);
    acis.send_back(buffer, X_AXIS_BOTTOM_MOTOR, STOP_STEPPER, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }

  if (sw[Y_AXIS_MOTOR].pressed(Y_SWICH_PIN))
  {
    interrupts();
    acis.stop(Y_AXIS_MOTOR);
    acis.send_back(buffer, Y_AXIS_MOTOR, STOP_STEPPER, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }

  if (sw[Y_AXIS_CPU_SWITCH].pressed(Y_AXIS_CPU_SWITCH_PIN, 150))
  {
    interrupts();
    acis.send_back(buffer, Y_AXIS_CPU_SWITCH, STOP_STEPPER, 0, STOP_INTERRUPT);
    Serial.write(buffer, BUFFER_SIZE);
  }
}
