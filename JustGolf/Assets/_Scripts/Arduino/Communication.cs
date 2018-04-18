using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arduino
{
    // Interface container for handling USB communication requests (read/write)
    public class Communication : MonoBehaviour
    {

        public string port = "COM5"; // What port to connect to
        USB ArduinoHandler;         // USB device to handle 

        float prevTime;

        public float pulse; // How much time should be guaranteed between sends to the arduino

        Coroutine listenerCoroutine;    // "Singleton" listener
        Coroutine resetCoroutine;       // "Singleton" reset handler

        bool isConnected = false;

        public Dictionary<string, System.Action<string>> messageHandlers; // All <string, callback> pairs to account for
        
        // Returns true if the USB communication is available and open
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

        // Add the <token, callback> pair
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

        // Attempt to connect to the port. Does nothing if already connected
        public bool TryConnect(string port)
        {
            if (isConnected)
                return true;

            ArduinoHandler = new USB(port);
            ArduinoHandler.Open();

            isConnected = ArduinoHandler.IsOpen();

            Debug.Log(isConnected);
            if(isConnected)
                StartListener(); // When it connects, start listening for messages to trigger

            return isConnected;
        }

        // Every <duration> seconds, stop the listener and start a new one for the same duration
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

        // Generic message handler. Call the appropriate callback
        void MessageHandler(string message)
        {
            Debug.Log("Message: " + message);
            bool foundHandler = false;
            foreach(string h in messageHandlers.Keys) // For all applicable keys
            {
                if (message.Contains(h))
                {
                    messageHandlers[h](message); // Call the callback
                    foundHandler = true;
                }
            }
            /*if (message.Contains("TRIGGER"))
            {
                TriggerHandler(message);
            }*/
            // Restart the listener
            if(!foundHandler)
            {
                StopListener();
                StartListener();
            }
        }

        // Reset the golf mechanism
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

        // Not able to read the message for some reason. Reset everything
        void ReadErrorHandler()
        {
            Debug.Log("Error!");
            StopListener();
            ResetTrigger();
            StartListener();
        }

        // Reset the golf mechanism
        void ResetTrigger()
        {
            WriteToArduino("RESETTRIGGER", 1); // Send an immediate request to reset
            //WriteToArduino("RESETTRIGGER");
        }

        // Stop the listener coroutine
        void StopListener()
        {
            if (listenerCoroutine != null)
            {
                StopCoroutine(listenerCoroutine);
            }
        }

        // Start listening for messages, with a timeout period
        void StartListener(float time = float.PositiveInfinity)
        {
            if (ArduinoHandler != null)
            {
                Debug.Log("Starting Arduino Listener");
                System.Action<string> M = (string s) => this.MessageHandler(s);  // Use the generic message handler
                System.Action f = () => ReadErrorHandler(); // In case there's an error, call this
                
                listenerCoroutine = StartCoroutine(ArduinoHandler.AsynchronousReadFromArduino(
                    M, f, time)); // Start listening
            } else
            {
                //Debug.Log("Arduino handler null");
            }
        }

        // Write the given message to the arduino. Will write immediately if priority is > 0
        public void WriteToArduino(string message, int priority = 0)
        {
            // Only send on the pulse interval, or if it is important
            if (Time.time - prevTime > pulse || priority > 0)
            {
                if (ArduinoHandler != null)
                {
                    ArduinoHandler.WriteToArduino(message);
                    prevTime = Time.time + pulse;
                }
            }
        }

        // Shut everything down cleanly
        void OnDisable()
        {
            WriteToArduino("RESETSTEP", 1);
            WriteToArduino("RESETTRIGGER", 1);
            if(ArduinoHandler != null && !ArduinoHandler.IsOpen())
                ArduinoHandler.Close();
        }
    }
}