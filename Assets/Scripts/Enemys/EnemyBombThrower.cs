using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBombThrower : EnemyWizard
{
    [SerializeField] string[] bombAttacks = { "Attack_01", "Attack_02"};


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


    protected override IEnumerator MagicAttack()
    {
        canAttack = false;
        int attack = Random.Range(0, bombAttacks.Length);
        anim.SetTrigger(bombAttacks[attack]);
        yield return new WaitForSeconds(castSpeed);
        canAttack = true;
        yield return null;
    }

    protected override void MagicSpell_01()
    {
        GameObject spell = Instantiate(spell01, spellSpawnParent.transform.GetChild(1).position, Quaternion.identity);
        if (spell.GetComponent<PolygonArsenal.PolygonProjectileScript>())
        {
            spell.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = spell01DMG;
        }
        spell.GetComponent<Rigidbody>().AddForce(spellSpawnParent.transform.GetChild(1).forward * spellSpeed);
    }

    protected override void MagicSpell_02()
    {
        StartCoroutine(ThrowBombs());
    }

    IEnumerator ThrowBombs()
    {
        foreach (Transform t in spellSpawnParent.GetComponentsInChildren<Transform>())
        {
            if (t == spellSpawnParent.transform) continue;
            GameObject spell = Instantiate(spell02, t.position, Quaternion.identity);
            if (spell.GetComponent<PolygonArsenal.PolygonProjectileScript>())
            {
                spell.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = spell02DMG;
            }
            float distanceToPlayer = Mathf.Abs((transform.position - player.transform.position).magnitude);
            spell.GetComponent<Rigidbody>().AddForce(t.forward * (spellSpeed / 13) * distanceToPlayer);
            yield return new WaitForSeconds(0.1f);
        }
    }

    //void MagicSpell_03()
    //{
    //    GameObject spell = Instantiate(spell03, transform.position, Quaternion.Euler(-90, 0, 0));
    //    spell.transform.localScale = new Vector3(1.5f, 1.5f, 3);
    //    Destroy(spell, 2f);
    //    StartCoroutine(HealSpell());
    //}

    //IEnumerator HealSpell()
    //{
    //    yield return new WaitForSeconds(1.75f);
    //    if (GetComponent<Health>() != null) GetComponent<Health>().AddHealth(healingAmount);
    //    yield return null;
    //}
}
