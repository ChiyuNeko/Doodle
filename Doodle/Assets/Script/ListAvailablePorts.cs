using System.IO.Ports;
using UnityEngine;

public class ListAvailablePorts : MonoBehaviour
{
    void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            Debug.Log("Available port: " + port);
        }
    }
}
