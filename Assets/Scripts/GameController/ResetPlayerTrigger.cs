using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerTrigger : MonoBehaviour
{
    Transform resetPoint;
    int resetDMG = 45;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            resetPoint = GetComponent<LevelPlattformGenerator>().GetStartingTile().tile.transform.GetChild(0);
            other.GetComponent<Health>().Damage(resetDMG);
            other.transform.position = resetPoint.position;
        }
    }
}
