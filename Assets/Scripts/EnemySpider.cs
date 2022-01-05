using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpider : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public GameObject targetPlayer;
    public Rigidbody rb;




    public float Speed;

    [Header("Wander Values")]
    public float wanderRadius = 1;
    public float wanderDistance = 1;
    public float wanderJitter = 1;

    Vector3 wanderTarget;
    void Start()
    {
        wanderTarget = gameObject.transform.position;
    }


    private void Update()
    {
        Wander();
    }


    void Wander()
    {
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter,
                                        0,
                                        Random.Range(-1.0f, 1.0f) * wanderJitter); 

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        //Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        //Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);
        
        agent.SetDestination( targetPlayer.transform.position);
    }


}
