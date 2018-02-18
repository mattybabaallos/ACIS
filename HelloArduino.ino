int LED = 10;

void setup() {
  Serial.begin(9600);
  pinMode(LED, OUTPUT);
}

void loop() {
  char data = Serial.read();

  switch (data){
    case 'A': digitalWrite(LED, HIGH); break;
    case 'B': digitalWrite(LED, LOW);break;
  }
}
