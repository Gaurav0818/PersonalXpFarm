using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeAnimation : StateMachineBehaviour
{
    public PlayerController controller;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = FindObjectOfType<PlayerController>();
        controller.dodge = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.dodgeMode = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.dodgeMode = false;
    }
}
