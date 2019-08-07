using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRegenerationSpawn : MonoBehaviour
{
    [SerializeField] GameObject regenParent;
    private int regInt = -1;
    //[SerializeField] GameObject spawnWeapon;

    // Start is called before the first frame update
    public void Spawn()
    {
        List<GameObject> regenObjects = new List<GameObject>();
        Transform[] weapons = regenParent.GetComponentsInChildren<Transform>();

        foreach (Transform t in weapons)
        {
            if (t.CompareTag("Regeneration"))
            {
                regenObjects.Add(t.gameObject);
            }
        }
        if(regInt == -1) regInt = Random.Range(0, regenObjects.Count);
        GameObject spawnReg = Instantiate(regenObjects[regInt], transform.position, Quaternion.identity);
        spawnReg.transform.parent = transform;
    }
}
