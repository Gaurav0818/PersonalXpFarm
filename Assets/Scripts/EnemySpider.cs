using UnityEngine;
using UnityEngine.AI;

public class EnemySpider : MonoBehaviour
{

    public enum TypeOfEnemy { RangeAttack , MeleeAttack , miniBoss , Boss }
    public enum StateOfEnemy {  Wonder , Attack , Idle , Seek , Defend }

    public TypeOfEnemy typeOfEnemy = TypeOfEnemy.MeleeAttack;

    public StateOfEnemy stateOfEnemy = StateOfEnemy.Wonder;

    public EnemyAreaController enemyAreaController;

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject targetPlayer;
    public Rigidbody rb;
    public float Speed;

    private float WanderTimer = 0f;

    public float DetectRange = 50;
    public float AttackRange_Range = 15;
    public float AttackRange_Melee = 20;



    [Header("Wander Values")]
    public float wanderSpeed = 1;
    public float wanderDistance = 1;
    public float wanderJitter = 1;

    Vector3 wanderTarget;
    void Start()
    {
        wanderTarget = gameObject.transform.position;
    }


    private void Update()
    {
        if (CalculateDistanceWithTarget() < DetectRange)
        {
            Attack();
        }
        else
        {
            WanderTime();
        }
    }

    float CalculateDistanceWithTarget()
    {
        Vector3 dis = transform.position - targetPlayer.transform.position;
        float distance = dis.magnitude;

        return distance;
    }


    void Attack()
    {
        if (typeOfEnemy == TypeOfEnemy.MeleeAttack)
        {
            if (CalculateDistanceWithTarget() < AttackRange_Melee)
                MeleeAttack();
            else
                TargetLocation(targetPlayer.transform.position);
        }
        else if (typeOfEnemy == TypeOfEnemy.RangeAttack)
        {
            if (CalculateDistanceWithTarget() < AttackRange_Range)
                RangeAttack();
            else
                TargetLocation(targetPlayer.transform.position);
        }

    }

    void MeleeAttack()
    {
        agent.isStopped = true;
    }

    void RangeAttack()
    {
        agent.Stop(true);
    }

    void TargetLocation( Vector3 targetPos)
    {
        agent.isStopped = false;
        agent.SetDestination(targetPos);
    }

#region - Wander - 

    void WanderTime()
    {
        if (WanderTimer > .25)
        {
            if (Random.Range(0, 100) < 5)
                Wander();
            WanderTimer = 0;
        }
        else
            WanderTimer += Time.deltaTime;
    }

    void Wander()
    {
        int randomArea = Random.Range(0, enemyAreaController.area.Length);

        TargetLocation(enemyAreaController.area[randomArea].AreaPos.position);

    }

#endregion

}
