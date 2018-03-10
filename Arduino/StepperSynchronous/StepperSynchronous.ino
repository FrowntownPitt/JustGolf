#include <SerialCommand.h>

#include "Stepper.h"
#include "Kicker.h"
#include "Trigger.h"

#define STEPMAX 384
Stepper stepper = Stepper(7, 8, STEPMAX, 4);
Kicker kicker = Kicker(10, 0, 120, 1*1000000);
Trigger trigger = Trigger(2, 3);

SerialCommand sCmd;

String inputString = "";
int stringComplete = 0;

void setup() {
  //stepper = Stepper(3, 4, STEPMAX, 4);
  //kicker = Kicker(10, 50, 100, 1*1000000);
  kicker.enableServo();
  stepper.setEnabled(1);
  trigger.enableInputs();

  Serial.begin(115200);
  
  while(!Serial);

  sCmd.addCommand("TARGET", stepperSetTarget);
  sCmd.addCommand("RESETSTEP", resetStepper);
  sCmd.addCommand("ENABLE", enableStepper);
  sCmd.addCommand("DISABLE", disableStepper);
  
  sCmd.addCommand("RESETKICK", resetKicker);
  sCmd.addCommand("KICKHIGH", kickerHigh);
  sCmd.addCommand("KICKLOW", kickerLow);
  sCmd.addCommand("CHECKRESET", checkReset);

  //sCmd.addCommand("GETTRIGGER", getTrigger());
  sCmd.addCommand("RESETTRIGGER", resetTrigger);
  
}

void loop() {
  sCmd.readSerial();

  stepper.stepToTarget();

  kicker.tryReset();

  trigger.checkTriggers();

  if(trigger.getTrigger()){
    Serial.print("TRIGGER ");
    Serial.println(trigger.getDelta());
    //trigger.resetState();
    trigger.resetTrigger();
    kicker.isReset = 0;
  }

  //Serial.print("State: "); Serial.println(trigger.getState());
}

void getTrigger(){
  if(trigger.getTrigger()){
    Serial.print("TRIGGER ");
    Serial.println(trigger.getDelta());
    trigger.resetTrigger();
  } else {
    Serial.println("TRIGGER NULL");
  }
}

void resetTrigger(){
  trigger.resetTrigger();
  trigger.resetState();
  resetKicker();
}

void checkReset(){
  if(kicker.isReset){
    Serial.println("ISRESET TRUE");
  } else {
    Serial.println("ISRESET FALSE");
  }
}

void stepperSetTarget(){
  int target;
  char *arg;
  //Serial.println("Setting target");
  
  arg = sCmd.next();
  if (arg != NULL) {
    target = atoi(arg);    // Converts a char string to an integer
    target = map(target, 0, 1000, 0, STEPMAX);
    stepper.setTarget(target);
  }
  else {
    //Serial.println("No arguments");
  }
}

void kickerHigh(){
  //Serial.println("Kicker High");
  kicker.goHigh();
  //kicker.servo.write(100);
}

void kickerLow(){
  //Serial.println("Kicker Low");
  kicker.goLow();
  //kicker.servo.write(50);
}

void resetKicker(){
  //Serial.println("Resetting Kicker");
  kicker.setReset();
  //kicker.goLow();
}

void resetStepper(){
  stepper.setTarget(0);
}

void enableStepper(){
  stepper.setEnabled(1);  
}

void disableStepper(){
  stepper.setEnabled(0);
}

//int triggered = 0;
//int 

