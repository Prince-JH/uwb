// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO.Ports;
// using System.Net.NetworkInformation;
// using System.Text;
// using System.Threading;
// using UnityEngine;

// public class SpineController : MonoBehaviour//Photon.MonoBehaviour, IPunObservable, ISensorController
// {
//     public GameObject SpineSensor;
//     public SpineSerial spineSerial;
//     public int State = 1;

//     private bool can_lyingon = true;
//     private float creeping_factor = 0.3f;
//     public float forward_scalar = 0;

//     PhotonView view;

//     // Use this for initialization
//     void Start()
//     {

//         try
//         {
//             view = GetComponentInParent<PhotonView>();

//             if (view.isMine)
//             {
//                 spineSerial = new SpineSerial();
//                 SerialManager.Instance.AddSerialPort(spineSerial);
//             }
//         }
//         catch (System.Exception e)
//         {
//             ULogger.Error(e.Message + e.StackTrace);
//         }

//     }
//     private void OnEnable()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (view.isMine)
//         {
//             if (spineSerial != null && spineSerial.isConnect)
//             {
//                 State = spineSerial.State;
//             }

//             if (State == 0) //정지
//             {
//                 this.transform.position = SpineSensor.transform.position;
//                 this.transform.rotation = SpineSensor.transform.rotation;
//             }
//             else if (State == -1) // 후진
//             {
//                 this.transform.position = SpineSensor.transform.position;
//                 this.transform.up = -SpineSensor.transform.up;
//             }
//             else //걷기
//             {
//                 if (LyingOnFloor()) //포복
//                 {
//                     State = 2;
//                 }
//                 else //걷기
//                 {
//                     State = 1;
//                 }
//                 this.transform.position = SpineSensor.transform.position;
//                 this.transform.rotation = SpineSensor.transform.rotation;
//             }
//         }
//     }
//     void OnDestroy()
//     {
//         try
//         {
//             if (view.isMine)
//             {
//                 SerialManager.Instance.RemoveSerialPort(spineSerial);
//                 //spineSerial.Disconnect();
//             }
//         }
//         catch (System.Exception e)
//         {
//             ULogger.Error(e.Message + e.StackTrace);
//         }
//     }

//     public int GetState()
//     {
//         return State;
//     }
//     /// <summary>
//     /// 포복 여부 판단
//     /// </summary>
//     /// <returns>true: 포복</returns>
//     public bool LyingOnFloor()
//     {
//         if (!can_lyingon)
//         {
//             return false;
//         }

//         //포복이 되는 자세의 기준
//         //- Spine 축 중 절대좌표계 상의 Up에 근접 축 검색
//         //근접 축에 따른 엎드림 자세 등의 판별
//         bool res = false;
//         //Vector3 up = Spine.transform.up.normalized;
//         Vector3 forward = SpineSensor.transform.forward.normalized;
//         //Vector3 right = Spine.transform.right.normalized;
//         Vector3 world_up = Vector3.up.normalized;
//         //float up_scalar = Vector3.Dot(world_up, up);
//         forward_scalar = Vector3.Dot(world_up, forward);
//         //float right_scalar = Vector3.Dot(world_up, right);
//         //Debug.Log("up_scalr : " + up_scalar.ToString());        
//         //Debug.Log("forward_scalar : " + forward_scalar.ToString());
//         //Debug.Log("right_scalar : " + right_scalar.ToString());
//         if (Mathf.Abs(forward_scalar) < creeping_factor)
//         {
//             res = true;
//         }
//         return res;
//     }
//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         if (stream.isWriting)
//         {
//             //Debug.Log("OnPhotonSerializeView");
//             stream.SendNext(State);
//         }
//         else
//         {
//             State = (int)stream.ReceiveNext();
//         }
//     }

//     /// <summary>
//     /// Sensor 파라미터 기록을 위한 함수들, ISensorController 인터페이스 참고
//     /// </summary>
//     /// <returns></returns>
//     public string[] GetData()
//     {
//         string[] obj = new string[1];
//         obj[0] = State.ToString();
//         return obj;
//     }
//     /// <summary>
//     /// Sensor 파라미터 기록을 위한 함수들, ISensorController 인터페이스 참고
//     /// </summary>
//     /// <returns></returns>
//     public void SetData(string[] objs)
//     {
//         State = int.Parse(objs[0]);
//     }
// }

// public class SpineSerial : SerialConnector
// {
//     private int state = 1;
//     public int State
//     {
//         get
//         {
//             if (serialPort.IsOpen)
//             {
//                 return state;
//             }
//             else
//             {
//                 return 1;
//             }
//         }
//     }
//     private float forward = 0;
//     private float backward = 0;
//     public SpineSerial()
//     {
//         try
//         {
//             string SpineSerial = ClientSettings.ClientSetting.DeviceSrl.SpineSerial;
//             string SpineSerialBaudRate = ClientSettings.ClientSetting.DeviceSrl.SpineSerialBaudRate;
//             string forwardstr = ClientSettings.ClientSetting.DeviceSrl.SpineSerialForward;
//             string backwardstr = ClientSettings.ClientSetting.DeviceSrl.SpineSerialBackward;

//             forward = float.Parse(forwardstr);
//             backward = float.Parse(backwardstr);

//             SetSerialPort(int.Parse(SpineSerial), int.Parse(SpineSerialBaudRate), Received, Send);

//             ULogger.Debug("<SpineSerialSetting:> COM" + SpineSerial + " BaudRate: " + SpineSerialBaudRate + " Forward:" + forward.ToString() + " Backward:" + backward.ToString());
//         }
//         catch (Exception e)
//         {
//             ULogger.Error(e.StackTrace + "\n" + e.Message);
//         }
//     }
//     public void Received()
//     {
//         try
//         {
//             if (serialPort.IsOpen)
//             {
//                 string receivedData = serialPort.ReadLine();
//                 //string receivedData = "0.01 0.00 0.00 0.00 0 0 0 0@\r";

//                 if (receivedData == null || receivedData == string.Empty)
//                 {
//                     return;
//                 }

//                 receivedData = receivedData.Trim();
//                 string[] received = receivedData.Split(' ', '@');
//                 if (received.Length > 4)
//                 {
//                     try
//                     {
//                         float value = float.Parse(received[3]);

//                         if (value >= forward)
//                         {
//                             state = 1;
//                         }
//                         else if (value < forward && value >= backward)
//                         {
//                             state = 0;
//                         }
//                         else if (value < backward)
//                         {
//                             state = -1;
//                         }
//                     }
//                     catch (Exception e)
//                     {
//                         ULogger.Error(e.StackTrace + "\n" + e.Message);
//                         state = 1;
//                     }
//                 }
//             }
//         }
//         catch (TimeoutException e)
//         {

//         }
//         catch (Exception e)
//         {
//             ULogger.Error(e.StackTrace + "\n" + e.Message);
//         }
//         finally
//         {
//             serialPort.DiscardInBuffer();
//         }
//     }
//     public void Send()
//     {

//     }
// }