using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;


public class Weapon : MonoBehaviour
{
    public int damage;
    public int damageVarianz;
    public float fireRate;
    public float projectileSpeed;
    public float dmgRadius;
    [SerializeField]private int maxAmmu;
    [SerializeField] private int startAmmu;
    [SerializeField] private int reloadAmount;
    
    [SerializeField] private GameManager.bulletType typeOfBullet;
    private int ammuAmount;
    public bool autoFire;
    public int weaponTyp;
    public GameObject bullet;

    public bool attacking;
    protected bool canAttack;

    protected Transform gunPoint;

    public Sprite guiImageWeapon;
    private Text ammuText;
    private GameObject guiCanvas;

    protected virtual void Start()
    {
        if (guiCanvas != null) return;
        ammuAmount = startAmmu;
        attacking = false;
        canAttack = true;
        gunPoint = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().gunPoint;
        //gunPoint = Player.player.gameObject.GetComponent<Player>().gunPoint;
        guiCanvas = GameObject.FindGameObjectWithTag("Canvas");
        foreach (WeaponButton w in guiCanvas.GetComponentsInChildren<WeaponButton>())
        {
            if (w.GetStatus()) ammuText = w.GetComponentInChildren<Text>();
        }
        ammuText.text = ammuAmount.ToString();
    }

    protected virtual void Update()
    {
        if (autoFire) Attack();
    }

    public virtual void Attack()
    {
        if (ammuAmount > 0 && canAttack && ((autoFire && attacking) || !autoFire)) StartCoroutine(Shot());

    }

    IEnumerator Shot()
    {
        canAttack = false;
        ammuAmount--;
        GameObject projectile = Instantiate(bullet, gunPoint.position, Quaternion.identity);
        if (projectile.GetComponent<PolygonArsenal.PolygonProjectileScript>())
        {
            projectile.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = GetWeaponDamage();
        }
        projectile.GetComponent<Rigidbody>().AddForce(gunPoint.forward * projectileSpeed);
        UpdateAmmuGUI();
        yield return new WaitForSeconds(fireRate);
        canAttack = true;
        yield return null;
    }

    public IEnumerator EnemyShot(Transform gunTransform)
    {
        canAttack = false;
        GameObject projectile = Instantiate(bullet, gunTransform.position, Quaternion.identity);
        if (projectile.GetComponent<PolygonArsenal.PolygonProjectileScript>())
        {
            projectile.GetComponent<PolygonArsenal.PolygonProjectileScript>().damage = GetWeaponDamage();
        }
        projectile.GetComponent<Rigidbody>().AddForce(gunTransform.forward * projectileSpeed);
        yield return new WaitForSeconds(fireRate);
        canAttack = true;
        yield return null;
    }

    public bool GetAttackStatus()
    {
        return canAttack;
    }

    public void UpdateAmmuGUI()
    {
        foreach (WeaponButton w in guiCanvas.GetComponentsInChildren<WeaponButton>())
        {
            if (w.GetStatus()) ammuText = w.GetComponentInChildren<Text>();
        }
        ammuText.text = ammuAmount.ToString();
    }

    public bool AddAmmu(GameManager.bulletType bullet, int amount)
    {
        if (bullet == typeOfBullet)
        {
            ammuAmount = (ammuAmount + amount) > maxAmmu ? maxAmmu : (ammuAmount + amount);
            UpdateAmmuGUI();
            return true;
        }
        else return false;
    }

    public bool AddAmmu(GameManager.bulletType bullet)
    {
        if (bullet == typeOfBullet)
        {
            ammuAmount = (ammuAmount + reloadAmount) > maxAmmu ? maxAmmu : (ammuAmount + reloadAmount);
            UpdateAmmuGUI();
            return true;
        }
        else return false;
    }

    public bool AddSecondAmmu(GameManager.bulletType bullet)
    {
        if (bullet == typeOfBullet)
        {
            if (guiCanvas == null) Start();
            ammuAmount = (ammuAmount + reloadAmount) > maxAmmu ? maxAmmu : (ammuAmount + reloadAmount);
            foreach (WeaponButton w in guiCanvas.GetComponentsInChildren<WeaponButton>())
            {
                if (!w.GetStatus()) ammuText = w.GetComponentInChildren<Text>();
            }
            ammuText.text = ammuAmount.ToString();
            return true;
        }
        else return false;
    }

    public int GetWeaponDamage()
    {
        return (damage + UnityEngine.Random.Range(-damageVarianz, damageVarianz + 1));
    }

}

/*   ****             Later To USE               ***   */

////JSON Externalisieren
//private string path;
//private string jsonString;
//public JSONWeapon weapon = new JSONWeapon();

//void Start(){
////JSON
//    path = Application.streamingAssetsPath + "/JSON/WeaponData.json";
//    jsonString = File.ReadAllText(path);
//    weapon = JsonUtility.FromJson<JSONWeapon>(jsonString);
//}

/*********************************************************/
