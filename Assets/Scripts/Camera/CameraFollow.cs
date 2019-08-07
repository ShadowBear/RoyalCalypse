using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform playerToFollow;
    [SerializeField] Vector3 offset;
    public float smoothing;
    // Start is called before the first frame update
    void Start()
    {
        if (playerToFollow == null) playerToFollow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, (playerToFollow.position + offset), smoothing);
    }
}
