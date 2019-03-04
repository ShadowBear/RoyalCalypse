using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    [SerializeField] private int damage;
    private Collider hitbox;
    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Collider>();
    }

    public void SetHitbox(int activater)
    {
        hitbox.enabled = activater == 0 ? false : true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().Damage(damage);
        }
    }
}
