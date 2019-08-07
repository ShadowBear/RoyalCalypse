using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyKiller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(KillRigid());
    }

    IEnumerator KillRigid()
    {
        yield return new WaitForSeconds(1.5f);
        if(transform.GetComponent<Rigidbody>()) Destroy(transform.GetComponent<Rigidbody>());
        Destroy(this);
        yield return null;
    }
}
