class Stepper {

  int stepPin;
  int dirPin;
  int dirDefault;
  int enabled;

  int angleCurrent;
  int angleTarget;

  unsigned long prevTime;

  int spr; // steps per revolution
  int rps;

  unsigned long stepTime;

public:
  Stepper(int sPin, int dPin, int stepsPerRev, float rps, int dDefault=0);

  void setTarget(int angle);

  void stepToTarget();

  void setEnabled(int en);

  int getEnabled();
  
};
