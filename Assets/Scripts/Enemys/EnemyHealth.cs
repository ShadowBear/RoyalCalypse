using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Animator anim;
    private EnemyAI enemyAI;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        shield = maxShield;
        UpdateFillamount();        
    }

    public override void ShowDamage(int damage, Transform positionTransform)
    {
       GameManager.gameManager.ShowDamageText(damage, positionTransform);
    }

    protected override void Die()
    {
        alive = false;
        enemyAI.alive = alive;        
        if (anim) anim.SetTrigger("Die");        
        Destroy(gameObject, 3.9f);
        
    }
}
