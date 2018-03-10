

class Trigger {
  int pinBot;
  int pinTop;

  unsigned long releaseTime;
  unsigned long triggerTime;

  int state;

  int triggered;

public:
  Trigger(int top, int bot);

  void enableInputs();

  void checkTriggers();

  unsigned long getDelta();

  void resetTrigger();
  void resetState();
  int getTrigger();

  int getState();
  
};

