using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDestruction : Health
{
    [SerializeField] GameObject fragmentsFX;
    [SerializeField] int hitsBeforeDie = 1;

    private AudioSource audioSource;


    // Start is called before the first frame update
    protected override void Start()
    {
        audioSource = GetComponent<AudioSource>();
        health = hitsBeforeDie;
        shield = 0;
    }
    protected override void Die()
    {
        GameObject splash = Instantiate(fragmentsFX, transform.position, Quaternion.identity);
        Destroy(splash, 2f);
        foreach(Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t == transform) continue;
            t.gameObject.SetActive(false);
        }
        audioSource.pitch = Random.Range(0.75f, 1.25f);
        audioSource.volume = Random.Range(0.85f, 1.15f);
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
    }

    public override void Damage(int dmg)
    {
        if (dmg > 0)
        {
            if (!alive) return;
            hitsBeforeDie--;

            if (hitsBeforeDie <= 0)
            {
                alive = false;
                Die();
            }
        }
    }
}
