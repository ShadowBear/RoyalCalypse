using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int maxShield;

    [SerializeField] protected int health;
    [SerializeField] protected int shield;

    [SerializeField] protected Image healthbar;
    [SerializeField] protected Image shieldbar;

    public bool alive = true;

    protected virtual void Start()
    {
        health = maxHealth;
        UpdateFillamount();
        shield = 0;
    }

    public virtual void Damage(int dmg)
    {
        if (!alive) return;

        if (shield > 0)
        {
            if (dmg > shield)
            {
                dmg -= shield;
                shield = 0;
                health -= dmg;
            } else shield -= dmg;
        }else health -= dmg;
        ShowDamage(dmg, transform);
        Debug.Log("Schaden: " + dmg);
        UpdateFillamount();

        if (health <= 0) Die();
    }

    protected void UpdateFillamount()
    {
        healthbar.fillAmount = (float)health / maxHealth;
        shieldbar.fillAmount = (float)shield / maxShield;
    }

    public virtual void AddHealth(int health)
    {
        this.health = (this.health + health) > maxHealth? maxHealth : (this.health + health);
        UpdateFillamount();
    }

    public virtual void AddShield(int shield)
    {
        this.shield = (this.shield + shield) > maxShield ? maxShield : (this.shield + shield);
        UpdateFillamount();
    }

    public virtual void ShowDamage(int damage, Transform trans) { }

    public void LostGame()
    {
        Die();
    }

    protected abstract void Die();
}
