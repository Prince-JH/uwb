using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Threading;

public class UWB : MonoBehaviour
{
    private Thread receiveThread;
    private const int THREAD_SLEEP_TIME = 10;
    private SerialPort serialPort;
    private string input1;
    private string input2;
    private string temp1;
    private string temp2;
    private string tag1_xGrid;
    private string tag1_yGrid;
    private string tag1_zGrid;
    private string tag2_xGrid;
    private string tag2_yGrid;
    private string tag2_zGrid;
    private float tag1_distance;
    private float tag2_distance;
    private Vector3 tag1_NewPos;
    private Vector3 tag2_NewPos;

    float timer;
    float waitingTime;

    public GameObject tag1;
    public GameObject tag2;

    void Start()
    {
        //별도의 쓰레드로 계속 읽다가 업데이트 시점에서는 쓰레드에 들어있는 값을 참조
        tag1_xGrid = tag1.transform.position.x.ToString();
        tag1_yGrid = tag1.transform.position.z.ToString();
        tag1_zGrid = tag1.transform.position.y.ToString();

        tag2_xGrid = tag2.transform.position.x.ToString();
        tag2_yGrid = tag2.transform.position.z.ToString();
        tag2_zGrid = tag2.transform.position.y.ToString();

        serialPort = new SerialPort("COM7", 115200, Parity.None, 8, StopBits.One);
        timer = 0.0f;
        waitingTime = 0.25f;

        try
        {
            serialPort.Open();
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            Debug.Log("Port access denied");
        }
        receiveThread = new Thread(new ThreadStart(Receive));
        receiveThread.Start();
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        MoveCheck();
    }

    private void Receive()
    {
        while (serialPort.IsOpen)
        {
            input1 = null;
            input2 = null;
            serialPort.ReadLine();
            temp1 = serialPort.ReadLine();
            temp2 = serialPort.ReadLine();
            serialPort.ReadLine();
            if (temp1 == null || temp1 == string.Empty)
            {
                return;
            }
            if (temp2 == null || temp2 == string.Empty)
            {
                return;
            }
            for (int i = 0; i < temp1.Length; i++)
            {
                input1 += temp1[i];
            }
            for (int i = 0; i < temp2.Length; i++)
            {
                input2 += temp2[i];
            }
            if (timer > waitingTime)
            {
                timer = 0;
                string inputFirstLeftSplit = input1.Split('[')[1];
                string inputFirstRightSplit = inputFirstLeftSplit.Split(']')[0];

                string inputSecondLeftSplit = input2.Split('[')[1];
                string inputSecondRightSplit = inputSecondLeftSplit.Split(']')[0];

                tag1_xGrid = inputFirstRightSplit.Split(',')[0];
                tag1_yGrid = inputFirstRightSplit.Split(',')[1];
                tag1_zGrid = inputFirstRightSplit.Split(',')[2];

                tag2_xGrid = inputSecondRightSplit.Split(',')[0];
                tag2_yGrid = inputSecondRightSplit.Split(',')[1];
                tag2_zGrid = inputSecondRightSplit.Split(',')[2];

                // string inputSecondLeftSplit = inputFirstLeftSplit.Split(']')[1];
                // string inputSecondRightSplit = inputSecondLeftSplit.Split(']')[0];
                // if(tag == 2)
                // {
                //     xGrid = inputFirstRightSplit.Split(',')[0];
                //     yGrid = inputFirstRightSplit.Split(',')[1];
                //     zGrid = inputFirstRightSplit.Split(',')[2];
                // }

                // Debug.Log("x좌표: " + xGrid);
                // Debug.Log("y좌표: " + zGrid);
                // Debug.Log("z좌표: " + yGrid);
            }
            Thread.Sleep(THREAD_SLEEP_TIME);
        }
    }
    // private void Update()
    // {
    //     timer += Time.deltaTime;
    //     WriteLog();
    // }
    // private void Receive()
    // {
    //     while (serialPort.IsOpen)
    //     {
    //         temp += serialPort.ReadLine();
    //         Thread.Sleep(THREAD_SLEEP_TIME);
    //     }
    // }
    // private void WriteLog()
    // {
    //     if(timer > 1)
    //     {
    //         timer = 0;
    //         Debug.Log(temp);
    //     }
    // }
    //이동할지 체크
    private void MoveCheck()
    {
        tag1_NewPos = new Vector3(float.Parse(tag1_xGrid), tag1.transform.position.y, float.Parse(tag1_yGrid));
        tag2_NewPos = new Vector3(float.Parse(tag2_xGrid), tag2.transform.position.y, float.Parse(tag2_yGrid));

        tag1_distance = Vector3.Distance(tag1.transform.position, tag1_NewPos);
        tag2_distance = Vector3.Distance(tag2.transform.position, tag2_NewPos);
        
        if (tag1_distance >= 0.1f || tag2_distance >= 0.1f)
            StartCoroutine(Move());
    } 
    //부드럽게 이동
    private IEnumerator Move()
    {
        tag1.transform.position = Vector3.Lerp(tag1.transform.position, tag1_NewPos, 0.01f);
        tag2.transform.position = Vector3.Lerp(tag2.transform.position, tag2_NewPos, 0.01f);
        yield return null;
    }
    // private IEnumerator Tag1_Move()
    // {
    //     tag1.transform.position = Vector3.Lerp(tag1.transform.position, tag1_NewPos, 0.01f);
    //     yield return null;
    // }
    // private IEnumerator Tag2_Move()
    // {
    //     tag2.transform.position = Vector3.Lerp(tag2.transform.position, tag2_NewPos, 0.01f);
    //     yield return null;
    // }
    private void OnApplicationQuit()
    {
        receiveThread.Abort();
        serialPort.Close();
    }
}
