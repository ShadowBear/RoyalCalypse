using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectShield : CollectItemTemplate
{
    public int shieldAmount;

    protected override void CollectIt(Collider player)
    {
        player.GetComponent<Health>().AddShield(shieldAmount);
        Destroy(gameObject);
    }
}
