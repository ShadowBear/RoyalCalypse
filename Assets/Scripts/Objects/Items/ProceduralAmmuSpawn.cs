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

        int ammu = 0;
        float rand = Random.Range(0f, 1f);
        if (rand <= 1f && rand >= 0.85f) ammu = 3;
        else if (rand < 0.85f && rand >= 0.70f) ammu = 2;
        else if (rand < 0.70f && rand >= 0.55f) ammu = 1;
        else ammu = 0;

        //int ammu = Random.Range(0, ammuObjects.Count);
        GameObject spawnAmmu = Instantiate(ammuObjects[ammu], transform.position, Quaternion.identity);
        spawnAmmu.transform.parent = transform;
        if (ammu == 3) spawnAmmu.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
    }
}
