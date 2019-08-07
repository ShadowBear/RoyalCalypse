using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChanger : MonoBehaviour
{

    int dungeonLevel;
    GUIManager guiManager;

    private void Start()
    {
        guiManager = GameManager.gameManager.GetComponent<GUIManager>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.gameManager.stageClear) guiManager.ExitIslandWinMenu();
            else guiManager.ExitIslandMenu();
        }        
    }

    //public void Update()
    //{
    //    if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
    //    {
    //        Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
    //        RaycastHit raycastHit;
    //        if (Physics.Raycast(raycast, out raycastHit))
    //        {
    //            //Debug.Log("Something Hit");

    //            if (raycastHit.collider.CompareTag("FlyBoat"))
    //            {
    //                //Debug.Log("Exit clicked");
    //                GameManager.gameManager.GetComponent<GUIManager>().MenuClicked();

    //            }
    //        }
    //    }
    //}

}
