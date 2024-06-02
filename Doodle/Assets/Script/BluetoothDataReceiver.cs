using System;
using System.IO.Ports;
using UnityEngine;

public class BluetoothDataReceiver : MonoBehaviour
{
    SerialPort serialPort;
    public float ax, ay, az, qw, qx, qy, qz;
    public string[] values;
    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 position;
    private Quaternion rotation;

    void Start()
    {
        string portName = "COM9"; // 根據設備管理器中的信息修改
        serialPort = new SerialPort(portName, 115200);

        // 設置讀取和寫入超時時間
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
            Debug.LogError("Failed to open Serial Port: 信號等待逾時。" + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open Serial Port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine();
                Debug.Log("Data received: " + data);
                values = data.Split(',');

                if (values.Length == 7)
                {

                    if (float.TryParse(values[0], out float ax) &&
                    float.TryParse(values[1], out float ay) &&
                    float.TryParse(values[2], out float az) &&
                    float.TryParse(values[3], out float qw) &&
                    float.TryParse(values[4], out float qx) &&
                    float.TryParse(values[5], out float qy) &&
                    float.TryParse(values[6], out float qz))
                    {

                        // 在這裡處理接收到的數據
                       // Debug.Log($"Acceleration: ({ax}, {ay}, {az}), QUATERNION: ({qw}, {qx}, {qy}, {qz})");
                        // 将解析后的值赋给类的字段
                        this.ax = ax;
                        this.ay = ay;
                        this.az = az;
                        this.qw = qw;
                        this.qx = qx;
                        this.qy = qy;
                        this.qz = qz;
                        
              
                    }
                    else
                    {
                        Debug.LogWarning("Unexpected data format received.");
                    }
                }
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
