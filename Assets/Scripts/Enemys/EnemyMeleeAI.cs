using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAI : EnemyAI
{
    [SerializeField] protected float meleeAttackDistance = 3f;
    //Depends on Animation Length for Attack
    private float attackRate = 2f;

    protected override void Attack()
    {
        navMeshAgent.isStopped = true;
        ///*******************************/
        //navMeshAgent.enabled = false;
        //navMeshObstacle.enabled = true;
        ///*******************************/
        RotateToPlayer();
        if (canAttack && navMeshAgent.remainingDistance < meleeAttackDistance) StartCoroutine(MeleeAttack());
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        //Debug.Log("Remaining Distance before Attack: " + navMeshAgent.remainingDistance);
        if (navMeshAgent.remainingDistance > meleeAttackDistance && canAttack) currentState = EnemyState.Follow;
    }

    protected override void Fight()
    {
        RotateToPlayer();
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance > meleeAttackDistance) currentState = EnemyState.Follow;
        else if (navMeshAgent.remainingDistance < meleeAttackDistance) currentState = EnemyState.Attack;
    }

    protected override void Follow()
    {
        ///*******************************/
        //navMeshAgent.enabled = true;
        //navMeshObstacle.enabled = false;
        /*******************************/

        //Simple Rotation
        //transform.LookAt(player.transform.position);

        //Smoother Rotation
        RotateToPlayer();

        if (!fieldOfView.GetVisible())
        {
            currentState = EnemyState.Search;
            lastSeenPosition = player.transform.position;
            return;
        }
        navMeshAgent.isStopped = false;
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance < meleeAttackDistance) currentState = EnemyState.Attack;

    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
        yield return null;
    }


}
