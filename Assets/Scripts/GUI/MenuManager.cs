using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject stages;
    public GameObject playerInfo;
    public GameObject ranking;
    public GameObject missions;
    public GameObject items;
    public GameObject shop;
    public GameObject settings;
    public GameObject main;
    public GameObject exit;
    public GameObject load;

    GameObject currentState;

    [SerializeField] int exp;
    [SerializeField] int maxExp;

    public Text expText;
    public Text lvlText;
    public Text lvlInfoText;

    public Text[] goldText;
    public Text hearthsText;
    public Text healthText;
    public Text attackText;
    public Text[] gemText;
    public Text[] nameText;
    public Text inventoryItemCountText;

    public GameObject inventory;


    public Image experienceBar;
    public Image loadBar;
    public Text loadBarText;

    private void Start()
    {
        currentState = main;
        UpdateValues();        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitButton();
        }
    }

    public void UpdateValues()
    {
        exp = GameManager.gameManager.exp;
        maxExp = GameManager.gameManager.maxExp;

        expText.text = exp.ToString() + "/" + maxExp.ToString();
        experienceBar.fillAmount = exp / (float)maxExp;

        foreach (Text text in goldText)
        {
            text.text = GameManager.gameManager.gold.ToString();
        }
        foreach (Text text in gemText)
        {
            text.text = GameManager.gameManager.gems.ToString();
        }
        foreach (Text text in nameText)
        {
            text.text = GameManager.gameManager.playerName.ToString();
        }

        hearthsText.text = GameManager.gameManager.hearths.ToString();
        lvlText.text = GameManager.gameManager.lvl.ToString();
        lvlInfoText.text = "Lv. " + GameManager.gameManager.lvl.ToString();

        healthText.text = GameManager.gameManager.maxHealth.ToString();
        attackText.text = GameManager.gameManager.attack.ToString();

        int count = 0;
        foreach (Transform t in inventory.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("MenuItem")) count++;
        }
        inventoryItemCountText.text = count.ToString();
        loadBar.fillAmount = 0;
        loadBarText.text = "Loading....0%";
    }

    public void Stages()
    {
        currentState.SetActive(false);
        currentState = stages;
        currentState.SetActive(true);
    }

    public void PlayerInfo()
    {
        currentState = playerInfo;
        currentState.SetActive(true);
    }
    public void Mission()
    {
        currentState = missions;
        currentState.SetActive(true);
    }

    public void Item()
    {
        currentState = items;
        currentState.SetActive(true);
    }

    public void Shop()
    {
        currentState = shop;
        currentState.SetActive(true);
    }
    public void Ranking()
    {
        currentState = ranking;
        currentState.SetActive(true);
    }
    public void Settings()
    {
        currentState = settings;
        currentState.SetActive(true);
    }

    public void Main()
    {
        if(!currentState) currentState = main;
        else currentState.SetActive(false);
        currentState = main;
        currentState.SetActive(true);
    }

    public void LoadIslandGenerator()
    {
        currentState.SetActive(false);
        currentState = load;
        currentState.SetActive(true);
        StartCoroutine(LoadAsynchrnously(1));
    }

    IEnumerator LoadAsynchrnously(int level)
    {
        float loadProgess = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        while (!operation.isDone)
        {
            loadProgess = Mathf.Clamp01(operation.progress / 0.9f);
            loadBar.fillAmount = loadProgess;
            loadBarText.text = "Loading...." + ((int)(loadProgess * 100)).ToString() + "%";
            yield return null;
        }        
    }

    public void QuitButton()
    {            
        if(currentState == main)
        {
            currentState.SetActive(false);
            currentState = exit;
            currentState.SetActive(true);
        }else if(currentState != load)
        {
            currentState.SetActive(false);
            currentState = main;
            currentState.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
