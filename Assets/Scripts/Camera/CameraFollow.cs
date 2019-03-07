using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerToFollow;
    private Vector3 offset;
    public float smoothing;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3 (0, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, (playerToFollow.position + offset), smoothing);
    }
}
