using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDistanceToPlayer : MonoBehaviour
{
#region - Public Variable -

    [Header("-------  Player  -------")]
    public GameObject player;

    [Header("-------  Distance  -------")]
    [Range(0f, 1000f)]
    public float maxDistance;
    [Range(0f, 500f)]
    public float innerCircle;
    [Range(0f, 500f)]
    public float outerCircle;

    [Header("-------  Max Distance  -------")]

    public GameObject enemy;

    [Header("-------  Inner Circle  -------")]



    [Header("-------  Outer Circle  -------")]
    public float abc;

#endregion

    void Update()
    {
        float CenterToPlayerDistance;

        CenterToPlayerDistance = (player.transform.position - transform.position).magnitude;
        
        if( CenterToPlayerDistance <= maxDistance )
        {
            WithinMaxDistance();

            if( CenterToPlayerDistance <= outerCircle )
            {
                WithinOuterCircle();

                if( CenterToPlayerDistance <= innerCircle)
                {
                    WithinInnerCircle();
                }
                else
                {
                    OutSideInnerCircle();
                }
            }
            else
            {
                OutSideOuterCircle();
            }
        }
        else
        {
            OutSideMaxDistance();
        }
    }


#region - Max Distance -

    private void WithinMaxDistance()
    {

        enemy.SetActive(true);

    }
    private void OutSideMaxDistance()
    {
        enemy.SetActive(false);
    }

#endregion


#region - Outer Circle -

    private void WithinOuterCircle()
    {

    }
    private void OutSideOuterCircle()
    {

    }

#endregion


#region - Inner Circle -
    private void WithinInnerCircle()
    {

    }

    private void OutSideInnerCircle()
    {

    }

    #endregion


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerCircle);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerCircle);
    }

}
