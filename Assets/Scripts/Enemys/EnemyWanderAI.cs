using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderAI : EnemyAlwaysAttackAI
{
    public float wanderRadius;


    protected override void Guard()
    {
        if (navMeshAgent.remainingDistance < 1f)
        {
            navMeshAgent.isStopped = true;
            if (guardingTime <= 0)
            {
                guardingTime = Random.Range(minGuardingTime, maxGuardingTime);
                Vector3 newPos = Vector3.zero;
                if (transform.parent == null)
                {
                    newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                }
                else
                {
                    newPos = RandomNavSphere(transform.parent.transform.position, wanderRadius, -1);
                }
                navMeshAgent.SetDestination(newPos);
            }
            else guardingTime -= Time.deltaTime;
            
        }
        else navMeshAgent.isStopped = false;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
