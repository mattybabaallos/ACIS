const int switchPin = 7;     // green wire of limit switch connected to pin 2 (INT0), red wire to VCC, black wire to ground
const int ledPin =  13;      // LED connected to pin 13
static int count = 0;
static unsigned long last_interrupt_time = 0;
unsigned long interrupt_time = millis();
void setup() {
  
  // initialize the LED pin as an output, LED is initally off
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  Serial.begin(9600);
  
  // initialize the switchPin as an input
  pinMode(switchPin, INPUT);
  
  // Attach an interrupt to the ISR vector, capture interrupt at falling edge
  attachInterrupt(digitalPinToInterrupt(switchPin), pin_ISR, FALLING);
}

void loop() {
  // Nothing here!
}

void pin_ISR() {
interrupt_time = millis();
 // If interrupts come faster than 200ms, assume it's a bounce and ignore
 if (interrupt_time - last_interrupt_time > 100) 
 {
      if (!digitalRead(switchPin)) {
          ++count;
            Serial.println(count);
    }
 }
 last_interrupt_time = interrupt_time;
 
}

