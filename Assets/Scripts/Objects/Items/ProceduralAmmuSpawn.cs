using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAmmuSpawn : MonoBehaviour
{
    [SerializeField] GameObject ammuParent;
    //[SerializeField] GameObject spawnWeapon;

    // Start is called before the first frame update
    public void Spawn()
    {
        List<GameObject> ammuObjects = new List<GameObject>();
        Transform[] weapons = ammuParent.GetComponentsInChildren<Transform>();

        foreach (Transform t in weapons)
        {
            if (t.CompareTag("Ammu"))
            {
                ammuObjects.Add(t.gameObject);
            }
        }
        int ammu = Random.Range(0, ammuObjects.Count);
        GameObject spawnAmmu = Instantiate(ammuObjects[ammu], transform.position, Quaternion.identity);
        spawnAmmu.transform.parent = transform;
        if (ammu == 3) spawnAmmu.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
    }
}
