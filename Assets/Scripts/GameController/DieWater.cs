using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWater : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.gameManager.Lost();
        }
    }
}
