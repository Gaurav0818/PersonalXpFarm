using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpeedForAnimation : StateMachineBehaviour
{

     PlayerController controller;
    public float speed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = FindObjectOfType<PlayerController>();
        controller.speed = speed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
