using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAISpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyType;
    public GameObject GetEnemyType()
    {
        return enemyType;
    }
}
