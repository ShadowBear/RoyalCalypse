using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPosition : MonoBehaviour
{
    public bool setStartPos;

    private void Start()
    {
        if (setStartPos) Player.player.transform.position = GameManager.gameManager.playerStartPos;
    }
}
