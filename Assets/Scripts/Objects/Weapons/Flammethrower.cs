using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammethrower : Weapon
{
    GameObject flame;

    protected override void Start()
    {
        base.Start();
        flame = transform.GetChild(0).gameObject;
        flame.SetActive(false);
    }

    public override void Attack()
    {
        if (attacking) flame.SetActive(true);
        else flame.SetActive(false);
    }
}
