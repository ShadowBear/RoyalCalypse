using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LookAtCamera());
    }

    IEnumerator LookAtCamera()
    {
        while (true)
        {
            transform.LookAt(Camera.main.transform);
            yield return new WaitForSeconds(.034f);
        }
        
    }
}
