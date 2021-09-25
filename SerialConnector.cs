// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO.Ports;
// using System.Text;
// using System.Threading;
// using UnityEngine;

// public class SerialConnector
// {
//     int PortNum = 0;
//     int BaudRate = 9600;
//     int readTimeout = 1000;

//     public SerialPort serialPort;
//     public delegate void SerialEvent();        
//     public SerialEvent OnReceived = null;
//     private Queue<string> sendQueue;
//     public SerialEvent OnSend = null;

//     public bool isConnect = false;
//     public bool hasPort = true;

//     public void SetSerialPort(int _PortNum, int _BaudRate, SerialEvent Receive = null, SerialEvent Send = null, int _readTimeOut = 100)
//     {
//         PortNum = _PortNum;
//         BaudRate = _BaudRate;
//         readTimeout = _readTimeOut;
//         OnReceived = Receive;
//         OnSend = Send;
//     }

//     public bool Connection()
//     {        
//         bool res = false;

//         if (!SerialConnector.IsPortAvailable("COM"+PortNum.ToString()))
//         {
//             ULogger.Error("COM" + PortNum.ToString() + " is Not available");
//             hasPort = false;
//             return res;
//         }

//         string prefix = "\\\\.\\";
//         //string prefix = string.Empty;
//         StringBuilder portBuilder = new StringBuilder();
//         if (PortNum > 9)
//             portBuilder.Append(prefix);
//         portBuilder.Append("COM");
//         portBuilder.Append(PortNum.ToString());
//         string PortNumStr = portBuilder.ToString();

//         //if (serialPort == null)
//         serialPort = new SerialPort(PortNumStr);
//         serialPort.BaudRate = BaudRate;
//         serialPort.ReadTimeout = readTimeout;        

//         if (!serialPort.IsOpen)
//         {
//             try
//             {                
//                 serialPort.Open();
//                 isConnect = serialPort.IsOpen;
//                 res = serialPort.IsOpen;
//                 if(res)
//                 {
//                     ULogger.Debug("SerialConnector Connection: " + serialPort.IsOpen + " Port: " + serialPort.PortName + " BaudRate: " + serialPort.BaudRate);
//                 }
//                 else
//                 {
//                     ULogger.Debug("Error| SerialConnector can't Connection: " + serialPort.IsOpen + " Port: " + serialPort.PortName + " BaudRate: " + serialPort.BaudRate);
//                 }
//             }
//             catch (System.Exception e)
//             {
//                 ULogger.Debug("SerialConnector Error : " + e.Message + " | " + e.GetType().ToString() + " | " + e.StackTrace);

//                 //hasPort = false;
//                 res = false;
//             }
//         }
//         return res;
//     }

//     public void Disconnect()
//     {
//         if (serialPort != null && serialPort.IsOpen)
//         {
//             try
//             {
//                 serialPort.Close();                
//             }
//             catch (Exception e)
//             {
//                 Debug.Log("serial can't closing " + e.ToString());
//             }
//         }
//     }

//     private static bool IsPortAvailable(string PortName)
//     {
//         bool res = false;
//         string[] PortNames = SerialPort.GetPortNames();
//         foreach (string aPortName in PortNames)
//         {            
//             if (aPortName == PortName)
//             {
//                 ULogger.Debug( PortName + " is available");
//                 res = true;
//             }
//         }
//         return res;
//     }
// }

