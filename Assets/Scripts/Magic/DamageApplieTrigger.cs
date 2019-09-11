using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageApplieTrigger : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int damageVarianz;
    public bool singleDmg = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().Damage(CalculateDamage());
            if(singleDmg) Destroy(this);
        }
    }

    int CalculateDamage()
    {
        return (damage + Random.Range(-damageVarianz, damageVarianz));
    }
}
