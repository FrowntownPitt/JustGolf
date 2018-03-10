#ifndef Servo_H
#include <Servo.h>
#endif
#ifndef Arduino_H
#include "Arduino.h"
#endif
#include "Kicker.h"


Kicker::Kicker(int pin, int lowAngle, int highAngle, unsigned long resetDuration){
  //servo.attach(pin);

  servoPin = pin;

  angleLow = lowAngle;
  angleHigh = highAngle;

  //servo.write(lowAngle);

  reset = 0;

  resetTime = resetDuration;
}

void Kicker::enableServo(){
  servo.attach(servoPin);
  Kicker::goLow();
}

void Kicker::tryReset(){
  if(reset && micros()-prevTime <= resetTime){
    servo.write(angleHigh);
  } else if(reset && micros()-prevTime <= resetTime*2){
    servo.write(angleLow);
    reset = 0;
    isReset = 1;
  }
}

void Kicker::setReset(){
  reset = 1;
  isReset = 0;
  prevTime = micros();
}

void Kicker::goHigh(){
  servo.write(angleHigh);
}

void Kicker::goLow(){
  servo.write(angleLow);
}

int Kicker::getPosition(){
  return currentPosition;
}

void Kicker::setPosition(int angle){
  if(angle >= 0 && angle <= 180)
    servo.write(angle);
}

