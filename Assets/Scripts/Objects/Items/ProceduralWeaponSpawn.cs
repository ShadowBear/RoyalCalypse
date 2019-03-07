using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWeaponSpawn : MonoBehaviour
{
    [SerializeField] GameObject weaponsParent;
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
        int weapon = Random.Range(0, weaponObjects.Count - 1);
        GameObject spawnWeapon = Instantiate(weaponObjects[weapon], transform.position, Quaternion.identity);
        spawnWeapon.transform.parent = transform;
        spawnWeapon.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    
}
