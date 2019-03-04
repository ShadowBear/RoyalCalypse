using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, 1.5f);
    }

    void SpawnEnemy()
    {
        Instantiate(enemy, new Vector3(Random.Range(-9f, 9f), 0.5f, Random.Range(-9f, 9f)), Quaternion.identity);
    }
}
