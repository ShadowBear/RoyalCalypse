using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{

    public bool ownGunPoint = false;

    protected override void Start()
    {
        base.Start();
        if (ownGunPoint) gunPoint = GameObject.FindGameObjectWithTag("OwnGunPoint").GetComponent<Transform>();
    }

    protected override IEnumerator Shot()
    {
        canAttack = false;
        ammuAmount--;
        foreach(Transform t in gunPoint.GetComponentsInChildren<Transform>())
        {
            if (t == gunPoint.transform) continue;
            GameObject projectile = Instantiate(bullet, t.position, Quaternion.identity);
            if (projectile.GetComponent<PolygonArsenal.PolygonProjectileScript>())
            {
                projectile.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = GetWeaponDamage();
            }
            projectile.GetComponent<Rigidbody>().AddForce(t.forward * projectileSpeed);
        }

        UpdateAmmuGUI();
        yield return new WaitForSeconds(fireRate);
        canAttack = true;
        yield return null;
    }

}
