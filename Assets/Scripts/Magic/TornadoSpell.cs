using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSpell : MonoBehaviour
{
    public float destroyTime = 5f;
    public float followSpeed = 2.5f;
    public float circleSpeed = 50f;

    public bool circle = false;
    public bool follow = false;
    public bool delay = false;

    private Transform playerTrans;
    [SerializeField] Transform circleTrans;
    private float dmgCounter = 0f;
    public int damage;

    public void Start()
    {
        Destroy(gameObject, destroyTime);
        playerTrans = Player.player.transform;
    }

    private void Update()
    {
        if (follow) FollowAttack(playerTrans.position);
        else if (circle) CircleTornado(circleTrans.position);
        else if (delay) DelayedFollow(playerTrans.position);
    }

    private void CircleTornado(Vector3 middlePoint)
    {
        transform.RotateAround(middlePoint, Vector3.up, circleSpeed * Time.deltaTime);
    }

    private void FollowAttack(Vector3 target)
    {
        // Move our position a step closer to the target.
        float step = followSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    private void DelayedFollow(Vector3 target)
    {

    }

    public void SetCircleTornado(Transform middleTrans)
    {
        follow = false;
        circle = true;
        delay = false;
    }

    public void SetDelayTornado()
    {
        follow = false;
        circle = false;
        delay = true;
    }

    public void SetForwardTornado()
    {
        follow = true;
        circle = false;
        delay = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TornadoHit");
            dmgCounter -= Time.deltaTime;
            if (dmgCounter <= 0)
            {
                dmgCounter = 0.33f;
                other.GetComponent<Health>().Damage(damage/3);
            }
        }
    }

}
