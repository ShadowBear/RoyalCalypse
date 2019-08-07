﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeingAI : MonoBehaviour
{
    // Start is called before the first frame update

    public enum State {Hide, Flee };
    [SerializeField] State currentState;
    private bool runningAway = false;
    private bool hiding = false;
    private NavMeshAgent agent;
    [SerializeField] float fleeRadius = 8f;
    private Animator anim;
    private bool alive = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = State.Flee;
        RunAway();
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;
        if (currentState == State.Flee)
        {
            if (!runningAway) RunAway();
            if (agent.remainingDistance <= 0.5f)
            {
                currentState = State.Hide;
            }
        }
        else if(currentState == State.Hide)
        {
            if(!hiding) Hide();
        }
        Animation();
    }

    void RunAway()
    {
        runningAway = true;
        hiding = false;
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * fleeRadius;
        //NavMeshHit hit;
        //NavMesh.SamplePosition(randomPoint, out hit, fleeRadius, 1);
        //Vector3 finalPosition = hit.position;
        randomPoint = new Vector3(randomPoint.x, transform.position.y, randomPoint.z);
        agent.SetDestination(randomPoint);
        agent.isStopped = false;
    }

    void Hide()
    {
        hiding = true;
        runningAway = false;
        agent.isStopped = true;
    }

    public void SetState(State s)
    {
        currentState = s;
    }

    public void SetAliveState(bool state)
    {
        alive = state;
    }

    protected virtual void Animation()
    {
        if (agent.velocity != Vector3.zero) anim.SetFloat("Speed", 1);
        else anim.SetFloat("Speed", 0);
    }

}