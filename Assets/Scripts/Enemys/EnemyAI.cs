using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
    protected GameObject player;

    [SerializeField] protected LayerMask alliesMask;
    [SerializeField] protected float helpRadius;

    protected NavMeshAgent navMeshAgent;

    [SerializeField] GameObject patrolPointParent;
    [SerializeField]protected Transform[] patrolPoints;
    protected int currentPatrolPoint;
    protected int lastPatrolPoint;

    [SerializeField] protected bool NonPatrolGuard;
    [SerializeField] private Vector3 NonPatrolGuardingPoint;
    private Quaternion startRotation;

    [SerializeField] protected float guardingTime;
    [SerializeField] protected float maxGuardingTime;
    [SerializeField] protected float minGuardingTime;

    protected bool followUpdater = true;
    protected Vector3 lastSeenPosition;

    protected Vector3 helpNeededPosition;

    protected bool canAttack;
    protected Animator anim;
    protected FieldOfView fieldOfView;
    //[SerializeField] protected float followDistance;
    
    public bool alive = true;
    private bool lookingAround = false;
    Quaternion rotation = Quaternion.Euler(0, 0, 0);

    public enum EnemyState { Patrol, Guard, Search, Fight, Attack, Follow, SummonHelp, Rescue, Run}
    [SerializeField] protected EnemyState currentState;
    //[SerializeField] private EnemyState previousState;
    //[SerializeField] private EnemyState lastState;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        player = Player.player.gameObject;
        navMeshAgent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();
        fieldOfView = GetComponent<FieldOfView>();
        if (!NonPatrolGuard && patrolPointParent)
        {
            patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>();
            if (navMeshAgent.destination != patrolPoints[0].position)
            {
                //Debug.Log("InitDestination");
                currentPatrolPoint = 0;
                lastPatrolPoint = 0;
                navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);
            }
        }
        else
        {
            NonPatrolGuard = true;
            startRotation = transform.rotation;
            navMeshAgent.SetDestination(transform.position);
        }
        NonPatrolGuardingPoint = transform.position;
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

        switch(currentState){
            case EnemyState.Attack: Attack();
                break;
            case EnemyState.Fight: Fight();
                break;
            case EnemyState.Follow: Follow();
                break;
            case EnemyState.Guard: Guard();
                break;
            case EnemyState.Patrol: Patrol();
                break;
            case EnemyState.Rescue: Rescue();
                break;
            case EnemyState.Run: Run();
                break;
            case EnemyState.Search: Search();
                break;
            case EnemyState.SummonHelp: SummonHelp();
                break;
            default: Guard();
                break;
        }

        if((currentState == EnemyState.Guard || currentState == EnemyState.Patrol
            || currentState == EnemyState.Search || currentState == EnemyState.Rescue) && fieldOfView.GetVisible())
        {
            currentState = EnemyState.Fight;
        }
        Animation();    
    }

    protected virtual void Patrol()
    {
        navMeshAgent.isStopped = false;
        if (!NonPatrolGuard && patrolPointParent != null)
        {
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                currentState = EnemyState.Guard;
                guardingTime = Random.Range(minGuardingTime, maxGuardingTime);
                //Debug.Log("GuardingTime: " + guardingTime);
                lastPatrolPoint = currentPatrolPoint;
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);
            }
        }
        else
        {
            if(navMeshAgent.destination != NonPatrolGuardingPoint) navMeshAgent.SetDestination(NonPatrolGuardingPoint);
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                currentState = EnemyState.Guard;
            }
        }      
        
    }

    protected virtual void Guard()
    {
        navMeshAgent.isStopped = true;

        if (!NonPatrolGuard)
        {
            if (currentState == EnemyState.Guard) guardingTime -= Time.deltaTime;
            if (guardingTime <= 0) currentState = EnemyState.Patrol;
            //transform.rotation = Quaternion.Slerp(transform.rotation, patrolPoints[currentPatrolPoint].rotation, Time.deltaTime * 5f);
        }
        else
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 5f);
            
        }
        if (!lookingAround) StartCoroutine(LookAroundOnGuarding());
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * .5f);
    }

    IEnumerator LookAroundOnGuarding()
    {
        lookingAround = true;
        float look = Random.Range(0f, 360f);
        rotation = Quaternion.Euler(transform.rotation.x, look, transform.rotation.z);
        float waitingTime = Random.Range(5f, 12f);
        yield return new WaitForSeconds(waitingTime);
        lookingAround = false;
        yield return null;

    }

    protected abstract void Fight();

    protected abstract void Attack();

    protected abstract void Follow();

    protected virtual void Search()
    {
        if(navMeshAgent.destination != lastSeenPosition) navMeshAgent.SetDestination(lastSeenPosition);
        navMeshAgent.isStopped = false;
        if (navMeshAgent.remainingDistance < 0.5f) currentState = LookAround();
    }

    protected virtual void SummonHelp()
    {
        //Debug.Log("Bitte um Hilfe");
        Collider[] friendsInViewRadius = Physics.OverlapSphere(transform.position, helpRadius, alliesMask);
        for (int i = 0; i < friendsInViewRadius.Length; i++)
        {
            if (friendsInViewRadius[i].transform.GetComponent<EnemyAI>())
            {
                friendsInViewRadius[i].transform.GetComponent<EnemyAI>().HelpMe(player.transform.position);
            }
        }
    }

    protected virtual void Rescue()
    {
        if (navMeshAgent.destination != helpNeededPosition) navMeshAgent.SetDestination(helpNeededPosition);
        navMeshAgent.isStopped = false;
        if (navMeshAgent.remainingDistance < 0.5f)
        {
            currentState = LookAround();
        }
    }

    protected virtual void Run()
    {
        //Later Todo
    }

    protected EnemyState LookAround()
    {
        Vector3 dirToTarget = (player.transform.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position, dirToTarget, fieldOfView.viewRadius, fieldOfView.obstacleMask))
        {
            return EnemyState.Follow;
        }
        else
        {
            if(!NonPatrolGuard) navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);
            else navMeshAgent.SetDestination(NonPatrolGuardingPoint);
            return EnemyState.Patrol;
        }

    }

    public void HelpMe(Vector3 helpPosition)
    {
        helpNeededPosition = helpPosition;
        //currentState = EnemyState.Rescue;
        if(currentState == EnemyState.Guard || currentState == EnemyState.Patrol) currentState = EnemyState.Fight;
        //Debug.Log("Hilfe angekommen!");
    }
    

    protected IEnumerator UpdatePlayerPos()
    {
        followUpdater = false;
        navMeshAgent.SetDestination(player.transform.position);
        yield return new WaitForSeconds(0.2f);
        followUpdater = true;
    }

    protected void RotateToPlayer()
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
        if (GetComponentInChildren<MeleeHitboxTrigger>()) GetComponentInChildren<MeleeHitboxTrigger>().SetHitbox(activater);
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }

    public void SetCurrentState(EnemyState state)
    {
        currentState = state;
    }

}
