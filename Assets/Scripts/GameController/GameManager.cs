﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool stageClear = false;
    public int stageDepth = 1;

    public int score = 0;
    public int kills = 0;
    public int gold = 0;
    public int acutalRaidGold = 0;
    public int exp = 0;
    public int lvl = 0;
    public int maxExp = 0;
    public int attack = 0;
    public int maxHealth = 0;
    //public int weaponID = 0;
    public int hearths = 0;
    public int gems = 0;
    public int acutalRaidGem = 0;

    public GameObject weaponItemUIParent;
    public bool[] unlockedWeapons = new bool[18];

    public string firstWeaponID = "";

    public string playerName = "ShadowBear";
        
    //public Text timeText;
    [SerializeField] private float time;
    [SerializeField] bool timeRun;
    
    [SerializeField] private GameObject damageText;
    public enum bulletType { Bullet, Rocket, Liquid, Shotgun };
    public static GameManager gameManager;
    [HideInInspector]public GUIManager guiManager;

    public GameObject[] inventar;
    

    private void Awake()
    {
        if (gameManager == null) gameManager = this;
        else if (gameManager != this) Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponItemUIParent = GameObject.FindGameObjectWithTag("WeaponItemParent");
        weaponItemUIParent.transform.parent.parent.gameObject.SetActive(false);
        LoadGame();
        Time.timeScale = 1;
        if (GameObject.FindGameObjectWithTag("Canvas"))
        {
            guiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GUIManager>();
            guiManager.UpdateScore();
        }
        
        //if (timeRun) StartCoroutine(UpdateTime());
        //else timeText.enabled = false;
        SceneManager.sceneLoaded += SceneChanged;
    }

    private void Update()
    {
        if (timeRun)
        {
            time -= Time.deltaTime;
            if (time <= -1) Player.player.GetComponent<Health>().LostGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SceneChanged(Scene scene, LoadSceneMode loadSceneMode)
    {
        weaponItemUIParent = GameObject.FindGameObjectWithTag("WeaponItemParent");
        if (weaponItemUIParent != null)
        {
            LoadUI();
            weaponItemUIParent.transform.parent.parent.gameObject.SetActive(false);
        }
        SaveGame();
        if (scene != SceneManager.GetSceneByBuildIndex(0))
        {
            Time.timeScale = 1;
            stageClear = false;
            score = 0;
            guiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GUIManager>();
            guiManager.UpdateScore();
        }
        //if (scene == SceneManager.GetSceneByBuildIndex(0))
        //{
        //    MenuManager menuManager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        //    menuManager.ResetItemInInventory(firstWeaponID);
        //}
    }

    public void AddScore()
    {
        score++;
        guiManager.UpdateScore();
    }

    public void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        guiManager.UpdateScore();
    }

    public void AddGold(int goldAmount)
    {
        acutalRaidGold += goldAmount;
        guiManager.UpdateScore();
    }

    public void AddGem(int gemAmount)
    {
        acutalRaidGem += gemAmount;
        guiManager.UpdateScore();
    }

    public void AddScore(int scoreAmount, bool kill)
    {
        score += scoreAmount;
        kills++;
        guiManager.UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        kills = 0;
        guiManager.UpdateScore();
    }

    public void Lost()
    {
        guiManager.Lost();
    }

    public void HomeCalled()
    {
        gold += acutalRaidGold;
        gems += acutalRaidGem;
        acutalRaidGem = 0;
        acutalRaidGold = 0;
    }

    public void ShowDamageText(int damage, Transform displayTrans)
    {
        GameObject dmgText = Instantiate(damageText);
        if (!GameObject.FindGameObjectWithTag("Canvas")) return;
        dmgText.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        dmgText.GetComponentInChildren<Text>().text = damage.ToString();
        if (damage > 25) dmgText.GetComponentInChildren<Text>().color = Color.red;
        dmgText.gameObject.SetActive(true);

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(displayTrans.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 0f), 0));
        dmgText.transform.position = screenPosition;

    }
      

    public void NewGame()
    {        
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void TreasureFound()
    {
        stageClear = true;
    }

    public void SaveGame()
    {
        //if (weaponItemUIParent != null)
        //{
        //    WeaponSelectInventoryButton[] weaponsUI = weaponItemUIParent.GetComponentsInChildren<WeaponSelectInventoryButton>();
        //    for (int i = 0; i < weaponsUI.Length; i++)
        //    {
        //        unlockedWeapons[i] = weaponsUI[i].GetLockState();
        //    }
        //}
        SaveSystem.SaveGame(this);
    }


    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data != null)
        {
            gold = data.gold;
            exp = data.exp;
            lvl = data.lvl;
            maxExp = data.maxExp;
            attack = data.attack;
            maxHealth = data.maxHealth;
            hearths = data.hearths;
            gems = data.gems;

            firstWeaponID = data.firstWeaponID;
            playerName = data.playerName;

            unlockedWeapons = data.unlockedweapons;
        }
        LoadUI();        
    }

    public void LoadUI()
    {
        WeaponSelectInventoryButton[] weaponsUI = weaponItemUIParent.GetComponentsInChildren<WeaponSelectInventoryButton>();
        for (int i = 0; i < weaponsUI.Length; i++)
        {
            weaponsUI[i].Unlock(unlockedWeapons[i]);
        }
    }

    //IEnumerator UpdateTime()
    //{
    //    while(time >= -1) {
    //        timeText.text = Mathf.Ceil(time).ToString();
    //        if (time < 10) timeText.color = Color.red;
    //        else timeText.color = Color.white;
    //        yield return new WaitForSeconds(.2f);
    //    }
    //}
}
