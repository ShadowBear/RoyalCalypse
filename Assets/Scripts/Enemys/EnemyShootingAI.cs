using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingAI : EnemyAI
{
    [Range(7,25)]
    [SerializeField] private float shootingDistance;
    [SerializeField] private float shootingHysterese = 3f;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private bool visible = false;

    [SerializeField] private bool followPlayer = false;
    [SerializeField] private float waitBeforeResetTime = 5f;
    [SerializeField] private bool waiting = false;

    private Vector3 playerPos;
    private Weapon equipedWeapon;

    protected override void Start()
    {
        base.Start();
        equipedWeapon = GetComponentInChildren<Weapon>();
        playerPos = player.transform.GetChild(0).transform.position;
    }    

    protected override void Animation()
    {
        if (navMeshAgent.velocity != Vector3.zero) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
        if(currentState == EnemyState.Attack) anim.SetBool("Attack", true);
        else anim.SetBool("Attack", false);
    }

    protected override void Fight()
    {
        RotateToPlayer();
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance > shootingDistance - shootingHysterese) currentState = EnemyState.Follow;
        else currentState = EnemyState.Attack;
    }

    protected override void Attack()
    {
        navMeshAgent.isStopped = true;

        ///*******************************/
        //navMeshAgent.enabled = false;
        //navMeshObstacle.enabled = true;
        ///*******************************/

        RotateToPlayer();
        if (navMeshAgent.remainingDistance < shootingDistance)
        {
            if (equipedWeapon.GetAttackStatus()) StartCoroutine(equipedWeapon.EnemyShot(gunPoint));
        }            
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        //Debug.Log("Remaining Distance before Attack: " + navMeshAgent.remainingDistance);
        if (navMeshAgent.remainingDistance > shootingDistance && canAttack) currentState = EnemyState.Follow;
    }

    protected override void Follow()
    {
        ///*******************************/
        //navMeshAgent.enabled = true;
        //navMeshObstacle.enabled = false;
        ///*******************************/

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
                if(!waiting) StartCoroutine(WaitBeforeReturnToNormal());
                navMeshAgent.isStopped = true;
                return;
            }
            
        }
        navMeshAgent.isStopped = false;
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance < shootingDistance - shootingHysterese) currentState = EnemyState.Attack;
    }

    IEnumerator WaitBeforeReturnToNormal()
    {
        waiting = true;
        yield return new WaitForSeconds(waitBeforeResetTime);
        currentState = EnemyState.Patrol;
        waiting = false;
        yield return null;
    }

}
