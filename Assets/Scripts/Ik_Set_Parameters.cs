using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ik_Set_Parameters : MonoBehaviour
{
    public float x;
    public float y;
    
    [Range(0,4)]
    public float xMinMax = 3;
    [Range(0,2)]
    public float yMinMax = 1.5f;
    [Range(0,5)]
    public float speed = 3;
    

    private Vector3 defaultPosition;

    private void Start()
    {
        defaultPosition.x = 0.0f;
        defaultPosition.y = 0.0f;
        defaultPosition.z = 2.5f;
    }

    private void Update()
    {
        //defaultPosition.z = transform.position.z;
        transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition + new Vector3(x, y, 0),speed*Time.deltaTime);
    }

    public void SetParameters(float xValue ,float yValue)
    {
        x = xValue * xMinMax;
        y = yValue * yMinMax;
    }
}
