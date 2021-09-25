using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Threading;

public class UWB_TAG2 : MonoBehaviour
{
    private Thread receiveThread;
    private const int THREAD_SLEEP_TIME = 10;
    private SerialPort serialPort;
    private string input;
    private string temp;
    private string xGrid;
    private string yGrid;
    private string zGrid;
    private float distance;
    private Vector3 newPos;

    float timer;
    float waitingTime;

    void Start()
    {
        //별도의 쓰레드로 계속 읽다가 업데이트 시점에서는 쓰레드에 들어있는 값을 참조
        xGrid = gameObject.transform.position.x.ToString();
        yGrid = gameObject.transform.position.z.ToString();
        zGrid = gameObject.transform.position.y.ToString();

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
            input = null;
            serialPort.ReadLine();
            serialPort.ReadLine();
            temp = serialPort.ReadLine();
            serialPort.ReadLine();
            if (temp == null || temp == string.Empty)
            {
                return;
            }
            for (int i = 0; i < temp.Length; i++)
            {
                input += temp[i];
            }
            Debug.Log(input);
            if (timer > waitingTime)
            {
                timer = 0;
                string inputFirstLeftSplit = input.Split('[')[1];
                string inputFirstRightSplit = inputFirstLeftSplit.Split(']')[0];

                // string inputSecondLeftSplit = input.Split('[')[1];
                // string inputSecondRightSplit = inputSecondLeftSplit.Split(']')[0];

                xGrid = inputFirstRightSplit.Split(',')[0];
                yGrid = inputFirstRightSplit.Split(',')[1];
                zGrid = inputFirstRightSplit.Split(',')[2];

                // string inputSecondLeftSplit = inputFirstLeftSplit.Split(']')[1];
                // string inputSecondRightSplit = inputSecondLeftSplit.Split(']')[0];
                // if(tag == 2)
                // {
                //     xGrid = inputFirstRightSplit.Split(',')[0];
                //     yGrid = inputFirstRightSplit.Split(',')[1];
                //     zGrid = inputFirstRightSplit.Split(',')[2];
                // }

                Debug.Log("x좌표: " + xGrid);
                Debug.Log("y좌표: " + zGrid);
                Debug.Log("z좌표: " + yGrid);
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
        newPos = new Vector3(float.Parse(xGrid), gameObject.transform.position.y, float.Parse(yGrid));
        distance = Vector3.Distance(gameObject.transform.position, newPos);
        if (distance >= 0.1f)
            StartCoroutine(Move());
    } 
    //부드럽게 이동
    private IEnumerator Move()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, newPos, 0.01f);
        yield return null;
    }
    private void OnApplicationQuit()
    {
        receiveThread.Abort();
        serialPort.Close();
    }
}
