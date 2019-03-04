using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButton : MonoBehaviour
{

    private Button weaponButton;
    public Button otherWeaponButton;
    [SerializeField] private bool active;
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        weaponButton = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
        if (active) rect.sizeDelta = new Vector2(160, 110);
        else rect.sizeDelta = new Vector2(105, 75);
        weaponButton.onClick.AddListener(ChangeWeapon);
    }

    public void ChangeWeapon()
    {
        active = active ? false : true;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SwapSecondWeapon();
        Player.player.gameObject.GetComponent<Player>().SwapSecondWeapon();
        UpdateWeapon();
        otherWeaponButton.GetComponent<WeaponButton>().ChangeStatus();
        otherWeaponButton.GetComponent<WeaponButton>().UpdateWeapon();
    }
    public void ChangeStatus()
    {
        active = active ? false : true;
    }

    public void UpdateWeapon()
    {
        if (active) rect.sizeDelta = new Vector2(160, 110);
        else rect.sizeDelta = new Vector2(105, 75);

    }

    public bool GetStatus()
    {
        return active;
    }


    

}
