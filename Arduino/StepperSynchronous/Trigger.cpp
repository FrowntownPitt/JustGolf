#ifndef Arduino_H
#include "Arduino.h"
#endif
#include "Trigger.h"

Trigger::Trigger(int top, int bot){
  pinTop = top;
  pinBot = bot;
}

void Trigger::enableInputs(){
  pinMode(pinTop, INPUT);
  pinMode(pinBot, INPUT);
}

void Trigger::checkTriggers(){
  switch(state){
    case 0: // Base
      if(digitalRead(pinTop) == LOW){
        releaseTime = micros();
        state = 1;
        break;
      }
    case 1: // Falling
      if(digitalRead(pinBot) == HIGH){
        triggerTime = micros();
        state = 2;
        break;
      }
      if(digitalRead(pinTop) == HIGH){
        state = 0; // Reset
        break;
      }
      break;
    case 2: // Triggered
      triggered = 1;
      state = 3;
      break;
    case 3: // Idle
      break;
    default:
      break;
  }
}

unsigned long Trigger::getDelta(){
  return triggerTime - releaseTime;
}

void Trigger::resetTrigger(){
  triggered = 0;
}

void Trigger::resetState(){
  state = 0;
}

int Trigger::getTrigger(){
  return triggered;
}
int Trigger::getState(){
  return state;
}

