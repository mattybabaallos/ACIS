#include "test.h"
#include "test_motor.h"


int main(int argc, char **argv)
{
	TEST_ASSERT_INT_EQ(test_move_forward(), EXIT_SUCCESS);
	TEST_ASSERT_INT_EQ(test_move_backward(), EXIT_SUCCESS);
	TEST_ASSERT_INT_EQ(test_stop(), EXIT_SUCCESS);
	TEST_ASSERT_INT_EQ(test_home(), EXIT_SUCCESS);
	TEST_ASSERT_INT_EQ(get_steps(), EXIT_SUCCESS);
}

