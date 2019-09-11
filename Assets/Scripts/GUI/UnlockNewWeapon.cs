using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockNewWeapon : MonoBehaviour
{
    public GameObject weaponItemToUnlock;
    
    public void TryUnlockItem()
    {
        weaponItemToUnlock.GetComponent<WeaponSelectInventoryButton>().Unlock(true);
    }

}
