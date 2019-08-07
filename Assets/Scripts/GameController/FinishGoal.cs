using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGoal : CollectItemTemplate
{
    public LayerMask enemyLayer;
    [SerializeField] float radius;
    private bool checking = false;
    private bool noEnemyInRange = false;
    private SpriteRenderer miniMapIcon;
    private Color iconColor;
    [SerializeField] private GameObject enemyText;

    protected override void Start()
    {
        base.Start();
        miniMapIcon = GetComponentInChildren<SpriteRenderer>();
        iconColor = miniMapIcon.color;
        enemyText.SetActive(false);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!checking) StartCoroutine(CheckRadius());
            if (noEnemyInRange)
            {
                base.OnTriggerStay(other);
                miniMapIcon.color = iconColor;
                enemyText.SetActive(false);
            }
            else
            {
                image.fillAmount = 0;
                miniMapIcon.color = Color.red;
                enemyText.SetActive(true);
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        enemyText.SetActive(false);
    }

    protected override void CollectIt(Collider player)
    {
        //GameManager.gameManager.Won();
    }

    IEnumerator CheckRadius()
    {
        checking = true;
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        if (targets.Length == 0) noEnemyInRange = true;
        else noEnemyInRange = false;
        yield return new WaitForSeconds(0.33f);
        checking = false;
    }
    
}
