using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerOld : MonoBehaviour
{

    public Text scoreText;
    public Text killsText;
    private int score;
    private int kills = 0;

    public Text timeText;
    [SerializeField] private float time;
    [SerializeField] bool timeRun;

    [SerializeField] private GameObject damageText;
    public enum bulletType { Bullet, Rocket, Liquid, Shotgun };
    public static GameManagerOld gameManager;

    public GameObject winningText;
    public GameObject losingText;

    public GameObject goalsParent;
    public Transform [] goals;
    public List<GameObject> goalsGameObject = new List<GameObject>();
    public GameObject finishGoal;

    public GameObject startingTransParent;
    public Transform[] startingTransforms;
    public Vector3 playerStartPos;

    private void Awake()
    {
        if (gameManager == null) gameManager = this;
        else if (gameManager != this) Destroy(this);
        goals = goalsParent.GetComponentsInChildren<Transform>();
        foreach(Transform t in goals)
        {
            if (t.CompareTag("Goal"))
            {
                goalsGameObject.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }            
        }
        int goal = Random.Range(0, goalsGameObject.Count);
        finishGoal = goalsGameObject[goal];
        finishGoal.SetActive(true);

        startingTransforms = startingTransParent.GetComponentsInChildren<Transform>();
        SetStartPosition();

    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        score = 0;
        UpdateScore();
        if (timeRun) StartCoroutine(UpdateTime());
        else timeText.enabled = false;
    }

    private void Update()
    {
        if (timeRun)
        {
            time -= Time.deltaTime;
            if (time <= -1) Player.player.GetComponent<Health>().LostGame();
        }
    }    

    public void AddScore()
    {
        score++;
        UpdateScore();
    }

    public void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        kills++;
        UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        kills = 0;
        UpdateScore();
    }

    public void Won()
    {
        GetComponent<GUIManager>().died = true;
        winningText.SetActive(true);
        Time.timeScale = 0;
    }

    public void Lost()
    {
        GetComponent<GUIManager>().died = true;
        losingText.SetActive(true);
        Time.timeScale = 0;
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

    void UpdateScore()
    {
        scoreText.text = score.ToString();
        killsText.text = kills.ToString();
    }

    void SetStartPosition()
    {
        int startInt = Random.Range(1, startingTransforms.Length);
        playerStartPos = startingTransforms[startInt].position;
        float distance = Vector3.Distance(playerStartPos, finishGoal.transform.position);
        time = distance * 1.33f > 999 ? 999: distance * 1.33f;
        if (distance < 125f) SetStartPosition();
    }

    public void NewGame()
    {        
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator UpdateTime()
    {
        while(time >= -1) {
            timeText.text = Mathf.Ceil(time).ToString();
            if (time < 10) timeText.color = Color.red;
            else timeText.color = Color.white;
            yield return new WaitForSeconds(.2f);
        }
    }
}
