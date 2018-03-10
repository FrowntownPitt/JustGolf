#ifndef Servo_H
#include <Servo.h>
#endif

class Kicker{

public:
  int signalPin;
  
  int angleLow;
  int angleHigh;

  int servoPin;

  int isReset;

  int reset;
  unsigned long prevTime;
  unsigned long resetTime;

  Servo servo;

  int currentPosition;

  Kicker(int pin, int lowAngle, int highAngle, unsigned long resetDuration);
  void enableServo();
  
  void goHigh();
  void goLow();

  int getPosition();
  void setPosition(int angle);
  
  void tryReset();
  void setReset();
};

