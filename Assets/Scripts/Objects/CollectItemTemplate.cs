using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CollectItemTemplate : MonoBehaviour
{
    protected Image image;
    public float collectTime;

    void Start()
    {
        image = GetComponentInChildren<Image>();
        image.fillAmount = 0;
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            image.fillAmount += Time.deltaTime / collectTime;
            if (image.fillAmount == 1) CollectIt(other);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (image.fillAmount == 1) CollectIt(other);
            else image.fillAmount = 0;
        }
    }

    protected abstract void CollectIt(Collider player);
}
