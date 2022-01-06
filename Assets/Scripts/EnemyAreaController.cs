using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaController : MonoBehaviour
{
    [System.Serializable]
    public struct MoveableArea
    {
        public Transform AreaPos;
    }

    public MoveableArea[] area;
    
}
