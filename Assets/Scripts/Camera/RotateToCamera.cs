using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    Camera mainCamera;
    public bool isUIIcon = false;
    Quaternion startRotation;
    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        startRotation = transform.rotation;
        //if(!isUIIcon) StartCoroutine(LookAtCamera());
        //else
        //{
        //    startRotation = transform.rotation;
        //    StartCoroutine(StopRotationCamera());
        //}
    }

    private void Update()
    {
        if(!isUIIcon) transform.LookAt(mainCamera.transform);
        else transform.rotation = startRotation;
    }

    IEnumerator LookAtCamera()
    {
        while (true)
        {
            transform.LookAt(mainCamera.transform);
            yield return new WaitForSeconds(.02f);
        }
        
    }

    IEnumerator StopRotationCamera()
    {
        while (true)
        {
            transform.rotation = startRotation;
            yield return new WaitForSeconds(.02f);
        }

    }
}
