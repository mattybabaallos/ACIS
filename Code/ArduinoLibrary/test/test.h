#include <inttypes.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include "../common.h"


#ifndef _TEST_H_
#define _TEST_H_

#define TEST_ASSERT_PRINT(FUNC, TYPE, RES, SHOULD_BE) \
    printf("%s: " #FUNC " was %" TYPE " but should have been %" TYPE "\n", __func__, \
        RES, SHOULD_BE); \

#define TEST_ASSERT_UINT16_EQ(FUNC, SHOULD_BE) { \
  uint16_t res = FUNC; \
  if (res != SHOULD_BE) { \
    TEST_ASSERT_PRINT(FUNC, PRIi16, res, SHOULD_BE) \
    return EXIT_FAILURE; \
  }}

#define TEST_ASSERT_INT_EQ(FUNC, SHOULD_BE) { \
  int res = FUNC; \
  if (res != SHOULD_BE) { \
    TEST_ASSERT_PRINT(FUNC, "d", res, SHOULD_BE) \
    return EXIT_FAILURE; \
  }}

#define TEST_ASSERT_INT_NOT(FUNC, SHOULDNT_BE) { \
  int res = FUNC; \
  if (res == SHOULDNT_BE) { \
    printf(\
       "%s: " #FUNC " was %d but should not have been\n", __func__, \
        res); \
    return EXIT_FAILURE; \
  }}

#define TEST_ASSERT_NOT_NULL(FUNC) { \
  void *res = FUNC; \
  if (res == NULL) { \
    printf("%s: " #FUNC " should not have been NULL\n", __func__); \
    return EXIT_FAILURE; \
  }}

#define TEST_ASSERT(FUNC) { \
  if (!FUNC) { \
    printf("%s: " #FUNC " should not have been NULL\n", __func__); \
    return EXIT_FAILURE; \
  }}

int test_compiler();

#endif /* _TEST_H_ */
