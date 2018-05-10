/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu

NOTE: I am reusing the same code I wrote for ECE 411
*/

#ifndef LED_H
#define LED_H
#include "common.h"


class led
{
  public:
    led(Adafruit_NeoPixel *leds, int number_leds);
    int set(int led, byte r, byte g, byte b);
    int set(byte r, byte g, byte b);
    int set(int hex);
    int set(int led,int hex);
    int off(int led);
    int off();

  private:
    int m_numLed;
    Adafruit_NeoPixel * m_leds;
};

#endif