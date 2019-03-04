using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    protected override void Die()
    {
        GameManager.gameManager.ResetScore();
        Start();
    }
}
