using System;
using System.IO.Ports;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private SerialPort serialPort;
    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 position;
    private Quaternion rotation;
    public float ax, ay, az, qw, qx, qy, qz;
    public string[] values;

    void Start()
    {
        string portName = "COM9"; // 根据设备管理器中的信息修改
        serialPort = new SerialPort(portName, 115200);

        // 设置读取和写入超时时间
        serialPort.ReadTimeout = 10000;
        serialPort.WriteTimeout = 10000;

        try
        {
            serialPort.Open();
            Debug.Log("Serial Port Opened");
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError("Failed to open Serial Port: 存取被拒。" + e.Message);
        }
        catch (TimeoutException e)
        {
            Debug.LogError("Failed to open Serial Port: 信号等待逾时。" + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open Serial Port: " + e.Message);
        }

        // 初始化变量
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine();
                Debug.Log("Data received: " + data);
                ParseData(data);
                UpdatePositionAndRotation();
            }
            catch (TimeoutException e)
            {
                Debug.LogWarning("Read from Serial Port timed out: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to read from Serial Port: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Serial port is not open.");
        }

        // 更新物体的Transform
        transform.position = position;
        transform.rotation = rotation;
    }

    void ParseData(string data)
    {
        string[] values = data.Split(',');

        if (values.Length == 7)
        {
            // 解析加速度数据
            acceleration.x = float.Parse(values[0]) * 9.81f / 16384f; // 将加速度计的数据转换为m/s²
            acceleration.y = float.Parse(values[1]) * 9.81f / 16384f;
            acceleration.z = float.Parse(values[2]) * 9.81f / 16384f;

            // 解析四元数数据
            float qw = float.Parse(values[3]);
            float qx = float.Parse(values[4]);
            float qy = float.Parse(values[5]);
            float qz = float.Parse(values[6]);

            rotation = new Quaternion(qx, qy, qz, qw);
        }
    }

    void UpdatePositionAndRotation()
    {
        float deltaTime = Time.deltaTime;

        // 计算速度和位置
        velocity += acceleration * deltaTime;
        position += velocity * deltaTime;
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial Port Closed");
        }
    }
}
