using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

namespace Arduino
{
    public class USB
    {

        public string port = "COM3";

        public USB(string port)
        {
            this.port = port;
        }

        public int baudrate = 115200;

        private SerialPort stream;

        WaitForSeconds waitForSeconds;

        // Open the stream
        public void Open()
        {
            stream = new SerialPort(port, baudrate);
            stream.ReadTimeout = 10;
            stream.DtrEnable = true;
            stream.Open();

            //SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public bool IsOpen()
        {
            return stream.IsOpen;
        }

        public void WriteToArduino(string message)
        {
            if (stream.IsOpen)
            {
                stream.WriteLine(message);
                stream.BaseStream.Flush();
            }
        }

        // Synchronous read from arduino (not used)
        public string ReadFromArduino(int timeout = 0)
        {
            stream.ReadTimeout = timeout;

            try
            {
                return stream.ReadLine();
            }
            catch
            {
                return null;
            }
        }

        // Async read from arduino for timeout time.
        public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
        {
            DateTime initialTime = DateTime.Now;
            DateTime nowTime;
            TimeSpan diff = default(TimeSpan);

            string dataString = null;

            waitForSeconds = new WaitForSeconds(0.05f);

            do
            {
                try
                {
                    dataString = stream.ReadLine();
                }
                catch (TimeoutException)
                {
                    dataString = null;
                }
                if (dataString != null)
                {
                    Debug.Log("Data string not null!");
                    callback(dataString); // This is a good message!
                    yield return null;
                }
                else
                {
                    yield return waitForSeconds;
                }

                nowTime = DateTime.Now;
                diff = nowTime - initialTime;
            } while (diff.Milliseconds < timeout); // Keep going until time out

            if (fail != null)
            {
                Debug.Log("Fail!");
                fail(); // It was not able to read anything before timing out
            }
            yield return null;
        }

        public void Close()
        {
            stream.Close();
        }
    }
}