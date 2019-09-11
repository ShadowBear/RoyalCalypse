using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    private Animator anim;
    private EnemyAI enemyAI;
    public GameObject dropParent;
    [SerializeField] private int score;
    [SerializeField] private int exp = 5;
    protected override void Start()
    {
        maxHealth = Random.Range(maxHealth / 2, maxHealth);
        health = maxHealth;
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.name == "SkillGuageHealth") healthbarSlider = t.GetComponentInChildren<Slider>();
            else if (t.name == "SkillGuageShield") shieldbarSlider = t.GetComponentInChildren<Slider>();
        }
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        shield = maxShield;
        UpdateFillamount();        
    }

    public override void Damage(int dmg)
    {
        base.Damage(dmg);
        if (enemyAI == null) return;
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
        if (score >= 0) GameManager.gameManager.AddScore(score, true);
        if (anim) anim.SetTrigger("Die");
        //if (transform.parent != null) Destroy(transform.parent.gameObject, 3.9f);
        //else Destroy(gameObject, 3.9f);
        Destroy(gameObject, 3f);
        if(dropParent != null)
        {
            GameObject drop = Instantiate(dropParent, transform.position, Quaternion.identity);
            drop.GetComponent<DropItem>().DropItemObject(3f);
            drop.GetComponent<DropItem>().SetExp(exp);
        }
    }
}
