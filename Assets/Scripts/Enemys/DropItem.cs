using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField]
    private GameObject[] drops;

    [SerializeField]
    private GameObject ep;
    private int epValue = 5;
    private float explosionRadius = 5;
    private int explosionForce = 250;
    private Vector3 offset;

    //DropRate in Percent 0-1
    [Range(0, 1)]
    public float dropRate;

    void Start()
    {
        offset = new Vector3(0, .5f, 0);
    }

    public void SetExp(int exp)
    {
        epValue = exp;
    }


    public void DropItemObject(float delayTime)
    {
        StartCoroutine(Drop(delayTime));    
    }

    IEnumerator Drop(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Random.Range(0, 1) <= dropRate)
        {
            if (drops.Length > 0)
            {
                int item = Random.Range(0, drops.Length);
                Instantiate(drops[item], transform.position + offset, transform.rotation);
            }
        }
        GameObject epObject = Instantiate(ep, transform.position + offset, transform.rotation);
        epObject.GetComponentInChildren<Experience>().experience = epValue;
        Vector3 randomExplosionVector = new Vector3(Random.Range(-1f, 1f), 0.9f, Random.Range(-1f, 1f));
        epObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position + randomExplosionVector, explosionRadius);
        Destroy(gameObject);
    }
}
