using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlwaysAttackAI : EnemyAI
{
    public bool melee;
    protected float meleeAttackDistance = 3f;
    private float attackRate = 2f;
    private float outOfRange = 15f;

    [Range(7, 25)]
    [SerializeField] private float shootingDistance;
    [SerializeField] private float shootingHysterese = 3f;
    [SerializeField] private Transform gunPoint;
    private Vector3 playerPos;
    [SerializeField] private Weapon equipedWeapon;

    private bool inSight = false;

    [SerializeField] float remainDis = 0;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        equipedWeapon = GetComponentInChildren<Weapon>();
        playerPos = player.transform.GetChild(0).transform.position;
        anim.SetBool("Melee", melee);
    }

    protected override void Attack()
    {
        navMeshAgent.isStopped = true;
        RotateToPlayer();
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (melee)
        {            
            if (canAttack && navMeshAgent.remainingDistance < meleeAttackDistance) StartCoroutine(MeleeAttack());
            if (navMeshAgent.remainingDistance > meleeAttackDistance && canAttack) currentState = EnemyState.Follow;
        }
        else
        {
            if (equipedWeapon.GetAttackStatus()) StartCoroutine(equipedWeapon.EnemyShot(gunPoint));
            if (navMeshAgent.remainingDistance > shootingDistance && canAttack) currentState = EnemyState.Follow;
        }
    }

    protected override void Fight()
    {
        RotateToPlayer();
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (melee)
        {
            if (navMeshAgent.remainingDistance > meleeAttackDistance) currentState = EnemyState.Follow;
            else currentState = EnemyState.Attack;
        }
        else
        {
            if (navMeshAgent.remainingDistance > shootingDistance - shootingHysterese) currentState = EnemyState.Follow;
            else currentState = EnemyState.Attack;
        }       
    }

    protected override void Follow()
    {
        RotateToPlayer();
        navMeshAgent.isStopped = false;
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        inSight = fieldOfView.GetVisible();
        if (inSight)
        {
            remainDis = navMeshAgent.remainingDistance;
            if (melee)
            {
                if (remainDis < meleeAttackDistance) currentState = EnemyState.Attack;
            }
            else
            {
                if (remainDis < shootingDistance - shootingHysterese) currentState = EnemyState.Attack;
            }            
        }
        else
        {
            remainDis = navMeshAgent.remainingDistance;
            if (remainDis >= outOfRange)
            {
                currentState = EnemyState.Patrol;
                Debug.Log("out Of Range");
            }
        }
    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;
        anim.SetTrigger("AttackT");
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
        yield return null;
    }

    protected override void Animation()
    {
        if (navMeshAgent.velocity != Vector3.zero) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
        if (!melee)
        {
            if (currentState == EnemyState.Attack) anim.SetBool("Attack", true);
            else anim.SetBool("Attack", false);
        }
    }
}
