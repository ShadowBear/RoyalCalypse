using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderEnemy : Mutant {

    [SerializeField] BoxCollider hitbox;

    protected override void Start()
    {
        meleeAttackDistance = 2f;
        rangeAttackDistance = 6f;
        base.Start();
    }

    protected override void Animation()
    {
        if (anim != null)
        {
            //speed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;
            //lastPosition = transform.position;
            //if (speed > 0.1f) anim.SetBool("Move Forward Slow", true);
            //else anim.SetBool("Move Forward Slow", false);

            if (navMeshAgent.velocity != Vector3.zero) anim.SetBool("Move Forward Slow", true);
            else anim.SetBool("Move Forward Slow", false);
        }
    }

    
    private IEnumerator MeleeAttack()
    {
        canAttack = false;
        if (Random.Range(0, 2) == 0) anim.SetTrigger("Claw Attack");
        else anim.SetTrigger("Bite Attack");
        hitbox.enabled = true;
        yield return new WaitForSeconds(2.5f);
        hitbox.enabled = false;
        canAttack = true;
    }

    protected override void Fight()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = agressivSpeed;
        RotateToPlayer();
        if (agressivStatus == AggressivState.Normal)
        {
            if (waitBeforeAttackTime == 4f) anim.SetTrigger("Cast Spell");
            if (waitBeforeAttackTime > 0)
            {
                waitBeforeAttackTime -= Time.deltaTime;
                return;
            }
            else
            {
                if (followUpdater) StartCoroutine(UpdatePlayerPos());
                if (navMeshAgent.remainingDistance > fieldOfView.viewRadius) currentState = EnemyState.Guard;
                else currentState = EnemyState.Follow;
            }
        }
        else
        {
            if (followUpdater) StartCoroutine(UpdatePlayerPos());
            if (navMeshAgent.remainingDistance > fieldOfView.viewRadius) currentState = EnemyState.Guard;
            else currentState = EnemyState.Follow;
        }
    }

    protected override void Attack()
    {
        navMeshAgent.isStopped = true;
        RotateToPlayer();
        if (navMeshAgent.remainingDistance < meleeAttackDistance && canAttack) StartCoroutine(MeleeAttack());
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance > meleeAttackDistance && canAttack) currentState = EnemyState.Follow;

    }

    protected override void Follow()
    {
        RotateToPlayer();

        if (!fieldOfView.GetVisible())
        {
            if (followPlayer)
            {
                lastSeenPosition = player.transform.position;
                currentState = EnemyState.Search;
                return;
            }
            else
            {
                if (!waiting) StartCoroutine(WaitBeforeReturnToNormal());
                navMeshAgent.isStopped = true;
                return;
            }

        }
        navMeshAgent.isStopped = false;
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance < meleeAttackDistance) currentState = EnemyState.Attack;
    }
}
