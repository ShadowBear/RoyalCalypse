using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolder : MonoBehaviour
{

    private bool collisionDetected = false;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("GroundTile"))
        {
            collisionDetected = true;
            Debug.Log("Collide with Object");
        }
    }

    public bool GetCollisionState()
    {
        return collisionDetected;
    }
}
