using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mutant : EnemyAI
{
    protected float normalSpeed = 1.5f;
    protected float agressivSpeed = 5f;
    protected float waitBeforeAttackTime = 5f;
    public int wanderRadius = 5;
    public int rangeObjectDmg = 25;
    public int spellSpeed = 750;

    protected bool followPlayer = true;
    protected bool waiting = true;

    protected float meleeAttackDistance = 4f;
    protected float rangeAttackDistance = 10f;
    protected float hystereseAttackDistance;

    public GameObject[] skins;
    public GameObject jumpGroundEffect;
    public GameObject rangeAttackObject;
    public Transform rootTransform;
    public Transform roaringTransformParent;

    private void Awake()
    {
        transform.localScale *= Random.Range(0.6f, 1f);
        if (skins.Length <= 0) return;
        int skin = Random.Range(0, skins.Length);
        for(int i = 0; i < skins.Length; i++)
        {
            if(i == skin) skins[i].SetActive(true);
            else skins[i].SetActive(false);
        }        
    }

    protected override void Start()
    {
        player = Player.player.gameObject;
        navMeshAgent = GetComponent<NavMeshAgent>();
        minGuardingTime = 0f;
        maxGuardingTime = 3f;
        hystereseAttackDistance = (rangeAttackDistance - meleeAttackDistance) / 2;
        alliesMask = LayerMask.NameToLayer("Enemy");

        anim = GetComponent<Animator>();
        fieldOfView = GetComponent<FieldOfView>();  
        
        NonPatrolGuard = true;
        navMeshAgent.SetDestination(transform.position);
        
        canAttack = true;
    }


    protected override void Guard()
    {
        navMeshAgent.speed = normalSpeed;
        waitBeforeAttackTime = 4f;
        if (navMeshAgent.remainingDistance < 2.5f)
        {
            navMeshAgent.isStopped = true;
            if (guardingTime <= 0)
            {
                guardingTime = Random.Range(minGuardingTime, maxGuardingTime);
                Vector3 newPos = Vector3.zero;
                newPos = RandomNavSphere(transform.position, wanderRadius, -1);
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

    protected override void Fight()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = agressivSpeed;
        RotateToPlayer();
        if (agressivStatus == AggressivState.Normal)
        {
            //Debug.Log("Bin im NormalMode");
            if (waitBeforeAttackTime == 4f) anim.SetTrigger("Agressiv");
            if (waitBeforeAttackTime > 0){
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
        if (navMeshAgent.remainingDistance < meleeAttackDistance && canAttack)StartCoroutine(MeleeAttack());
        else if (navMeshAgent.remainingDistance < rangeAttackDistance && canAttack)
        {
            if (Random.value <= 0.5f) StartCoroutine(RangeAttack("Roaring"));
            else StartCoroutine(RangeAttack("JumpAttack"));
        }
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance > rangeAttackDistance && canAttack) currentState = EnemyState.Follow;
        
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
        if (navMeshAgent.remainingDistance < (rangeAttackDistance - hystereseAttackDistance)) currentState = EnemyState.Attack;
    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;
        if (Random.Range(0, 2) == 0) anim.SetTrigger("Punch");
        else anim.SetTrigger("Swiping");
        yield return new WaitForSeconds(2.5f);
        canAttack = true;
    }

    public void JumpAttackInstantiator()
    {
        Vector3 pos = new Vector3(rootTransform.position.x, transform.position.y, rootTransform.position.z);
        GameObject groundCrush = Instantiate(jumpGroundEffect,pos , Quaternion.identity);
        groundCrush.transform.rotation = Quaternion.Euler(-90, 0, 0);
        groundCrush.transform.localScale *= 2;
        Destroy(groundCrush, 3f);
    }

    public void RoaringAttack()
    {
        StartCoroutine(Roar());
    }

    IEnumerator Roar()
    {
        foreach (Transform t in roaringTransformParent.GetComponentInChildren<Transform>())
        {
            if (t.transform == roaringTransformParent) continue;
            GameObject spell = Instantiate(rangeAttackObject, t.position, Quaternion.identity);
            if (spell.GetComponent<PolygonArsenal.PolygonProjectileScript>())
            {
                spell.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = rangeObjectDmg;
            }
            spell.GetComponent<Rigidbody>().AddForce(t.forward * spellSpeed);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator RangeAttack(string attack)
    {
        canAttack = false;
        if(attack == "Roaring")
        {
            anim.SetTrigger("Roaring");
            yield return new WaitForSeconds(5f);
        }
        else if (attack == "JumpAttack")
        {
            anim.SetTrigger("JumpAttack");
            yield return new WaitForSeconds(4f);
        }
        canAttack = true;
    }

    public IEnumerator WaitBeforeReturnToNormal()
    {
        waiting = true;
        yield return new WaitForSeconds(waitBeforeAttackTime/2);
        currentState = EnemyState.Patrol;
        waiting = false;
        yield return null;
    }


}
