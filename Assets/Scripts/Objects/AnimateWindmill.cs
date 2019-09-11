using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWindmill : MonoBehaviour
{
    private int speed;

    public void Start()
    {
        speed = Random.Range(25, 50);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
