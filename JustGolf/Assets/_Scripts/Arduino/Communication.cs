using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arduino
{
    public class Communication : MonoBehaviour
    {

        public string port = "COM5";
        USB ArduinoHandler;

        float prevTime;

        public float pulse;

        Coroutine listenerCoroutine;
        Coroutine resetCoroutine;

        bool isConnected = false;

        public Dictionary<string, System.Action<string>> messageHandlers;

        public bool IsConnected()
        {
            if(ArduinoHandler != null)
                return ArduinoHandler.IsOpen();

            return false;
        }

        private void Start()
        {
            messageHandlers = new Dictionary<string, System.Action<string>>();
            isConnected = false;
        }

        public void AddHandler(string token, System.Action<string> handler)
        {
            Debug.Log("Added handler (" + token + ")");
            messageHandlers.Add(token, handler);
        }

        // Use this for initialization
        void OnEnable()
        {
            //TryConnect(port);
        }

        public bool TryConnect(string port)
        {
            if (isConnected)
                return true;

            ArduinoHandler = new USB(port);
            ArduinoHandler.Open();

            isConnected = ArduinoHandler.IsOpen();

            Debug.Log(isConnected);
            if(isConnected)
                StartListener();

            return isConnected;
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

        /*void TriggerHandler(string message)
        {
            //if(resetCoroutine == null)
            //    resetCoroutine = StartCoroutine(HandleTrigger());
        }*/

        void MessageHandler(string message)
        {
            Debug.Log("Message: " + message);
            bool foundHandler = false;
            foreach(string h in messageHandlers.Keys)
            {
                if (message.Contains(h))
                {
                    messageHandlers[h](message);
                    foundHandler = true;
                }
            }
            /*if (message.Contains("TRIGGER"))
            {
                TriggerHandler(message);
            }*/
            if(!foundHandler)
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
            if (listenerCoroutine != null)
            {
                StopCoroutine(listenerCoroutine);
            }
        }

        void StartListener(float time = float.PositiveInfinity)
        {
            if (ArduinoHandler != null)
            {
                Debug.Log("Starting Arduino Listener");
                System.Action<string> M = (string s) => this.MessageHandler(s);
                System.Action f = () => ReadErrorHandler();
                
                listenerCoroutine = StartCoroutine(ArduinoHandler.AsynchronousReadFromArduino(
                    M, f, time));
            } else
            {
                //Debug.Log("Arduino handler null");
            }
        }

        public void WriteToArduino(string message, int priority = 0)
        {
            if (Time.time - prevTime > pulse || priority > 0)
            {
                if (ArduinoHandler != null)
                {
                    ArduinoHandler.WriteToArduino(message);
                    prevTime = Time.time + pulse;
                }
            }
        }

        void OnDisable()
        {
            WriteToArduino("RESETSTEP", 1);
            WriteToArduino("RESETTRIGGER", 1);
            ArduinoHandler.Close();
        }
    }
}