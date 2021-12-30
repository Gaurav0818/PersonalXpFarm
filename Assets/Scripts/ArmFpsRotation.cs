using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmFpsRotation : MonoBehaviour
{

    public float minVertical = -10.0f;
    public float maxVertical = 10.0f;
    public float sensVertical = 0.5f;
    
    
    public float rotationX = 0;
    
    
    private void Update()
    {
        rotationX -= Input.GetAxis("Mouse Y") * sensVertical;
        rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);

        float rotationY = transform.localEulerAngles.y;
        rotationX = transform.localEulerAngles.x + rotationX;
        float rotationZ = transform.localEulerAngles.z;
        transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        print("abc");
    }
}
