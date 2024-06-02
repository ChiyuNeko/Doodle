using UnityEngine;

public class ObjectMovementController : MonoBehaviour
{
    public float moveSpeed ;
    public float rotationSpeed = 100.0f;
    public BluetoothDataReceiver bluetoothDataReceiver;

    public Rigidbody rb;

    public Vector3 acceleration;
    private Vector3 velocity = Vector3.zero;
    private Vector3 position = Vector3.zero;
    private Quaternion rotation;

    private float lastUpdateTime;

    public Vector3 gravity = new Vector3(0, 0, 0); // 假设重力方向为Y轴向下
    public float alpha = 0.1f; // 低通滤波参数
    public float noiseThreshold = 0.02f; // 噪声阈值
    void Start()
    {
        // 初始化时间
        lastUpdateTime = Time.time;
        position = rb.position;
    }

    void Update()
    {
        float currentTime = Time.time;
        float deltaTime = currentTime - lastUpdateTime;

        if (bluetoothDataReceiver != null)
        {
            // 获取加速度计和陀螺仪的值
            acceleration.x = (float) bluetoothDataReceiver.ax * 9.81f / 16384f;
            acceleration.z = (float)bluetoothDataReceiver.ay * 9.81f / 16384f;
            acceleration.y = (float)bluetoothDataReceiver.az * 9.81f / 16384f;

            float qw = bluetoothDataReceiver.qw;
            float qx = bluetoothDataReceiver.qx;
            float qy = bluetoothDataReceiver.qy;
            float qz = bluetoothDataReceiver.qz;

            // 简单的低通滤波器处理加速度数据
            Vector3 filteredAcceleration = Vector3.Lerp(acceleration, gravity, alpha);
            Vector3 accelerationWithoutGravity = filteredAcceleration - gravity;

            // 噪声阈值处理
            if (Mathf.Abs(accelerationWithoutGravity.magnitude) < noiseThreshold)
            {
                accelerationWithoutGravity = Vector3.zero;
            }
            // 调整加速度以适应移动速度
            accelerationWithoutGravity *= moveSpeed;

            velocity += acceleration * deltaTime;
            position += velocity * deltaTime;

            // 使用移动速度更新物体位置
            rb.MovePosition(position);
            lastUpdateTime = currentTime;
            //transform.Translate(acceleration * Time.deltaTime);

            // 创建四元数
            Quaternion rotation = new Quaternion(qx, qy, qz, qw);

            // 使用四元数旋转物体
            rb.MoveRotation(rotation);

            Debug.Log($"Acceleration: ({filteredAcceleration.x}, {filteredAcceleration.z}, {filteredAcceleration.y}), QUATERNION: ({qw}, {qx}, {qy}, {qz})");
        }
        else
        {
            Debug.LogWarning("BluetoothDataReceiver instance is not assigned.");
        }

       
    }
}
