using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer line;
    public float minDistance = 0.1f;
    private Vector3 PreviousPos;
    public Transform cursor;
        
        void Start()
        {
            line = GetComponent<LineRenderer>();
            PreviousPos = transform.position;
        }

        void Update()
        {
            // if(Input.GetKey(KeyCode.Space))
            // {
                Vector3 currentPos = cursor.position;
                if(Vector3.Distance(currentPos, PreviousPos) > minDistance)
                {
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, currentPos);
                    PreviousPos = currentPos;
                }
            // }    

        }
}
