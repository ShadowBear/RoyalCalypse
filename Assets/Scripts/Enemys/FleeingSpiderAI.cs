using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingSpiderAI : FleeingAI
{
    protected override void Animation()
    {
        if (agent.velocity != Vector3.zero) anim.SetBool("Move Forward Slow", true);
        else anim.SetBool("Move Forward Slow", false);
    }
}
