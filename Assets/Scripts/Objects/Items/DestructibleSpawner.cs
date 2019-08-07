using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestructibleSpawner : DestructableItem
{
    public GameObject spawnObject;
    private int spawnAmount = 0;
    public bool dontSpawn = false;
    // Start is called before the first frame update
    protected override void Die()
    {
        base.Die();
        if (!dontSpawn && Random.Range(0f, 1f) < 0.65f) spawnAmount = Random.Range(1, 4);
        //Debug.Log(spawnAmount);
        //NavMeshHit hit;
        //if(NavMesh.SamplePosition(transform.position, out hit, 2.5f, NavMesh.AllAreas))
        //{
        Vector3 pos = new Vector3(transform.position.x, 0.1f, transform.position.z);
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject spawn = Instantiate(spawnObject, pos, Quaternion.identity);
        }
        //}              
    }
}
