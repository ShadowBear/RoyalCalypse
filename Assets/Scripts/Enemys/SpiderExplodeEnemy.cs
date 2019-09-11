using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderExplodeEnemy : Mutant
{
    public GameObject explodeFX;
    [SerializeField] float explosionRadius;
    public int explosionDMG;
    [SerializeField] LayerMask damageLayer;
    [SerializeField] float explodeTime = 1.2f;

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
            if (navMeshAgent.velocity != Vector3.zero) anim.SetBool("Move Forward Slow", true);
            else anim.SetBool("Move Forward Slow", false);
        }
    }


    private IEnumerator MeleeAttack()
    {
        canAttack = false;        
        while(explodeTime > 0)
        {
            transform.localScale *= 1.005f;
            explodeTime -= Time.deltaTime;
        }
        GameObject explode = Instantiate(explodeFX, transform.position, Quaternion.identity);
        alive = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            Health health = colliders[i].GetComponent<Health>();
            if (!health) continue;

            int damageToApplie = CalculateDamage(colliders[i].transform.position);
            health.Damage(damageToApplie);
        }
        Destroy(explode, 2f);
        Destroy(gameObject);        
        yield return null;
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
        if (!alive) return;
        RotateToPlayer();
        if (navMeshAgent.remainingDistance < meleeAttackDistance && canAttack) StartCoroutine(MeleeAttack());
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.isActiveAndEnabled) {
            if (navMeshAgent.remainingDistance > meleeAttackDistance && canAttack) currentState = EnemyState.Follow;
        }
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

    private int CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damageAmount = relativeDistance * explosionDMG;

        // Make sure that the minimum damage is always 0.
        damageAmount = Mathf.Max(0f, damageAmount);

        return (int)damageAmount;
    }
}
