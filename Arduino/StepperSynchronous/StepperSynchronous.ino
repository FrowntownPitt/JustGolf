#include <SerialCommand.h>

#include "Stepper.h"

Stepper stepper = Stepper(3, 4, 48, 4);

SerialCommand sCmd;

String inputString = "";
int stringComplete = 0;

void setup() {
  // put your setup code here, to run once:
  stepper.setEnabled(1);

  Serial.begin(115200);
  
  while(!Serial);

  sCmd.addCommand("TARGET", setTarget);
  sCmd.addCommand("ENABLE", enableStepper);
  sCmd.addCommand("DISABLE", disableStepper);
}

void loop() {
  // put your main code here, to run repeatedly:
  stepper.stepToTarget();
  sCmd.readSerial();
  Serial.read();
}

void setTarget(){
  int target;
  char *arg;
  Serial.println("Setting target");
  
  arg = sCmd.next();
  if (arg != NULL) {
    target = atoi(arg);    // Converts a char string to an integer

    stepper.setTarget(target);
  }
  else {
    Serial.println("No arguments");
  }
}

void enableStepper(){
  stepper.setEnabled(1);  
}

void disableStepper(){
  stepper.setEnabled(0);
}

