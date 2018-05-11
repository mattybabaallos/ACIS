/*
ECE Capstone 2018
Automatic Check In Station

Matty Baba Allos matty@pdx.edu

NOTE: I am reusing the same code I wrote for ECE 411
*/

#include "led.h"

led::led(Adafruit_NeoPixel *leds, int number_leds) : m_leds(leds), m_numLed(number_leds)
{
}

int led::set(int led, byte r, byte g, byte b)
{

    if (!m_leds)
        return INVALID_DEVICE;

    if (led < 0 || led > m_numLed)
        return INVALID_OPERATION;

    m_leds->setPixelColor(led, r, g, b);
    m_leds->show();
    return SUCCESS;
}

int led::set(byte r, byte g, byte b)
{
    if (!m_leds)
        return INVALID_DEVICE;
    for (int i = 0; i < m_numLed; ++i)
    {
        m_leds->setPixelColor(i, r, g, b);
        m_leds->show();
    }
    return SUCCESS;
}

int led::set(long hex_color)
{
    if (!m_leds)
        return INVALID_DEVICE;
    byte r = ((hex_color >> 8) >> 8);
    byte g = (((hex_color << 8) >> 8) >> 8);
    byte b = hex_color;
    for (int i = 0; i < m_numLed; ++i)
    {
        m_leds->setPixelColor(i, r, g, b);
        m_leds->show();
    }
    return SUCCESS;
}

int led::set(int led, long hex_color)
{
    if (!m_leds)
        return INVALID_DEVICE;
    byte r = ((hex_color >> 8) >> 8);
    byte g = (((hex_color << 8) >> 8) >> 8);
    byte b = hex_color;
    m_leds->setPixelColor(led, r, g, b);
    m_leds->show();
    return SUCCESS;
}

int led::off(int led)
{
    if (!m_leds)
        return INVALID_DEVICE;
    if (led < 0 || led > m_numLed)
        return INVALID_OPERATION;
    m_leds->setPixelColor(led, 0, 0, 0);
    m_leds->show();
    return SUCCESS;
}

int led::off()
{
    if (!m_leds)
        return INVALID_DEVICE;
    for (int i = 0; i < m_numLed; ++i)
    {
        m_leds->setPixelColor(i, 0, 0, 0);
        m_leds->show();
    }
    return SUCCESS;
}
