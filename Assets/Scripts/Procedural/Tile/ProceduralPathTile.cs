using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPathTile : MonoBehaviour
{
    public GameObject[] houseProps;
    public GameObject[] specialProps;
    public GameObject[] churchProps;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in GetComponentsInChildren<Transform>())
        {
            if (item.transform == transform) continue;
            if (Random.value > 0.25f)
            {
                float percent = Random.value;
                if (percent > 0.95f) Instantiate(churchProps[Random.Range(0, churchProps.Length)], item.position, Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
                else if (percent > 0.80f) Instantiate(specialProps[Random.Range(0, specialProps.Length)], item.position, Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
                else  Instantiate(houseProps[Random.Range(0, houseProps.Length)], item.position + new Vector3(0, Random.Range(-0.15f, 0f), 0), Quaternion.Euler(0, Random.Range(-180, 180), 0), transform);
            }
        }
    }
}
