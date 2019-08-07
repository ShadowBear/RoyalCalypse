using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShipBombing : MonoBehaviour
{
    // Start is called before the first frame update
    public bool firing = false;
    [SerializeField] GameObject bomb;
    [SerializeField] Transform[] shotPoints;
    [SerializeField] float bulletForce = 25;
    [SerializeField] float minBulletForce = 175;
    [SerializeField] float maxBulletForce = 400;
    [SerializeField] float attackSpeed = 1.5f;
    [SerializeField] bool rdyToShot = true;

    // Update is called once per frame
    void Update()
    {
        if (firing && rdyToShot) StartCoroutine(Fire());
    }


    IEnumerator Fire()
    {
        rdyToShot = false;
        bulletForce = Random.Range(minBulletForce, maxBulletForce);
        int bullets = Random.Range(1, shotPoints.Length);
        if(bullets == 1)
        {
            int r = Random.Range(0, shotPoints.Length);
            GameObject bullet = Instantiate(bomb, shotPoints[r].position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(shotPoints[r].forward * bulletForce);
        }
        else if(bullets == 2)
        {
            GameObject bullet = Instantiate(bomb, shotPoints[0].position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(shotPoints[0].forward * bulletForce);

            bullet = Instantiate(bomb, shotPoints[2].position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(shotPoints[2].forward * bulletForce);
        }
        else
        {
            foreach(Transform t in shotPoints)
            {
                GameObject bullet = Instantiate(bomb, t.position, Quaternion.identity);
                bullet.transform.localScale *= 0.2f;
                bullet.GetComponent<Rigidbody>().AddForce(t.forward * bulletForce);
            }
        }
        yield return new WaitForSeconds(attackSpeed);
        rdyToShot = true;
        yield return null;
    }
}
