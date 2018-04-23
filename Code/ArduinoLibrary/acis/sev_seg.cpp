
#include "sev_seg.h"

sev_seg::sev_seg() : m_cs_pin(DEFAULT_CS_PINT), m_intensity(DEFAULT_INTENSITY),
					 m_num_digits(DEFAULT_NUM_DIGITS)
{
	init();
}

sev_seg::sev_seg(unsigned int cs_pin) : m_cs_pin(cs_pin), m_intensity(DEFAULT_INTENSITY),
								m_num_digits(DEFAULT_NUM_DIGITS)
{
	init();
}

void sev_seg::init()
{
	set_spi();
	write_out(SHUTDOWN_REG, 0x01);			//Exit shutdown mode;
	write_out(DECODE_REG, 0xFF);			//Code B decode for digits 7–0
	write_out(INTENSITY_REG, m_intensity);  //set intensity
	write_out(SCANLIMIT_REG, m_num_digits); //set number of digits to light
	all_off();
}

int sev_seg::display_binary(unsigned char byte)
{
	for(int i =1; i < 9; ++i)
	{
		write_out(i, byte%2);
		byte /=2;
	}
}


int sev_seg::display(int num)
{
	int reg = 1;
	int val = 0;
	all_off();
	if (num < 0)
	{
		num *= -1;
		while (num > 0)
		{
			if (reg < 8)
			{
				val = num % 10;
				write_out(reg, val);
				++reg;
			}
			num /= 10;
		}
		write_out(reg, 10);
		return 0;
	}
	if (num > 0)
	{
		while (num > 0)
		{
			if (reg < 9)
			{
				val = num % 10;
				write_out(reg, val);
				++reg;
			}
			num /= 10;
		}
		return 0;
	}

	write_out(reg,0);
	return 0;
}

int sev_seg::display(unsigned int digit, unsigned int num)
{

	if (digit < 1 || digit > 8 || num > 15)
		return EINVAL;
	write_out(digit, num);
	return 0;
}

int sev_seg::set_intensity(unsigned int intensity)
{
	if (intensity > 0xF)
		return EINVAL;
	m_intensity = intensity;
	write_out(INTENSITY_REG, m_intensity);
	return 0;
}

int sev_seg::set_num_digits(unsigned int digits)
{
	if (digits > 0x7)
		return EINVAL;
	m_num_digits = digits;
	return 0;
}

void sev_seg::write_out(unsigned int reg, unsigned int val)
{
	digitalWrite(m_cs_pin, LOW);
	SPI.transfer(reg);
	SPI.transfer(val);
	digitalWrite(m_cs_pin, HIGH);
}

void sev_seg::set_spi()
{
	SPI.setDataMode(SPI_MODE0);
	SPI.setBitOrder(MSBFIRST);

	digitalWrite(m_cs_pin, HIGH);
	pinMode(m_cs_pin, OUTPUT);
	SPI.begin();
}

void sev_seg::all_off()
{
	for (int i = 1; i < 9; ++i)
		write_out(i, 0x0F);
}
