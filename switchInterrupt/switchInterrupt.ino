const int switchPin = 2;     // green wire of limit switch connected to pin 2 (INT0), red wire to VCC, black wire to ground
const int ledPin =  13;      // LED connected to pin 13

void setup() {
  // initialize the LED pin as an output, LED is initally off
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  
  // initialize the switchPin as an input
  pinMode(switchPin, INPUT);
  
  // Attach an interrupt to the ISR vector, capture interrupt at falling edge
  attachInterrupt(0, pin_ISR, FALLING);
}

void loop() {
  // Nothing here!
}

void pin_ISR() {
  if (digitalRead(ledPin) == LOW)
    digitalWrite(ledPin, HIGH);
  else
    digitalWrite(ledPin, LOW);
}

