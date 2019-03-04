using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
