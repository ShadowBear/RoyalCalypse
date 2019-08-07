using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammethrower : Weapon
{
    GameObject flame;
    float ammuAmountFloat = 0f;
    [SerializeField] GameObject hitBox;

    protected override void Start()
    {
        base.Start();
        flame = transform.GetChild(0).gameObject;
        flame.SetActive(false);
    }

    public override void Attack()
    {

        if (attacking && ammuAmount > 0)
        {
            flame.SetActive(true);
            hitBox.SetActive(true);
            ammuAmountFloat -= Time.deltaTime;
            ammuAmount = (int)ammuAmountFloat;
        }
        else {
            flame.SetActive(false);
            hitBox.SetActive(false);
            ammuAmountFloat = ammuAmount; 
        }
        UpdateAmmuGUI();
        
    }
}
