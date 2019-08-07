using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldIcon : MonoBehaviour
{
    public GameObject shieldImage;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        shieldImage.SetActive(false);
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(image.fillAmount > 0)
        {
            if (!shieldImage.activeSelf) shieldImage.SetActive(true);
        }
        else
        {
            if (shieldImage.activeSelf) shieldImage.SetActive(false);
        }
    }
}
