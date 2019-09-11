using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGem : MonoBehaviour
{
    [SerializeField] int gemAmount;
    private AudioSource audioSource;
    public GameObject portal;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        portal.SetActive(false);
        int additionalGems = 0;
        if (Random.value > 0.98f) additionalGems = 2;
        else if (Random.value > 0.9f) additionalGems = 1;
        gemAmount = GameManager.gameManager.stageDepth + additionalGems;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.gameManager.AddGem(gemAmount);
            GameManager.gameManager.stageClear = true;
            if(audioSource.clip) AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
            portal.SetActive(true);
            Destroy(gameObject);
        }
    }
}
