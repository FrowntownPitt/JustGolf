#include <Servo.h>

#define SERVO 10

Servo servo;

void setup() {
  servo.attach(SERVO);
}

int pos = 0;

void loop() {

  for(pos=0; pos <= 180; pos++){
    servo.write(pos);
    delay(15);
  }

  for(pos=180; pos >= 0; pos--){
    servo.write(pos);
    delay(15);
  }

}
