using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAITrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAI"))
        {
            if (other.transform.childCount == 0)
            {
                GameObject enemyToSpawn = other.GetComponent<EnemyAISpawner>().GetEnemyType();

                if (enemyToSpawn)
                {
                    GameObject enemyAI = Instantiate(enemyToSpawn, other.transform.position, Quaternion.identity);
                    enemyAI.transform.parent = other.transform;
                }
            }            
        }
        if (other.CompareTag("Item"))
        {
            if(other.transform.childCount == 0)
            {
                if (other.GetComponent<ProceduralAmmuSpawn>())
                {
                    other.GetComponent<ProceduralAmmuSpawn>().Spawn();
                }
                else if (other.GetComponent<ProceduralWeaponSpawn>())
                {
                    other.GetComponent<ProceduralWeaponSpawn>().Spawn();
                }
                else if (other.GetComponent<ProceduralRegenerationSpawn>())
                {
                    other.GetComponent<ProceduralRegenerationSpawn>().Spawn();
                }
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyAI") || other.CompareTag("Item"))
        {
            if (other.transform.childCount == 1) Destroy(other.transform.GetChild(0).gameObject);
        }
    }
}
