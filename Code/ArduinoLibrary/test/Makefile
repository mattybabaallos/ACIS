BIN=acis_test
CC=avr-gcc
CCFLAGS=-I /usr/share/arduino/hardware/arduino/cores/arduino -g -Wall -Wextra -std=c++11
CCFLAGS=-I /usr/share/arduino/hardware/arduino/cores/arduino -g -Wextra -std=c++11
LDFLAGS=

SRCS=$(wildcard ../*.cpp) $(wildcard *.cpp)
OBJS=$(SRCS:.cpp=.o)

all: $(BIN)

$(BIN): $(OBJS)
	$(CC) $(OBJS) -o $@ $(LDFLAGS)

%.o:%.cpp
	$(CC) $(CCFLAGS) -o $@ -c $<

clean:
	rm -f $(OBJS) $(BIN)

check: $(BIN)
	./$(BIN)

check_valgrind: $(BIN)
	valgrind --leak-check=full \
		./$(BIN)
