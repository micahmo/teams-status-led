#define LED 13

void setup() {
  // Begin listening on the serial port
  Serial.begin(9600);

  // Set the LED pin (13) to output
  pinMode(LED, OUTPUT);
}

void loop() {
  if(Serial.available())
  {
    char status = Serial.read();
    if (status == '1')
    {
      digitalWrite(LED, HIGH);
    }
    else if (status == '0')
    {
      digitalWrite(LED, LOW);
    }
  }
}
