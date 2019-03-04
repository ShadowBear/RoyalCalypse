using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectHealth : CollectItemTemplate
{
    public int healthAmount;
    protected override void CollectIt(Collider player)
    {
        player.GetComponent<Health>().AddHealth(healthAmount);
        Destroy(gameObject);
    }
}
