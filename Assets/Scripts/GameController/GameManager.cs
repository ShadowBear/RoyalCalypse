using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Text scoreText;
    private int score;
    [SerializeField] private GameObject damageText;
    public enum bulletType { Bullet, Rocket, Liquid, Shotgun };
    public static GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null) gameManager = this;
        else if (gameManager != this) Destroy(this);
        score = 0;
        UpdateScore();
    }
       
    public void AddScore()
    {
        score++;
        UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScore();
    }

    public void ShowDamageText(int damage, Transform displayTrans)
    {
        GameObject dmgText = Instantiate(damageText);
        if (!GameObject.FindGameObjectWithTag("Canvas")) Debug.Log("Canvas nicht gefunden");
        dmgText.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        dmgText.GetComponentInChildren<Text>().text = damage.ToString();
        if (damage > 25) dmgText.GetComponentInChildren<Text>().color = Color.red;
        dmgText.gameObject.SetActive(true);

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(displayTrans.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 0f), 0));
        dmgText.transform.position = screenPosition;

    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
}
