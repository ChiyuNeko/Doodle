using UnityEngine;
using System.IO.Ports;

public class MPU6050Reader : MonoBehaviour
{
    private SerialPort serialPort;
    public Rigidbody trackedRigidbody;
    public float moveSpeed = 5.0f;

    private Vector3 linearVelocity = Vector3.zero;

    private void Start()
    {
        string portName = "COM9"; // Modify according to your device manager
        serialPort = new SerialPort(portName, 115200);

        // Set read and write timeouts
        serialPort.ReadTimeout = 10000;
        serialPort.WriteTimeout = 10000;

        try
        {
            serialPort.Open();
            Debug.Log("Serial Port Opened");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open Serial Port: " + e.Message);
        }
    }

    private void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                // Get accelerometer and gyroscope data
                Vector3 accelerometer = GetAccelData();
                Vector3 gyroscope = GetGyroData();

                // Convert Arduino units to Unity units
                accelerometer *= 9.81f; // Convert from m/s^2 to Unity's unit (m/s^2)
                gyroscope *= Mathf.Deg2Rad; // Convert from deg/s to Unity's unit (rad/s)

                // Calculate linear acceleration
                Vector3 linearAcceleration = CalculateLinearAcceleration(accelerometer, gyroscope);

                // Apply linear acceleration as force to the rigidbody
                trackedRigidbody.AddForce(linearAcceleration, ForceMode.Acceleration);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to read from Serial Port: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Serial port is not open.");
        }
    }

    private Vector3 GetAccelData()
    {
        // Read accelerometer data from serial port
        string data = serialPort.ReadLine();
        string[] values = data.Split(',');
        if (values.Length == 3)
        {
            float ax = float.Parse(values[0]);
            float ay = float.Parse(values[1]);
            float az = float.Parse(values[2]);
            return new Vector3(ax, az, ay);
        }
        else
        {
            // Return zero vector if data is invalid
            return Vector3.zero;
        }
    }

    private Vector3 GetGyroData()
    {
        // Read gyroscope data from serial port
        string data = serialPort.ReadLine();
        string[] values = data.Split(',');
        if (values.Length == 3)
        {
            float gx = float.Parse(values[3]);
            float gy = float.Parse(values[4]);
            float gz = float.Parse(values[5]);
            return new Vector3(gx, gz, gy);
        }
        else
        {
            // Return zero vector if data is invalid
            return Vector3.zero;
        }
    }

    private Vector3 CalculateLinearAcceleration(Vector3 accelerometer, Vector3 gyroscope)
    {
        // Assuming the gravity is removed from accelerometer data
        Vector3 gravity = new Vector3(0f, 9.81f, 0f);
        Vector3 worldAccel = accelerometer - gravity;

        // Use gyro data to compensate for rotation
        Vector3 linearAccel = worldAccel;

        return linearAccel;
    }

    private void OnApplicationQuit()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial Port Closed");
        }
    }
}
