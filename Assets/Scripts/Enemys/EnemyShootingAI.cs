using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingAI : EnemyAI
{
    [SerializeField] private float shootingDistance;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private bool visible = false;
    private Vector3 playerPos;
    private Weapon equipedWeapon;

    protected override void Start()
    {
        base.Start();
        equipedWeapon = GetComponentInChildren<Weapon>();
        playerPos = player.transform.GetChild(0).transform.position;
    }

    protected override void Update()
    {
        if (!alive) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < followDistance)
        {
            transform.LookAt(player.transform.position);
            RaycastHit hit;

            if (Physics.SphereCast(gunPoint.forward, 1, playerPos, out hit, shootingDistance))
            {
                if (hit.collider.CompareTag("Player"))visible = true;          
            }
            else visible = false;
            Debug.DrawRay(gunPoint.forward, playerPos, Color.red);
            if (distanceToPlayer > shootingDistance || !visible)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.destination = player.transform.position;
            }
            else
            {
                navMeshAgent.isStopped = true;
                if (equipedWeapon.GetAttackStatus()) StartCoroutine(equipedWeapon.EnemyShot(gunPoint));
            }
            Animation();
        }
        else
        {
            anim.SetFloat("Speed", 0);
            navMeshAgent.isStopped = true;
        }
    }
    

    protected override void Animation()
    {
        if (navMeshAgent.velocity != Vector3.zero) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
        anim.SetBool("Attack", equipedWeapon.GetAttackStatus());
    }
}
