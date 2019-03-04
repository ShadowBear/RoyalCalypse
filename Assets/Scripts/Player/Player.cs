using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player player;
    public Joystick movementJoystick;
    public Joystick rotationJoystick;
    public float movementMaxSpeed;
    public float movementSpeed;
    private Vector3 movement;
    private Vector3 rotation;
    private Rigidbody playerRigidbody;

    public Transform gunPoint;
    public bool arming;

    public GameObject line;
    private Animator anim;

    public GameObject equipedWeapon;
    private Weapon equipedWeaponScript;
    [SerializeField] private GameObject secondWeapon;
    [SerializeField] private Image firstWeaponImage;
    [SerializeField] private Image secondWeaponImage;
    
    void Start()
    {
        if (player == null) player = this;
        else if (player != this) Destroy(gameObject);

        playerRigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        arming = false;
        line.SetActive(false);
        equipedWeaponScript = equipedWeapon.GetComponent<Weapon>();
        firstWeaponImage.sprite = equipedWeaponScript.guiImageWeapon;
        secondWeaponImage.sprite = secondWeapon.GetComponent<Weapon>().guiImageWeapon;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = movementJoystick.Horizontal;
        float v = movementJoystick.Vertical;
        if (h > 0f || h < 0f) movementSpeed = movementMaxSpeed;
        else if (v > 0f || v < 0f) movementSpeed = movementMaxSpeed;
        else movementSpeed = 0f;
        Move(h, v);
        Rotate(rotationJoystick.Horizontal, rotationJoystick.Vertical);
        Animate(h,v);
    }

    void Move(float h, float v)
    {
        movement.Set(h, 0, v);
        movement = movement.normalized * movementSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Rotate(float h, float v)
    {
        rotation.Set(h, 0, v);
        if (rotation != Vector3.zero)
        {
            transform.forward = rotation;
            line.SetActive(true);
            equipedWeaponScript.attacking = true;
        }
        else
        {
            equipedWeaponScript.attacking = false;
            line.SetActive(false);
            rotation.Set(movementJoystick.Horizontal, 0, movementJoystick.Vertical);
            if (rotation != Vector3.zero) transform.forward = rotation;
        }
    }

    public void Attack()
    {
        equipedWeaponScript.Attack();
    }

    public void SwapWeapon(string weaponName)
    {
        if(secondWeapon == null)
        {
            secondWeapon = equipedWeapon;
            Sprite weaponSprite = equipedWeaponScript.guiImageWeapon;
            secondWeaponImage.sprite = weaponSprite;
        }
        Transform gunTrans = transform.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/" + weaponName);
        GameObject weapon = gunTrans.gameObject;
        equipedWeapon.SetActive(false);
        equipedWeapon = weapon;
        equipedWeaponScript = equipedWeapon.GetComponent<Weapon>();
        if (firstWeaponImage.GetComponentInParent<WeaponButton>().GetStatus())
        {
            firstWeaponImage.sprite = equipedWeaponScript.guiImageWeapon;
        }else secondWeaponImage.sprite = equipedWeaponScript.guiImageWeapon;

        equipedWeapon.SetActive(true);
    }

    public void SwapSecondWeapon()
    {
        GameObject temp = equipedWeapon;

        equipedWeapon.SetActive(false);  
        
        equipedWeapon = secondWeapon;
        secondWeapon = temp;
        
        equipedWeaponScript = equipedWeapon.GetComponent<Weapon>();
        equipedWeapon.SetActive(true);
    }

    public int GetWeaponDamage()
    {
        int damage = equipedWeaponScript.damage;
        int varianz = equipedWeaponScript.damageVarianz;
        damage = damage + Random.Range(-varianz, varianz + 1);
        return damage;
    }
    public bool AddAmmu(GameManager.bulletType type, int amount)
    {
        return equipedWeaponScript.AddAmmu(type, amount);
    }

    public bool AddAmmu(GameManager.bulletType type)
    {
        bool reloaded = equipedWeaponScript.AddAmmu(type);
        if (reloaded) return true;
        else
        {
            reloaded = secondWeapon.GetComponent<Weapon>().AddSecondAmmu(type);
            equipedWeaponScript.UpdateAmmuGUI();
            return reloaded;
        }
        
    }

    void Animate(float h, float v)
    {
        if (h != 0 || v != 0) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
        anim.SetBool("Attack", arming);
        int direction = GetDirection(h, v);
        if(rotationJoystick.Horizontal != 0f || rotationJoystick.Vertical != 0f)
        {
            anim.SetInteger("Direction", direction);
        }else anim.SetInteger("Direction", -1);

    }

    public int GetDirection(float h, float v)
    {
        // Animation Direction:
        // Forward = -1
        // Right = 2
        // Left = 1
        // Backward = 0
        Vector3 localRotate = transform.TransformDirection(Camera.main.transform.forward);
        if(Mathf.Abs(h) > Mathf.Abs(v))
        {
            if(h > 0f)
            {
                if (Mathf.Abs(localRotate.z) > Mathf.Abs(localRotate.x))
                {
                    //Forward & Backward
                    if (localRotate.z > 0.1f) return 2;
                    else if (localRotate.z < -0.1f) return 1;
                }
                else
                {
                    //Left & Right
                    if (localRotate.x > 0f) return -1;
                    else if (localRotate.x < 0f) return 0;
                }
            }
            else if(h < 0)
            {
                if (Mathf.Abs(localRotate.z) > Mathf.Abs(localRotate.x))
                {
                    //Forward & Backward
                    if (localRotate.z > 0.1f) return 1;
                    else if (localRotate.z < -0.1f) return 2;
                }
                else
                {
                    //Left & Right
                    if (localRotate.x > 0f) return 0;
                    else if (localRotate.x < 0f) return -1;
                }
            }
            
        }
        else
        {
            if(v > 0f)
            {
                if (Mathf.Abs(localRotate.z) > Mathf.Abs(localRotate.x))
                {
                    //Forward & Backward
                    if (localRotate.z > 0.1f) return -1;
                    else if (localRotate.z < -0.1f) return 0;
                }
                else
                {
                    //Left & Right
                    if (localRotate.x > 0f) return 1;
                    else if (localRotate.x < 0f) return 2;
                }
            }
            else if(v < 0f)
            {
                if (Mathf.Abs(localRotate.z) > Mathf.Abs(localRotate.x))
                {
                    //Forward & Backward
                    if (localRotate.z > 0.1f) return 0;
                    else if (localRotate.z < -0.1f) return -1;
                }
                else
                {
                    //Left & Right
                    if (localRotate.x > 0f) return 2;
                    else if (localRotate.x < 0f) return 1;
                }
            }


            
        }
        
        return 0;
    }
}

/** 
 * Animation for Apocalypse Enemys
 * 
 * void Animate(float h, float v)
    {
        anim.SetInteger("WeaponType_int", weaponType);
        if (h != 0 || v != 0)
        {
            anim.SetFloat("Speed_f", 1);
            if (anim.GetInteger("WeaponType_int") != 0)
            {
                if (anim.GetInteger("WeaponType_int") == 1)
                {
                    anim.SetFloat("Body_Vertical_f", 0.2f);
                    anim.SetFloat("Body_Horizontal_f", 0f);
                }
                else
                {
                    anim.SetFloat("Body_Vertical_f", 0.6f);
                    anim.SetFloat("Body_Horizontal_f", 0.3f);
                }
            }
            else
            {
                anim.SetFloat("Body_Vertical_f", 0f);
                anim.SetFloat("Body_Horizontal_f", 0f);
            }
        }
        else
        {
            anim.SetFloat("Speed_f", 0);
            anim.SetFloat("Body_Vertical_f", 0f);
            anim.SetFloat("Body_Horizontal_f", 0f);
        }

    }*/
