#ifndef Arduino_H
#include "Arduino.h"
#endif
#include "Stepper.h"

Stepper::Stepper(int sPin, int dPin, int stepsPerRev, float rps, int dDefault=0){
  stepPin = sPin;
  dirPin = dPin;
  dirDefault = dDefault;

  spr = stepsPerRev;

  angleCurrent = 0;

  stepTime = (long) 1000000/stepsPerRev / rps;

  prevTime = micros();

  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);

  digitalWrite(dirPin, dDefault);
  
}

void Stepper::setTarget(int angle){
  angleTarget = angle;
}

void Stepper::stepToTarget(){
  unsigned long currTime = micros();
  if(enabled && (currTime - prevTime) >= stepTime){
    int delta = angleTarget - angleCurrent;

    /*if(delta > spr/2){
      delta = -(spr-delta);
    }
    if(delta < -spr/2){
      delta = delta+spr;
    }*/

    if(delta > 0){
      digitalWrite(dirPin, dirDefault);
      digitalWrite(stepPin, LOW);
      digitalWrite(stepPin, HIGH);
      prevTime = currTime;
      angleCurrent++;
    } else if(delta < 0){
      digitalWrite(dirPin, !dirDefault);
      digitalWrite(stepPin, LOW);
      digitalWrite(stepPin, HIGH);
      prevTime = currTime;
      angleCurrent--;
    }
    

    
    if(angleCurrent >= spr){
      angleCurrent = 0;
    } else if(angleCurrent < 0){
      angleCurrent = spr-1;
    }
  }
}

void Stepper::setEnabled(int en){
  enabled = en;
}

int Stepper::getEnabled(){
  return enabled;
}
  
