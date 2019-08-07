using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLowWizard : EnemyAI
{
    [Range(7, 25)]
    [SerializeField] private float shootingDistance;
    [SerializeField] private float shootingHysterese = 3f;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private bool visible = false;
    [SerializeField] private bool followPlayer = false;
    [SerializeField] private float waitBeforeResetTime = 5f;
    [SerializeField] private bool waiting = false;
    [SerializeField] float castSpeed = 2.3f;

    [SerializeField] GameObject spell01;
    [SerializeField] GameObject spell02;
    [SerializeField] GameObject spell03;
    [SerializeField] GameObject portDust;

    [SerializeField] GameObject spellSpawnParent;

    [SerializeField] int spell01DMG = 20;
    [SerializeField] int spell02DMG = 40;

    [SerializeField] float portdistanceRadius = 6f;

    public bool porting = false;

    [SerializeField] int healingAmount = 25;


    [SerializeField] int spellSpeed = 750;

    private bool waveActive = false;
    [SerializeField] string[] attacks = { "Attack_01", "Attack_02", "Power_Up" };


    protected override void Attack()
    {
        navMeshAgent.isStopped = true;

        RotateToPlayer();
        if (navMeshAgent.remainingDistance < shootingDistance)
        {
            if(canAttack) StartCoroutine(MagicAttack());
        }
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance > shootingDistance && canAttack) currentState = EnemyState.Follow;
    }

    protected override void Fight()
    {
        RotateToPlayer();
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        if (navMeshAgent.remainingDistance > shootingDistance - shootingHysterese) currentState = EnemyState.Follow;
        else currentState = EnemyState.Attack;
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
        if (navMeshAgent.remainingDistance < shootingDistance - shootingHysterese) currentState = EnemyState.Attack;
    }

    protected override void Animation()
    {
        if (navMeshAgent.velocity != Vector3.zero) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
    }

    IEnumerator WaitBeforeReturnToNormal()
    {
        waiting = true;
        yield return new WaitForSeconds(waitBeforeResetTime);
        currentState = EnemyState.Patrol;
        waiting = false;
        yield return null;
    }

    IEnumerator MagicAttack()
    {
        canAttack = false;
        float temp = Random.value;
        //int attack = Random.Range(0, attacks.Length);
        int attack = 0;
        if (temp > 0.8f) attack = 2;
        else if (temp > 0.5f) attack = 1;
        else attack = 0;
        anim.SetTrigger(attacks[attack]);
        yield return new WaitForSeconds(castSpeed);
        canAttack = true;
        if (porting && Random.value < 0.4f) Porting();
        yield return null;
    }

    void MagicSpell_01()
    {
        GameObject spell = Instantiate(spell01, spellSpawnParent.transform.GetChild(1).position, Quaternion.identity);
        if (spell.GetComponent<PolygonArsenal.PolygonProjectileScript>())
        {
            spell.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = spell01DMG;
        }
        spell.GetComponent<Rigidbody>().AddForce(spellSpawnParent.transform.GetChild(1).forward * spellSpeed);
    }

    void MagicSpell_02()
    {
        foreach (Transform t in spellSpawnParent.GetComponentsInChildren<Transform>())
        {
            if (t == spellSpawnParent.transform) continue;
            GameObject spell = Instantiate(spell02, t.position, Quaternion.identity);
            if (spell.GetComponent<PolygonArsenal.PolygonProjectileScript>())
            {
                spell.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = spell02DMG;
            }
            spell.GetComponent<Rigidbody>().AddForce(t.forward * spellSpeed);
        }
    }

    void Healing()
    {
        GameObject spell = Instantiate(spell03, transform.position, Quaternion.Euler(-90, 0, 0));
        spell.transform.localScale = new Vector3(1.5f, 1.5f, 3);
        Destroy(spell, 2f);
        StartCoroutine(HealSpell());
    }

    IEnumerator HealSpell()
    {
        yield return new WaitForSeconds(1.75f);
        if (GetComponent<Health>() != null) GetComponent<Health>().AddHealth(healingAmount);
        yield return null;
    }
    void MagicSpell_04()
    {
        
    }

    void MagicSpell_05()
    {
        Healing();
    }

    void Porting()
    {
        NavMeshHit hit;
        Vector3 portPosition = Random.insideUnitSphere * portdistanceRadius + player.transform.position;
        portPosition.y = player.transform.position.y;
        if (NavMesh.SamplePosition(portPosition, out hit, 2.5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        GameObject dust = Instantiate(portDust, transform.position, Quaternion.identity);
        Destroy(dust, 2f);
    }

}
