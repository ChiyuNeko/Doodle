using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;



public class MotionTrackinYPR : MonoBehaviour
{
    KalmanFilterTest kalmanFilterX = new KalmanFilterTest();
    KalmanFilterTest kalmanFilterY = new KalmanFilterTest();
    KalmanFilterTest kalmanFilterZ = new KalmanFilterTest();
    Vector3 currentAcceleration = Vector3.zero;
    Vector3 currentVelocity = Vector3.zero;
    Vector3 currentPosition = Vector3.zero;
    float deltaTime = 0.02f;

    private SerialPort serialPort;
    public Rigidbody trackedRigidbody;
    public float lx =0f;
    public Vector3 vec;
    public float distance;
    public float smooth;
    

    void Start()
    {

        string portName = "COM3"; // 根据设备管理器修改
        serialPort = new SerialPort(portName, 115200);

        // 设置读写超时
        serialPort.ReadTimeout = 10000;
        serialPort.WriteTimeout = 10000;

        try
        {
            serialPort.Open();
            Debug.Log("串口已打开");
        }
        catch (System.Exception e)
        {
            Debug.LogError("打开串口失败: " + e.Message);
        }
    }

    void Update()
    {
        // 更新加速度数据
        UpdateAccelerationData();

        currentVelocity.x = Mathf.Lerp(gameObject.transform.position.x, Mathf.Atan(currentAcceleration.x) * distance, 1f/smooth);
        currentVelocity.y = Mathf.Lerp(gameObject.transform.position.y, Mathf.Atan(currentAcceleration.y) * distance, 1f/smooth);
         //currentPosition += currentVelocity * deltaTime;
        //transform.position = currentPosition;

        
        gameObject.transform.position = currentVelocity;
        Debug.Log(currentVelocity);
    }

    void UpdateAccelerationData()
    {
        // 从串口读取加速度计数据
        string data;
        try
        {
            data = serialPort.ReadLine();
            string[] values = data.Split(',');
            if (values.Length == 3)
            {
                float ax = float.Parse(values[0]);
                float ay = float.Parse(values[1]);
                float az = float.Parse(values[2]);

               

                
                currentAcceleration = new Vector3(ax, az, ay);

                //Debug.Log($"{ax}, {az},{ay}");
                //Debug.Log(currentAcceleration);
                
            }
            else
            {
                // 如果数据无效，则返回零向量
                currentAcceleration = Vector3.zero;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("从串口读取数据失败: " + e.Message);
            
        }
    }
}
