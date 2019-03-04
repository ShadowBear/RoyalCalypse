﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectAmmu : CollectItemTemplate
{
    //public int ammuAmount;
    public GameManager.bulletType weaponType;
    protected override void CollectIt(Collider player)
    {
        bool reloaded = player.GetComponent<Player>().AddAmmu(weaponType);
        if (reloaded) Destroy(gameObject);
        else image.fillAmount = 0;
    }
}
