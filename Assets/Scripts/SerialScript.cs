using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
// For Serial
using System.IO.Ports;
// For compare array
using System.Linq;

public class SerialScript : MonoBehaviour
{

    public string portName;
    public int baudRate = 9600;
    SerialPort arduinoSerial;
    public Text txtMessage;
    public string received_message;
    bool isDisconnected = true;

    List<byte> byteList = new List<byte>();

    // Start is called before the first frame update
    void Start()
    {
        connectDevice();
    }

    // Update is called once per frame
    void Update()
    {
        if (!arduinoSerial.IsOpen)
        {
            txtMessage.text = "Hardware not found";
            isDisconnected = true;
            connectDevice();
        }
        else
        {
            readSerial();
        }
    }

    public void connectDevice()
    {
        // Open Serial port
        arduinoSerial = new SerialPort(portName, baudRate);
        // Set buffersize so read from Serial would be normal
        arduinoSerial.ReadTimeout = 1;
        arduinoSerial.ReadBufferSize = 8192;
        arduinoSerial.WriteBufferSize = 128;
        arduinoSerial.ReadBufferSize = 4096;
        arduinoSerial.Parity = Parity.None;
        arduinoSerial.StopBits = StopBits.One;
        arduinoSerial.DtrEnable = true;
        arduinoSerial.RtsEnable = true; ;
        arduinoSerial.Open();
        if (arduinoSerial.IsOpen)
        {
            Debug.Log("Hardware Connected");
            txtMessage.text = "Hardware Connected";
            isDisconnected = false;
            sendSerial("Hi");
        }
    }

    public void disconnectDevice()
    {
        if (arduinoSerial.IsOpen)
        {
            arduinoSerial.Close();
            Debug.Log("Hardware Disconnected");
        }
    }

    public void readSerial()
    {
        string str = null;
        try
        {
            //read byte array from serial 
            do
            {
                var rxByte = (byte)arduinoSerial.ReadByte();
                //Console.WriteLine("New Byte: "+rxByte);
                // end of word
                if (rxByte != 10 && rxByte != 13)
                {
                    //Console.WriteLine("New Byte: " + rxByte);
                    byteList.Add(rxByte);
                    //Console.WriteLine("000  " + System.Text.Encoding.ASCII.GetString(new[] {rxByte }));
                }
                else
                {
                    string data = System.Text.Encoding.ASCII.GetString(byteList.ToArray());
                    if (data.Length > 0)
                    {
                        str = data.Trim();
                    }
                    byteList.Clear();
                }
            } while (arduinoSerial.BytesToRead > 0);

            /*str = arduinoSerial.ReadLine();*/ //read line by line 
            if(str!=null)
                received_message = str;
            Debug.Log(str);
            /*//int number;
            //if (Int32.TryParse(str, out number))
            {
                sensor = number;
            }*/
        }
        catch (TimeoutException e)
        {
        }
    }

    public void sendSerial(string message)
    {
        try
        {
            if (arduinoSerial.IsOpen)
            {
                //string[] strs = new string[2];

                // Convert all integers to 3-digit string
                //strs[0] = degs[0].ToString("000");
                //strs[1] = degs[1].ToString("000");

                // The string to write to Serial
                //string str = strs[0] + strs[1];
                //string str = "Hi";

                arduinoSerial.WriteLine(message);
                arduinoSerial.BaseStream.Flush();

                Debug.Log("Send Serial: " + message);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);
        }
    }

    void OnApplicationQuit()
    {
        disconnectDevice();
    }
}
