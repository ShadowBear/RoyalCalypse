using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableItem : Health
{

    [SerializeField] GameObject fragmentsFX;
    [SerializeField] int hitsBeforeDie = 1;

    private AudioSource audioSource;
    private Renderer rend;
    private Collider col;

    [SerializeField] int destructionScore = 1;
    

    // Start is called before the first frame update
    protected override void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        health = hitsBeforeDie;
        shield = 0;
    }

    protected override void Die()
    {
        rend.enabled = false;
        col.enabled = false;
        GameObject smoke = Instantiate(fragmentsFX, transform.position, Quaternion.identity);
        Destroy(smoke, 2f);
        if (audioSource)
        {
            audioSource.pitch = Random.Range(0.75f, 1.25f);
            audioSource.volume = Random.Range(0.85f, 1.15f);
            audioSource.Play();
        }        
        GameManager.gameManager.AddScore(destructionScore);
        Destroy(gameObject, audioSource.clip.length);
    }

    public override void Damage(int dmg)
    {
        if(dmg > 0)
        {
            if (!alive) return;
            hitsBeforeDie--;

            if(hitsBeforeDie <= 0)
            {
                alive = false;
                Die();
            }
        }        
    }
}
