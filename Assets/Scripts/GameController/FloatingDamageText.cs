using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour
{
    //public Animator animator;
    //public Text damageText;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
    //    Destroy(gameObject, clipInfos[0].clip.length);
    //    damageText = animator.GetComponent<Text>();
    //}

    //public void SetText(string text)
    //{
    //    damageText.text = text;
    //}

    private float lifeTime = 0.5f;
    private float scaleRate = 0.005f;
    private Vector3 scaleVector;

    private Transform parentTrans;

    // Use this for initialization
    void Start()
    {
        scaleVector = new Vector3(scaleRate, scaleRate, scaleRate);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) Destroy(transform.parent.gameObject);
        else
        {
            if(Time.timeScale > 0) transform.localScale += scaleVector;
        }
    }
}
