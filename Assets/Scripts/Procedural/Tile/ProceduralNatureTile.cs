using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralNatureTile : MonoBehaviour
{
    public GameObject[] woodProps;
    public GameObject[] rockProps;
    public GameObject[] shroomsProps;
    public GameObject[] bushesProps;
    public GameObject grasProp;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in GetComponentsInChildren<Transform>())
        {
            if (item.transform == transform) continue;
            if (Random.value > 0.60f)
            {
                float percent = Random.value;
                if(percent >0.925f) Instantiate(shroomsProps[Random.Range(0, shroomsProps.Length)], item.position + new Vector3(0,Random.Range(-0.3f, 0f),0), Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
                else if (percent > 0.85f) Instantiate(bushesProps[Random.Range(0, bushesProps.Length)], item.position, Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
                else if (percent > 0.775f) Instantiate(grasProp, item.position, Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
                else if (percent > 0.20f) Instantiate(woodProps[Random.Range(0, woodProps.Length)], item.position + new Vector3(0, Random.Range(-0.15f, 0f), 0), Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
                else Instantiate(rockProps[Random.Range(0, rockProps.Length)], item.position + new Vector3(0, Random.Range(-0.075f, 0f), 0), Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
            }
        }
    }

    
}
