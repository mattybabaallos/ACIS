

#ifndef SEG_H
#define SEG_H

#include <Arduino.h>
#include <SPI.h>
#include <errno.h>

#define INTENSITY_REG 0x0A
#define SCANLIMIT_REG 0x0B
#define SHUTDOWN_REG 0x0C
#define DECODE_REG 0x09

#define DEFAULT_INTENSITY 0x0F	//Full intensity
#define DEFAULT_NUM_DIGITS 0x07 //All digits
#define DEFAULT_CS_PINT 0x02		//Pin 2


class sev_seg
{
public:
	sev_seg();
	sev_seg(unsigned int cs_pin);
	int set_intensity(unsigned int intensity);
	int set_segments(unsigned int segments);
	int set_num_digits(unsigned int digit);
	int display(int num);
	int display(unsigned int digit, unsigned int num);
	int display_binary(unsigned char byte);
  void all_off();

private:
	void write_out(unsigned int reg, unsigned int val);
	void set_spi();
	void init();
	unsigned int m_cs_pin;
	unsigned int m_intensity;
	unsigned int m_num_digits;
};

#endif /* SEG_H */
