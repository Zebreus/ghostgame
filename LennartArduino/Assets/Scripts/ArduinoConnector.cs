/* ArduinoConnector by Alan Zucconi
* http://www.alanzucconi.com/?p=2979
*/
using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class ArduinoConnector : MonoBehaviour
{

    /* The serial port where the Arduino is connected. */
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM4";
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;

    private string test;
    private SerialPort stream;

    public void Start()
    {
        Open();
    }

    public IEnumerator Corroutine()
    {
        bool once = false;
        while (!once)
        {
            yield return new WaitForSeconds(5f);
            WriteToArduino("Hi\n");
            once = true;
        }

    }

    public void Open()
    {
        // Opens the serial port
        stream = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One) { ReadTimeout = 50 };
        stream.Open();
        //this.stream.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
    }

    public void WriteToArduino(string message)
    {
        stream.Write(message);
        stream.BaseStream.Flush();
    }

    public void WriteToArduino(int byteMessage)
    {
        byte[] array = new byte[1];
        array[0] = (byte)byteMessage;
        stream.Write(array, 0, 1);
        //stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout)
    {
        stream.ReadTimeout = timeout;
        try
        {
            test = stream.ReadLine();
            return test;
        }
        catch (TimeoutException)
        {
            return null;
        }
    }


    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            // A single read attempt
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
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    public void Close()
    {
        stream.Close();
    }
}