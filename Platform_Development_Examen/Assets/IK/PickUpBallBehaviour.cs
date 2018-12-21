using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBallBehaviour : StateMachineBehaviour
{
    public Transform RightHandBallTarget;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandBallTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
    }
}
