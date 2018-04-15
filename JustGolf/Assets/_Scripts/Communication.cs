using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Communication : MonoBehaviour {

    public string port = "COM5";
    USB ArduinoHandler;

    float prevTime;

    public float pulse;

    Coroutine listenerCoroutine;
    Coroutine resetCoroutine;

    public double mult = 118.768;
    public double exp = -0.00879889;
    public double offset = 10;


	// Use this for initialization
	void Start () {
        ArduinoHandler = new USB(port);

        ArduinoHandler.Open();

        //StartCoroutine(TriggerCheck());
        StartListener();
	}

    IEnumerator TriggerCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            StopListener();
            StartListener(2000f);
        }
    }

    public double CalcPower(long time)
    {
        return mult * (Mathf.Exp((float)exp * time)) + offset;
    }

    void MessageHandler(string message)
    {
        Debug.Log("Message: " + message);
        if (message.Contains("TRIGGER"))
        {
            string[] m = message.Split();
            string t = m[1];
            long dur;
            bool valid =  long.TryParse(t, out dur);
            if (valid)
            {
                Debug.Log("Triggered: " + dur);
                Debug.Log("Power: " + CalcPower(dur/1000) + "%");
                
            }
            //if(resetCoroutine == null)
            //    resetCoroutine = StartCoroutine(HandleTrigger());
        }
        else
        {
            StopListener();
            StartListener();
        }
    }

    public void TryReset()
    {
        ResetTrigger();
    }

    /*IEnumerator HandleTrigger()
    {
        StopListener();


        bool doReset = true;

        WaitForSeconds waitForSeconds = new WaitForSeconds(.1f);

        WriteToArduino("CHECKRESET", 1);

        string response = ArduinoHandler.ReadFromArduino(5000);

        Debug.Log("CHECKRESET: " + response);

        string[] m = response.Split();


        if (m.Length != 2)
        {
            Debug.Log("CHECKRESET Response malformed!");
        }
        else
        {
            if (m[1].Equals("TRUE"))
            {
                doReset = false;
                Debug.Log("Already Reset!");
            }
            if (m[1].Equals("FALSE"))
            {
                doReset = true;
                ResetTrigger();
            }
        }

        while (doReset)
        {
            WriteToArduino("CHECKRESET", 1);

            response = ArduinoHandler.ReadFromArduino(5000);

            Debug.Log("CHECKRESET: " + response);

            m = response.Split();

            if(m.Length != 2)
            {
                Debug.Log("CHECKRESET Response malformed!");
            }
            else
            {
                if (m[1].Equals("TRUE"))
                {
                    doReset = false;
                    Debug.Log("Finished reset!");
                    break;
                }
                if (m[1].Equals("FALSE"))
                {
                }
            }
            
            yield return waitForSeconds;
        }

        Debug.Log("Reset finished!");

        StartListener();

        resetCoroutine = null;

        yield return null;
    }*/

    void ReadErrorHandler()
    {
        Debug.Log("Error!");
        StopListener();
        ResetTrigger();
        StartListener();
    }

    void ResetTrigger()
    {
        WriteToArduino("RESETTRIGGER", 1);
        //WriteToArduino("RESETTRIGGER");
    }

    void StopListener()
    {
        if(listenerCoroutine != null)
        {
            StopCoroutine(listenerCoroutine);
        }
    }

    void StartListener(float time=float.PositiveInfinity)
    {
        Debug.Log("Starting Arduino Listener");
        listenerCoroutine = StartCoroutine(ArduinoHandler.AsynchronousReadFromArduino(
            (string s) => MessageHandler(s), () => ReadErrorHandler(), time));
    }

    public void WriteToArduino(string message, int priority=0)
    {
        if (Time.time - prevTime > pulse || priority > 0)
        {
            ArduinoHandler.WriteToArduino(message);
            prevTime = Time.time + pulse;
        }
    }

    void OnDisable()
    {
        WriteToArduino("RESETSTEP", 1);
        WriteToArduino("RESETTRIGGER", 1);
        ArduinoHandler.Close();
    }
}
