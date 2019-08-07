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

    //[SerializeField] protected Image healthbar;
    //[SerializeField] protected Image shieldbar;
    protected Slider healthbarSlider;
    protected Slider shieldbarSlider;

    public bool alive = true;

    protected virtual void Start()
    {
        health = maxHealth;
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.name == "SkillGuageHealth") healthbarSlider = t.GetComponentInChildren<Slider>();
            else if(t.name == "SkillGuageShield") shieldbarSlider = t.GetComponentInChildren<Slider>();
        }
        //healthbarSlider = transform.Find("SkillGuageHealth").GetComponentInChildren<Slider>();
        //shieldbarSlider = transform.Find("SkillGuageShield").GetComponentInChildren<Slider>();
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
        UpdateFillamount();

        if (health <= 0) Die();
    }

    protected void UpdateFillamount()
    {
        if(healthbarSlider != null)healthbarSlider.value = (float)health / maxHealth;
        //else healthbar.fillAmount = (float)health / maxHealth;
        if(shieldbarSlider != null) shieldbarSlider.value = (float)shield / maxShield;
        //else shieldbar.fillAmount = (float)shield / maxShield;
    }

    public virtual bool AddHealth(int health)
    {
        if (this.health == maxHealth) return false;
        this.health = (this.health + health) > maxHealth? maxHealth : (this.health + health);
        UpdateFillamount();
        return true;
    }

    public virtual bool AddShield(int shield)
    {
        if (this.shield == maxShield) return false;
        this.shield = (this.shield + shield) > maxShield ? maxShield : (this.shield + shield);
        UpdateFillamount();
        return true;
    }

    public virtual void ShowDamage(int damage, Transform trans) { }

    public void LostGame()
    {
        Die();
    }

    protected abstract void Die();
}
