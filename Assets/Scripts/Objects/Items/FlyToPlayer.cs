using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToPlayer : MonoBehaviour
{
    int speed = 250;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 direction = other.transform.position - transform.position;
            GetComponentInParent<Rigidbody>().AddForce(direction * speed * Time.deltaTime);
        }        
    }
}
