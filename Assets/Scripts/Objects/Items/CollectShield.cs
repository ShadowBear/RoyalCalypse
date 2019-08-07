using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectShield : CollectItemTemplate
{
    public int shieldAmount;

    protected override void CollectIt(Collider player)
    {
        if (player.GetComponent<Health>().AddShield(shieldAmount))
        {
            if (transform.parent != null) Destroy(transform.parent.gameObject);
            else Destroy(gameObject);
        }
        else image.fillAmount = 0;        
    }
}
