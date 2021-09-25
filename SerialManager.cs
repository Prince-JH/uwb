// using System.Collections;
// using System.Collections.Generic;
// using System.IO.Ports;
// using System.Threading;
// using UnityEngine;
// using System.Threading.Tasks;

// public class SerialManager : MonoBehaviour
// {
//     public delegate void SerialEvent();

//     private static SerialManager _instance = null;
//     public static SerialManager Instance
//     {
//         get
//         {
//             return _instance;
//         }
//     }

//     private List<SerialConnector> serialConnectors = new List<SerialConnector>();
//     protected Thread ReceiveThread, SendThread;
//     protected const int THREAD_SLEEP_TIME = 10;
//     //Coroutine Coroutine;

//     //protected Task ReceiveTask, SendTask;
//     protected System.Action<List<SerialConnector>> ReceiveAction, SendAction;

//     private void Awake()
//     {
//         DontDestroyOnLoad(this);
//         _instance = this;        
//         ReceiveThread = new Thread(new ThreadStart(Receive));
//         ReceiveThread.Start();
//         SendThread = new Thread(new ThreadStart(Send));
//         SendThread.Start();
//     }
//     private void OnEnable()
//     {
        
//     }

//     private void OnDestroy()
//     {
//         DisconnectAll();
//     }

//     private void OnApplicationQuit()
//     {
//         DisconnectAll();
//     }

//     private void DisconnectAll()
//     {
//         try
//         {
//             //if (Coroutine != null)
//             //    StopCoroutine(Coroutine);

//             int size = serialConnectors.Count;

//             for (int i = 0; i < size; i++)
//             {
//                 try
//                 {
//                     if (!serialConnectors[i].isConnect && serialConnectors[i].hasPort)
//                     {
//                         serialConnectors[i].Disconnect();
//                     }
//                 }
//                 catch (System.Exception e)
//                 {
//                     ULogger.Error(e.Message + e.StackTrace);
//                 }
//             }

//             if (ReceiveThread.IsAlive)
//                 ReceiveThread.Abort();
//             if (SendThread.IsAlive)
//                 SendThread.Abort();
//         }
//         catch (System.Exception e)
//         {
//             ULogger.Error(e.Message + e.StackTrace);
//         }
//     }

//     public void AddSerialPort(SerialConnector aSerialConnector)
//     {
//         serialConnectors.Add(aSerialConnector);
//     }

//     public void RemoveSerialPort(SerialConnector aSerialConnector)
//     {
//         try
//         {
//             aSerialConnector.Disconnect();
//             serialConnectors.Remove(aSerialConnector);
//         }
//         catch(System.Exception e){
//             ULogger.Error(e.Message + e.StackTrace);
//         }
//     }

//     // public IEnumerator StartConnection()
//     // {
//     //     while (true)
//     //     {
//     //         // if (serialConnectors.Count > 0)
//     //         // {
//     //         //     int size = serialConnectors.Count;

//     //         //     if (size > 0)
//     //         //     {
//     //         //         for (int i = 0; i < size; i++)
//     //         //         {
//     //         //             try
//     //         //             {
//     //         //                 if (!serialConnectors[i].isConnect && serialConnectors[i].hasPort)
//     //         //                 {
//     //         //                     serialConnectors[i].Connection();
//     //         //                 }
//     //         //             }
//     //         //             catch (System.Exception e)
//     //         //             {
//     //         //                 ULogger.Error(e.StackTrace + e.Message);
//     //         //             }
//     //         //             yield return new WaitForSeconds(1.0f);
//     //         //         }
//     //         //     }
//     //         // }    
//     //         yield return new WaitForSeconds(0.01f);
//     //     }
//     // }
//     private void Receive()
//     {
//         while (true)
//         {
//             try
//             {
//                 int size = serialConnectors.Count;
//                 if (size > 0)
//                 {
//                     for (int i = 0; i < size; i++)
//                     {
//                         try
//                         {
//                             if (!serialConnectors[i].isConnect && serialConnectors[i].hasPort)//연결되지 않은 포트존재 커넥터들 연결 시도
//                             {
//                                 try
//                                 {
//                                     serialConnectors[i].Connection();                                    
//                                 }
//                                 catch(System.Exception e)
//                                 {
//                                     ULogger.Error("serialConnectors connection error:" + e.Message + e.StackTrace);
//                                 }
//                                 Thread.Sleep(THREAD_SLEEP_TIME);
//                             }
//                             else
//                             {
//                                 try
//                                 {
//                                     if (serialConnectors[i].isConnect && serialConnectors[i].hasPort && serialConnectors[i].OnReceived != null) // 연결되어있는 포트들에 대해서 수신 콜백 실행
//                                     {                                        
//                                         serialConnectors[i].OnReceived();
//                                     }
//                                 }
//                                 catch (System.Exception e)
//                                 {
//                                     ULogger.Error("OnReceive:" + e.Message + e.StackTrace);
//                                 }
//                                 Thread.Sleep(THREAD_SLEEP_TIME);
//                             }
//                         }
//                         catch (System.Exception e)
//                         {
//                             ULogger.Error(e.Message + e.StackTrace);
//                         }
//                     }
//                 }
//                 Thread.Sleep(THREAD_SLEEP_TIME);
//             }
//             catch (System.Exception e)
//             {
//                 ULogger.Error(e.Message + e.StackTrace);
//             }
//         }
//     }

//     private void Send()
//     {
//         while (true)
//         {
//             try
//             {
//                 int size = serialConnectors.Count;
//                 if (size > 0)
//                 {
//                     for (int i = 0; i < size; i++)
//                     {
//                         try
//                         {
//                             if (serialConnectors[i].isConnect && serialConnectors[i].hasPort && serialConnectors[i].OnSend != null)
//                             {
//                                 serialConnectors[i].OnSend();
//                             }
//                         }
//                         catch (System.Exception e)
//                         {
//                             ULogger.Error(e.Message + e.StackTrace);
//                         }
//                         Thread.Sleep(THREAD_SLEEP_TIME);
//                     }
//                 }
//                 Thread.Sleep(THREAD_SLEEP_TIME);
//             }
//             catch (System.Exception e)
//             {
//                 ULogger.Error(e.Message + e.StackTrace);
//             }
//         }
//     }
// }
