using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.ComponentModel;

public class KalmanFilterTest
{
    public float Q = 0.001f; // 过程噪声协方差
    public float R = 0.01f;  // 测量噪声协方差
    public float A = 1;      // 状态转移矩阵
    public float B = 1;      // 控制输入影响矩阵
    public float C = 1;      // 测量矩阵

    private float cov = float.MaxValue; // 估计的协方差
    private float x = 0; // 估计的状态

    public float Update(float measurement, float controlInput)
    {
        // 预测
        x = A * x + B * controlInput;
        cov = A * cov * A + Q;

        // 更新
        float K = cov * C / (C * cov * C + R);
        x = x + K * (measurement - C * x);
        cov = (1 - K * C) * cov;

        return x;
    }
}

public class MotionTracking : MonoBehaviour
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
    public float smooth;
    public int counter;
    public bool stop;
    public int recoverCount;
    
    

    void Start()
    {
        KalmanFilterTest kalmanFilter = new KalmanFilterTest(); // 正确初始化 KalmanFilterTest

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

        currentVelocity.x = kalmanFilterX.Update(currentAcceleration.x, deltaTime);
        currentVelocity.y = kalmanFilterY.Update(currentAcceleration.y, deltaTime);
        currentVelocity.z = kalmanFilterZ.Update(currentAcceleration.z, deltaTime);

        if(MathF.Abs(currentVelocity.x) < 0.5)
                    currentVelocity.x = 0;
                if(MathF.Abs(currentVelocity.y) < 0.5)
                    currentVelocity.y = 0;
                if(MathF.Abs(currentVelocity.z) < 0.5)
                    currentVelocity.z = 0;

        

        //currentPosition += currentVelocity * deltaTime;
        //transform.position = currentPosition;

        // if(lx == 0)
        // {
        //     stop = true;
        //     counter = 0;
        // }
        // if(currentVelocity.x <0 && stop && counter < recoverCount)
        // {
        //     Debug.Log("+++++++++++++++++++++++++++++++");
        //     gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        //     counter++;
        //     if(counter == recoverCount)
        //     {
        //         stop = false;
        //         Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        //     }
        // }
        // else if(currentVelocity.x > 0 && stop && counter < recoverCount)
        // {
        //     Debug.Log("-------------------------------------");
        //     gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        //     counter++;
        //     if(counter == recoverCount)
        //     {
        //         stop = false;
        //         Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        //     }
        // }

        lx =  currentVelocity.x;


        
        gameObject.GetComponent<Rigidbody>().velocity = currentVelocity ;
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
                float ax = float.Parse(values[0])/100f;
                float ay = float.Parse(values[1])/100f;
                float az = float.Parse(values[2])/100f;

               

                if(MathF.Abs(ax) < 1)
                    ax = 0;
                if(MathF.Abs(ay) < 1)
                    ay = 0;
                if(MathF.Abs(az) < 1)
                    az = 0;
                
                currentAcceleration = new Vector3(ax , az , ay);

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
