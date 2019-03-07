using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Animator anim;
    private EnemyAI enemyAI;
    [SerializeField] private int score;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        shield = maxShield;
        UpdateFillamount();        
    }

    public override void Damage(int dmg)
    {
        base.Damage(dmg);
        if (enemyAI.GetCurrentState() != EnemyAI.EnemyState.Attack || 
            enemyAI.GetCurrentState() != EnemyAI.EnemyState.Follow ||
            enemyAI.GetCurrentState() != EnemyAI.EnemyState.Fight)
        {
            enemyAI.SetCurrentState(EnemyAI.EnemyState.Fight);
        }
    }

    public override void ShowDamage(int damage, Transform positionTransform)
    {
       GameManager.gameManager.ShowDamageText(damage, positionTransform);
    }

    protected override void Die()
    {
        alive = false;
        enemyAI.alive = alive;
        if (score >= 0) GameManager.gameManager.AddScore(score);
        if (anim) anim.SetTrigger("Die");
        if (transform.parent != null) Destroy(transform.parent.gameObject, 3.9f);
        else Destroy(gameObject, 3.9f);
        
    }
}
