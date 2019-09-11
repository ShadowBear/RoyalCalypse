using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{
    public Joystick movementJoystick;
    public Joystick rotationJoystick;

    public GameObject pauseMenu;
    public GameObject exitLoseMenu;
    public GameObject exitWinMenu;
    
    public Text killsText;
    public Text[] actualRaidGoldText;
    public Text[] actualRaidGemText;

    public GameObject diedMenu;

    private bool menuState = false;
    private bool exitState = false;
    private bool winState = false;

    public Image loadBar;
    public Text loadBarText;
    public GameObject loadBarScreen;

    public bool died = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) PauseMenuClicked();
    }

    public void UpdateScore()
    {
        foreach (Text text in actualRaidGoldText)
        {
            text.text = GameManager.gameManager.acutalRaidGold.ToString();
        }
        foreach (Text text in actualRaidGemText)
        {
            text.text = GameManager.gameManager.acutalRaidGem.ToString();
        }
        killsText.text = GameManager.gameManager.kills.ToString();
    }

    public void Start()
    {
        died = false;
        menuState = false;
        exitState = false;
        winState = false;
        loadBarScreen.SetActive(false);
        diedMenu.SetActive(false);
        pauseMenu.SetActive(menuState);
        exitLoseMenu.SetActive(exitState);
        exitWinMenu.SetActive(winState);
        UpdateScore();
        Time.timeScale = 1;
    }

    public void PauseMenuClicked()
    {
        if (died) return;
        menuState = menuState ? false : true;
        if (menuState) Time.timeScale = 0;
        else Time.timeScale = 1;
        pauseMenu.SetActive(menuState);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Debug.Log("Back to Main Menu");
    }

    public void ExitIslandMenu()
    {
        if (died) return;
        exitState = exitState ? false : true;
        if (exitState) Time.timeScale = 0;
        else Time.timeScale = 1;
        exitLoseMenu.SetActive(exitState);
    }

    public void ExitIslandWinMenu()
    {
        if (died) return;
        winState = winState ? false : true;
        if (winState) Time.timeScale = 0;
        else Time.timeScale = 1;
        exitWinMenu.SetActive(winState);
    }

    public void NextIsland()
    {
        GameManager.gameManager.stageDepth++;
        Time.timeScale = 0;        
        loadBarScreen.SetActive(true);
        StartCoroutine(LoadNextIsland());
    }   

    public void Lost()
    {
        died = true;
        GameManager.gameManager.acutalRaidGold = (int)(GameManager.gameManager.acutalRaidGold * 0.1f);
        GameManager.gameManager.acutalRaidGem = 0;

        UpdateScore();
        diedMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void CancelHome()
    {
        GameManager.gameManager.acutalRaidGold = (int)(GameManager.gameManager.acutalRaidGold * 0.1f);
        GameManager.gameManager.acutalRaidGem = 0;
        GameManager.gameManager.HomeCalled();
        loadBarScreen.SetActive(true);
        StartCoroutine(LoadHome());
    }

    IEnumerator LoadHome()
    {         
        float loadProgess = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        while (!operation.isDone)
        {
            loadProgess = Mathf.Clamp01(operation.progress / 0.9f);
            loadBar.fillAmount = loadProgess;
            loadBarText.text = "Loading...." + ((int)(loadProgess * 100)).ToString() + "%";
            yield return null;
        }        
    }
    IEnumerator LoadNextIsland()
    {
        float loadProgess = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        while (!operation.isDone)
        {
            loadProgess = Mathf.Clamp01(operation.progress / 0.9f);
            loadBar.fillAmount = loadProgess;
            loadBarText.text = "Loading...." + ((int)(loadProgess * 100)).ToString() + "%";
            yield return null;
        }
    }


    public void BackHome()
    {
        GameManager.gameManager.HomeCalled();
        loadBarScreen.SetActive(true);
        StartCoroutine(LoadHome());
    }


}
