using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject suicide;
    public float speed;
    private GameObject player;

    public int suicideDmg;

    // Start is called before the first frame update
    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        player = Player.player.gameObject;
    }

    private void Update()
    {
        transform.LookAt(player.transform.GetChild(0));
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        if (Vector3.Distance(transform.position, player.transform.GetChild(0).position) >= 1.42f)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            Suicide();
        }
    }

    public void Die()
    {
        GameManager.gameManager.AddScore();
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Suicide()
    {
        player.GetComponent<Health>().Damage(suicideDmg);
        Instantiate(suicide, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
