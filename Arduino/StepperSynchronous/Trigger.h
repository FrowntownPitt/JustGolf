

class Trigger {
  int pinBot;
  int pinTop;

  unsigned long releaseTime;
  unsigned long triggerTime;

  long prevStateTime;

  int state;

  int triggered;

  long debounce;

public:
  Trigger(int top, int bot, long debnc);

  void enableInputs();

  void checkTriggers();

  unsigned long getDelta();

  void resetTrigger();
  void resetState();
  int getTrigger();

  int getState();
  
};

