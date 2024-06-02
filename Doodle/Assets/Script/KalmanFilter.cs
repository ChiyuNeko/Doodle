using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalmanFilter
{
        private float Q_angle; // Process noise variance for the accelerometer
        private float Q_bias; // Process noise variance for the gyro bias
        private float R_measure; // Measurement noise variance

        private float angle; // The angle calculated by the Kalman filter
        private float bias; // The gyro bias calculated by the Kalman filter
        private float rate; // Unbiased rate calculated from the rate and the calculated bias

        private float P00, P01, P10, P11; // Error covariance matrix

        public KalmanFilter()
        {
            Q_angle = 0.001f;
            Q_bias = 0.003f;
            R_measure = 0.03f;

            angle = 0f;
            bias = 0f;

            P00 = 0f;
            P01 = 0f;
            P10 = 0f;
            P11 = 0f;
        }

        public float GetAngle(float newAngle, float newRate, float dt)
        {
            // Predict
            rate = newRate - bias;
            angle += dt * rate;

            P00 += dt * (dt * P11 - P01 - P10 + Q_angle);
            P01 -= dt * P11;
            P10 -= dt * P11;
            P11 += Q_bias * dt;

            // Update
            float S = P00 + R_measure;
            float K0 = P00 / S;
            float K1 = P10 / S;

            float y = newAngle - angle;
            angle += K0 * y;
            bias += K1 * y;

            float P00_temp = P00;
            float P01_temp = P01;

            P00 -= K0 * P00_temp;
            P01 -= K0 * P01_temp;
            P10 -= K1 * P00_temp;
            P11 -= K1 * P01_temp;

            return angle;
        }
}


