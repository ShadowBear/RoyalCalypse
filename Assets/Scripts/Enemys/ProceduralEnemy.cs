using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralEnemy : MonoBehaviour
{
    [SerializeField] GameObject weaponParent;
    [SerializeField] GameObject avatarParent;
    public bool forceMelee = false;
    public bool forceRange = false;
    int avatar = -1;
    int weapon = -1;
    private EnemyAISpawner enemyAISpawner;


    private void Awake()
    {
        Transform[] avatars = avatarParent.GetComponentsInChildren<Transform>();
        List<GameObject> weaponObjects = new List<GameObject>();
        Transform[] weapons = weaponParent.GetComponentsInChildren<Transform>();
        //if (GetComponentInParent<EnemyAISpawner>().avatar == -1)
        //{
        //    avatar = Random.Range(1, avatars.Length);
        //    GetComponentInParent<EnemyAISpawner>().avatar = avatar;
        //    Debug.Log("Kein Avatar vorher");
        //}
        //else avatar = GetComponentInParent<EnemyAISpawner>().avatar;
        GetAvatar(avatars);

        for (int i = 1; i < avatars.Length; i++)
        {
            if (i == avatar) avatars[i].gameObject.SetActive(true);
            else avatars[i].gameObject.SetActive(false);
        }
        
        foreach(Transform t in weapons)
        {
            if (t.CompareTag("Weapon"))
            {
                weaponObjects.Add(t.gameObject);
            }
        }


        //if (GetComponentInParent<EnemyAISpawner>().weapon == -1)
        //{
        //    if (forceMelee) weapon = Random.Range(0, 4);
        //    else if (forceRange) weapon = Random.Range(4, weaponObjects.Count - 1);
        //    else weapon = Random.Range(0, weaponObjects.Count - 1);

        //    GetComponentInParent<EnemyAISpawner>().weapon = weapon;
        //}
        //else weapon = GetComponentInParent<EnemyAISpawner>().weapon;
        
        if (forceMelee) weapon = Random.Range(0, 4);
        else if (forceRange) weapon = Random.Range(4, weaponObjects.Count - 1);
        else weapon = Random.Range(0, weaponObjects.Count - 1);

        for (int i = 0; i < weaponObjects.Count; i++)
        {
            if (i == weapon) weaponObjects[i].SetActive(true);
            else weaponObjects[i].SetActive(false);
        }
        if (weaponObjects[weapon].GetComponent<Weapon>()) GetComponent<EnemyAlwaysAttackAI>().melee = false;
        else GetComponent<EnemyAlwaysAttackAI>().melee = true;

    }

    private void GetAvatar(Transform [] avatars)
    {
        avatar = Random.Range(1, avatars.Length);
        if (avatar == 47) GetAvatar(avatars);        
    }

}
