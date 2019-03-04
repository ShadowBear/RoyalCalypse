using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    protected GameObject player;
    protected NavMeshAgent navMeshAgent;
    protected bool canAttack;
    protected Animator anim;
    [SerializeField] protected float followDistance;
    [SerializeField] protected float meleeAttackDistance;
    public bool alive = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        player = Player.player.gameObject;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        canAttack = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!alive)
        {
            navMeshAgent.isStopped = true;
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < followDistance)
        {
            //Simple Rotation
            //transform.LookAt(player.transform.position);

            //Smoother Rotation
            RotateToPlayer();

            if (distanceToPlayer > meleeAttackDistance && canAttack)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.destination = player.transform.position;
            }
            else
            {
                navMeshAgent.isStopped = true;
                if (canAttack) StartCoroutine(MeleeAttack());
            }
            Animation();
        }else
        {
            anim.SetFloat("Speed", 0);
            navMeshAgent.isStopped = true;
        }
        
        
        
    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(2f);
        canAttack = true;
        yield return null;
    }

    void RotateToPlayer()
    {
        //Smoother Rotation
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    protected virtual void Animation()
    {
        if (navMeshAgent.velocity != Vector3.zero) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
    }

    public virtual void SetHitbox(int activater)
    {
        if (GetComponentInChildren<SwordTrigger>()) GetComponentInChildren<SwordTrigger>().SetHitbox(activater);
    }
}
