using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectWeapon : CollectItemTemplate
{

    public string weaponName;
    public int ammunation;

    protected override void CollectIt(Collider player)
    {
        player.GetComponent<Player>().SwapWeapon(weaponName);
        if (transform.parent != null) Destroy(transform.parent.gameObject);
        else Destroy(gameObject);
    }
}
