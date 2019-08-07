using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammethrowerDamageApplie : MonoBehaviour
{
    int damage;
    float dmgCounter = 0;

    private void Start()
    {
        damage = GetComponentInParent<Weapon>().damage;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            dmgCounter -= Time.deltaTime;
            if (dmgCounter <= 0)
            {
                dmgCounter = 0.33f;
                other.GetComponent<Health>().Damage(damage/3);
            }
        }
    }
}
