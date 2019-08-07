using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWeaponSpawn : MonoBehaviour
{
    [SerializeField] GameObject weaponsParent;
    private int weapon = -1;
    //[SerializeField] GameObject spawnWeapon;

    // Start is called before the first frame update
    public void Spawn()
    {
        List<GameObject> weaponObjects = new List<GameObject>();
        Transform[] weapons = weaponsParent.GetComponentsInChildren<Transform>();
        
        foreach (Transform t in weapons)
        {
            if (t.CompareTag("Weapon"))
            {
                weaponObjects.Add(t.gameObject);
            }
        }
        //if (weapon == -1) weapon = Random.Range(0, weaponObjects.Count - 1);
        if (weapon == -1) WeaponProducer();
        GameObject spawnWeapon = Instantiate(weaponObjects[weapon], transform.position, Quaternion.identity);
        spawnWeapon.transform.parent = transform;
        spawnWeapon.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    private void WeaponProducer()
    {
        float temp = Random.Range(0f, 1f);
        if (temp >= 0f && temp < 0.15f) weapon = 0;
        else if (temp >= 0.15f && temp < 0.25f) weapon = 1;
        else if (temp >= 0.25f && temp < 0.40f) weapon = 2;
        else if (temp >= 0.40f && temp < 0.55f) weapon = 3;
        else if (temp >= 0.55f && temp < 0.65f) weapon = 4;
        else if (temp >= 0.65f && temp < 0.75f) weapon = 5;
        else if (temp >= 0.75f && temp < 0.80f) weapon = 6;
        else if (temp >= 0.80f && temp < 0.85f) weapon = 7;
        else if (temp >= 0.85f && temp <= 0.1f) weapon = 8;

        //Random.Range(0, weaponObjects.Count - 1);
    }       

    
}
