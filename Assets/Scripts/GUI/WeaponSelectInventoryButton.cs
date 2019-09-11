using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectInventoryButton : MonoBehaviour
{
    private Image image;
    [SerializeField] bool unlocked = false;

    public Sprite selectedObjectSprite;
    public Sprite unselectedObjectSprite;
    [SerializeField] Image lockedSpriteImage;
    public bool selected = false;
    public string weaponID;
    public int saveID;
    MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        image = GetComponent<Image>();
        lockedSpriteImage = transform.GetChild(2).GetComponent<Image>();
        lockedSpriteImage.enabled = !unlocked;
    }

    public void Selected()
    {
        if (!selected && unlocked)
        {
            menuManager.SelectItemInInventory(weaponID);
            GameManager.gameManager.firstWeaponID = weaponID;
        }
    }

    public void ToogleSelected()
    {
        selected = selected ? false : true;
        if (selected) image.sprite = selectedObjectSprite;
        else image.sprite = unselectedObjectSprite;
    }

    public void SetItemSelected()
    {
        selected = true;
        if(image) image.sprite = selectedObjectSprite;
        else GetComponent<Image>().sprite = selectedObjectSprite;
    }

    public void SetItemUnSelected()
    {
        selected = false;
        if (image) image.sprite = unselectedObjectSprite;
        else GetComponent<Image>().sprite = unselectedObjectSprite;
    }

    public void Unlock(bool lockState)
    {
        unlocked = lockState;
        UpdateSpriteImage();
        GameManager.gameManager.unlockedWeapons[saveID] = lockState;
    }   

    public bool GetLockState()
    {
        return unlocked;
    }

    public void UpdateSpriteImage()
    {
        lockedSpriteImage.enabled = !unlocked;
    }

}
