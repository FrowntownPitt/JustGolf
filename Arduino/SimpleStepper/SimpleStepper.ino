#define SERVO 10
#define MOTOR 3
#define DIR   4

int spd = 1000/48/2/8;

void setup() {
  // put your setup code here, to run once:
  pinMode(MOTOR, OUTPUT);
  pinMode(DIR, OUTPUT);

  digitalWrite(DIR, LOW);
}

void loop() {
  // put your main code here, to run repeatedly:
  delay(spd);
  digitalWrite(MOTOR, HIGH);
  delay(spd);
  digitalWrite(MOTOR, LOW);
}
