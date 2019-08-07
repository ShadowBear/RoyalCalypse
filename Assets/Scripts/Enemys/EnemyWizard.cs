using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizard : EnemyAI
{
    [Range(7, 25)]
    [SerializeField] protected float shootingDistance;
    [SerializeField] protected float shootingHysterese = 3f;
    [SerializeField] protected Transform gunPoint;
    [SerializeField] protected bool visible = false;
    [SerializeField] protected bool followPlayer = false;
    [SerializeField] protected float waitBeforeResetTime = 5f;
    [SerializeField] protected bool waiting = false;
    [SerializeField] protected float castSpeed = 2.3f;

    [SerializeField] protected GameObject spell01;
    [SerializeField] protected GameObject spell02;
    [SerializeField] protected GameObject spell03;
    [SerializeField] protected GameObject spell04;
    [SerializeField] protected GameObject spell05;

    [SerializeField] protected GameObject spellSpawnParent;
    [SerializeField] protected Transform summonPoint;

    [SerializeField] protected int spell01DMG = 50;
    [SerializeField] protected int spell02DMG = 40;
    [SerializeField] protected int spell03DMG = 30;
    [SerializeField] protected int spell04DMG = 20;

    [SerializeField] protected int healingAmount = 10;


    [SerializeField] protected GameObject summonMonster;
    [SerializeField] protected int summonMonstersCount = 0;
    [SerializeField] protected int maxSummons = 2;


    [SerializeField] protected int spellSpeed = 500;

    protected bool waveActive = false;
    [SerializeField] protected string[] attacks = { "Cast_01", "Attack_01", "Attack_02", "Attack_2H_Area", "Power_Up" };


    protected override void Attack()
    {
        navMeshAgent.isStopped = true;

        RotateToPlayer();
        if (navMeshAgent.remainingDistance < shootingDistance)
        {
            if(canAttack) StartCoroutine(MagicAttack());
        }
        if (followUpdater) StartCoroutine(UpdatePlayerPos());
        //Debug.Log("Remaining Distance before Attack: " + navMeshAgent.remainingDistance);
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

    protected IEnumerator WaitBeforeReturnToNormal()
    {
        waiting = true;
        yield return new WaitForSeconds(waitBeforeResetTime);
        currentState = EnemyState.Patrol;
        waiting = false;
        yield return null;
    }

    protected virtual IEnumerator MagicAttack()
    {
        canAttack = false;
        int attack = Random.Range(0, attacks.Length);
        if (attacks[attack] == "Cast_01" && summonMonstersCount >= maxSummons)
        {
            //Do Nothing
            Debug.Log("MaxMinions");
        }else if (attacks[attack] == "Cast_01")
        {
            summonMonstersCount++;
            anim.SetTrigger(attacks[attack]);
        }else anim.SetTrigger(attacks[attack]);
        yield return new WaitForSeconds(castSpeed);
        canAttack = true;
        yield return null;
    }

    protected virtual void MagicSpell_01()
    {
        GameObject spell = Instantiate(spell01, spellSpawnParent.transform.GetChild(1).position, Quaternion.identity);
        if (spell.GetComponent<PolygonArsenal.PolygonProjectileScript>())
        {
            spell.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = spell01DMG;
        }
        spell.GetComponent<Rigidbody>().AddForce(spellSpawnParent.transform.GetChild(1).forward * spellSpeed);
    }

    protected virtual void MagicSpell_02()
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

    protected virtual void MagicSpell_03()
    {
        GameObject spell = Instantiate(spell03, transform.position, Quaternion.Euler(-90,0,0));
        Destroy(spell, 4f);
    }


    //Summons a Minion
    protected virtual void MagicSpell_04()
    {
        GameObject monster = Instantiate(summonMonster, summonPoint.position, Quaternion.identity);
        //GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
        monster.transform.position = summonPoint.position + new Vector3(0,0.5f,0);
        monster.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        monster.transform.forward = transform.forward;
        //Destroy(monster, 2f);


        GameObject spell = Instantiate(spell04, summonPoint.position, Quaternion.identity);
        //Destroy(spell, 4f);
    }

    protected virtual void MagicSpell_05()
    {
        GameObject spell = Instantiate(spell05, transform.position, Quaternion.Euler(-90, 0, 0));
        spell.transform.localScale = new Vector3(1.5f, 1.5f, 3);
        Destroy(spell, 2f);
        StartCoroutine(HealSpell());
    }

    protected virtual IEnumerator HealSpell()
    {
        yield return new WaitForSeconds(1.75f);
        if (GetComponent<Health>() != null) GetComponent<Health>().AddHealth(healingAmount);
        yield return null;
    }

}
