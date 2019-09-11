using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeingHealth : Health
{
    FleeingAI fleeingAI;
    public int score = 1;
    public int exp = 5;
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    public GameObject dropParent;

    protected override void Start()
    {
        base.Start();
        fleeingAI = GetComponent<FleeingAI>();
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void Damage(int dmg)
    {
        base.Damage(dmg);
        if(alive) fleeingAI.SetState(FleeingAI.State.Flee);
    }

    protected override void Die()
    {
        alive = false;
        fleeingAI.SetAliveState(false);
        navMeshAgent.isStopped = true;
        if (score >= 0) GameManager.gameManager.AddScore(score);
        if (anim) anim.SetTrigger("Die");
        //if (transform.parent != null) Destroy(transform.parent.gameObject, 3.9f);
        Destroy(gameObject, 3f);
        if (dropParent != null)
        {
            GameObject drop = Instantiate(dropParent, transform.position, Quaternion.identity);
            drop.GetComponent<DropItem>().DropItemObject(3f);
            drop.GetComponent<DropItem>().SetExp(exp);
        }
    }
}
