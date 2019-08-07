using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private GameObject burningObjekt;
    [SerializeField] GameObject explosion;
    [SerializeField] int explosionDMG;
    [SerializeField] float explosionRadius;
    [SerializeField] float detonationTime;
    [SerializeField] LayerMask damageLayer;

    //Testing Bool
    public bool activated = false;
    [SerializeField] bool collideExplosion = false;
    
    // Start is called before the first frame update
    void Start()
    {
        burningObjekt = transform.GetChild(0).gameObject;
        burningObjekt.SetActive(false);
        if (activated) FlameIt();
    }

    //private void Update()
    //{
    //    if (activated) FlameIt();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!activated && collideExplosion)
        {
            if (other.CompareTag("Player"))
            {
                activated = true;
                FlameIt();
            }
        }
    }

    public void FlameIt()
    {
        StartCoroutine(Detonation());        
    }

    IEnumerator Detonation()
    {
        burningObjekt.SetActive(true);
        yield return new WaitForSeconds(detonationTime);
        burningObjekt.SetActive(false);
        GameObject explo = Instantiate(explosion, transform.position, Quaternion.identity);
        explo.transform.localScale *= 2;
        Destroy(explo, 2f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            Health health = colliders[i].GetComponent<Health>();
            if (!health) continue;

            int damageToApplie = CalculateDamage(colliders[i].transform.position);
            health.Damage(damageToApplie);
        }
        Destroy(gameObject);
        yield return null;
    }


    private int CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damageAmount = relativeDistance * explosionDMG;

        // Make sure that the minimum damage is always 0.
        damageAmount = Mathf.Max(0f, damageAmount);

        return (int)damageAmount;
    }
}
