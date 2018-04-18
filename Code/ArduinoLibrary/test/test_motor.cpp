#include "test_motor.h"

int test_move_forward() {
	Adafruit_MotorShield shield;
	motor m;
	m.init_motor(shield.getStepper(MOTOR_STEPS, X_AXIS_TOP_CHANNEL),
		 MAX_X_TOP_LENGTH);


}

 return EXIT_FAILURE;
};

int test_move_backward() {
 return EXIT_FAILURE;
};

int test_stop() {
 return EXIT_FAILURE;
};

int test_home() {
 return EXIT_FAILURE;
};

int get_steps() {
 return EXIT_FAILURE;
};
