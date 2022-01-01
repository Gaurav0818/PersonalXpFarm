using UnityEngine;

public class Ik_Set_Parameters : MonoBehaviour
{
    public float x;
    public float y;
    
    [Range(0,10)]
    public float xMinMax = 3;
    [Range(0,10)]
    public float yMinMax = 1.5f;
    [Range(0,50)]
    public float speed = 10;

    public GameObject CenterPoint;

    private void Update()
    {
        Vector3 defaultPosition = CenterPoint.transform.localPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition + new Vector3(x, y, 0),speed*Time.deltaTime);
    }

    public void SetParameters(float xValue ,float yValue)
    {
        x = xValue * xMinMax;
        y = yValue * yMinMax;
    }
}
