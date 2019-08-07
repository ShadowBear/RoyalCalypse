using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHealth : Health
{
    // Start is called before the first frame update
    protected override void Start()
    {
        health = 1;
        shield = 0;
    }

    protected override void Die()
    {
        GetComponent<Bomb>().FlameIt();
    }

    public override void Damage(int dmg)
    {
        if (dmg > 0)
        {
            if (!alive) return;
            alive = false;
            Die();
        }
    }
}
