using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestEnemyTrigger : Health
{
    public GameObject[] lowEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] hardEnemies;

    [SerializeField] GameObject fragmentsFX;
    [SerializeField] GameObject smokeFX;
    int hitsBeforeDie = 1;

    public GameObject rewardCoin;

    private AudioSource audioSource;
    private Renderer rend;
    private Collider col;

    public int enemyLevel = 1;
    public int enemyAmount = 3;
    

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

        SpawnReward();
        Destroy(smoke, 2f);

        //audioSource.pitch = Random.Range(0.75f, 1.25f);
        //audioSource.volume = Random.Range(0.85f, 1.15f);
        //audioSource.Play();

        StartCoroutine(TriggerEnemies(enemyLevel, enemyAmount));
        //Destroy(gameObject, audioSource.clip.length);
        Destroy(gameObject, 2f);
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
                return;
            }
        }
    }

    private void SpawnReward()
    {
        GameObject goldSack = Instantiate(rewardCoin, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        if (goldSack.GetComponent<CollectGold>()) goldSack.GetComponent<CollectGold>().SetGoldAmount(Random.Range(5, 25));
    }

    IEnumerator TriggerEnemies(int level, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 rndPos = Random.insideUnitCircle * 5.5f;
            Vector3 pos = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(rndPos.x, 0, rndPos.y);
            GameObject smoke = Instantiate(smokeFX, pos, Quaternion.identity);
            smoke.transform.rotation = Quaternion.Euler(90, 0, 0);
            Destroy(smoke, 2f);
            yield return new WaitForSeconds(0.2f);
            if (level == 1) Instantiate(lowEnemies[Random.Range(0, lowEnemies.Length)], pos, Quaternion.identity);
            else if (level == 2) Instantiate(mediumEnemies[Random.Range(0, mediumEnemies.Length)], pos, Quaternion.identity);
            else Instantiate(hardEnemies[Random.Range(0, hardEnemies.Length)], pos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }

        yield return null;
    }
}
