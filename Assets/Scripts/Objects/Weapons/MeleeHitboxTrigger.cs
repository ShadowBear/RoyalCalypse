using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitboxTrigger : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int damageVarianz;
    private Collider hitbox;
    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
    }

    public void SetHitbox(int activater)
    {
        hitbox.enabled = activater == 0 ? false : true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().Damage(CalculateDamage());
        }
    }

    int CalculateDamage()
    {
        return (damage + Random.Range(-damageVarianz, damageVarianz));
    }
}
