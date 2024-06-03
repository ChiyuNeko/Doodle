using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer line;
    public float minDistance = 0.1f;
    public float width;
    private Vector3 PreviousPos;
    public Transform cursor;

        
        void Start()
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = 1;
            PreviousPos = transform.position;
            line.startWidth = line.endWidth = width;
        }

        void Update()
        {
            Vector3 currentPos = cursor.position;
            if(Input.GetKey(KeyCode.Space))
             {
                if(Vector3.Distance(currentPos, PreviousPos) > minDistance)
                {
                    if(PreviousPos == transform.position)
                    {
                        line.SetPosition(0, currentPos);
                    }
                    else
                    {

                        line.positionCount++;
                        line.SetPosition(line.positionCount - 1, currentPos);
                    }
                    PreviousPos = currentPos;
                }
             }    

        }
}
