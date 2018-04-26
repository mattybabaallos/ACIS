
const int XINT0=2;
const int LED1=11;
const int LED2=9;
const int LED3=10;
const int LED4=8;
const int BUTTON1=14;
const int BUTTON2=15;
const int BUTTON3=16;
const int BUTTON4=17;

float interrupt_time=0.0;
float last_interrupt_time=0.0; 


bool pressed(int pin_number)
{
  bool pressed = false;
  interrupt_time = millis();
  if (interrupt_time - last_interrupt_time > 100)
  {
    if(!digitalRead(pin_number))
    {
      pressed = true;
    }
  }
  last_interrupt_time = interrupt_time;
  return pressed;
}


void BUTTON_ISR()
{
  int triggered = 0;
  triggered |= digitalRead(BUTTON1) | (digitalRead(BUTTON2) << 1) | (digitalRead(BUTTON3) << 2) | (digitalRead(BUTTON4) << 3);
  if(!(triggered & 1))
  {
    digitalWrite(LED1, HIGH);
  }
  if(!(triggered & 2))
  {
    digitalWrite(LED2, HIGH);
  }
  if(!(triggered & 4))
  {
    digitalWrite(LED3, HIGH);
  }
  if(!(triggered & 8))
  {
    digitalWrite(LED4, HIGH);
  }
  interrupts();
}


void setup() {
  // put your setup code here, to run once:
  attachInterrupt(digitalPinToInterrupt(XINT0), BUTTON_ISR, FALLING);
  pinMode(BUTTON1, INPUT_PULLUP);
  pinMode(BUTTON2, INPUT_PULLUP);
  pinMode(BUTTON3, INPUT_PULLUP);
  pinMode(BUTTON4, INPUT_PULLUP);
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(LED3, OUTPUT);
  pinMode(LED4, OUTPUT);  
}

void loop() {
  // put your main code here, to run repeatedly:
  int i;
  
  interrupts();
  delay(200);
  digitalWrite(LED1, LOW);
  digitalWrite(LED2, LOW);
  digitalWrite(LED3, LOW);
  digitalWrite(LED4, LOW);
  /*while(2)
  {
    digitalWrite(LED1, HIGH);
    delay(50);
    digitalWrite(LED1, LOW);
    digitalWrite(LED2, HIGH);
    delay(50);
    digitalWrite(LED2, LOW);
    digitalWrite(LED3, HIGH);
    delay(50);
    digitalWrite(LED3, LOW);
    digitalWrite(LED4, HIGH);
    delay(50);
    digitalWrite(LED1, LOW);
    digitalWrite(LED2, LOW);
    digitalWrite(LED3, LOW);
    digitalWrite(LED4, LOW);
  }*/

}

