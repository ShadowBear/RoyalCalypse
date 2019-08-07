using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerTrigger : MonoBehaviour
{
    [SerializeField] Transform resetPoint;
    [SerializeField] int resetDMG = 25;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().Damage(resetDMG);
            other.transform.position = resetPoint.position;
        }
    }
}
