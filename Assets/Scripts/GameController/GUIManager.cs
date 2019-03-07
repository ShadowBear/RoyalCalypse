using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text distanceToGoalText;
    public GameObject menu;
    private bool menuState = false;

    private Vector3 playerPos;
    private Vector3 finishPos;

    private void Start()
    {
        playerPos = Player.player.transform.position;
        finishPos = GameManager.gameManager.finishGoal.GetComponentInChildren<FinishGoal>().transform.position;
        StartCoroutine(UpdateDistance());
        menu.SetActive(menuState);
    }

    public void MenuClicked()
    {
        menuState = menuState ? false : true;
        if (menuState) Time.timeScale = 0;
        else Time.timeScale = 1;
        menu.SetActive(menuState);
    }

    IEnumerator UpdateDistance()
    {
        int distance = 0;
        while (true)
        {
            playerPos = Player.player.transform.position;
            distance = (int) Vector3.Distance(playerPos, finishPos);
            distanceToGoalText.text = "Distance: " + distance.ToString();
            yield return new WaitForSeconds(0.2f);
        }
    }



}
