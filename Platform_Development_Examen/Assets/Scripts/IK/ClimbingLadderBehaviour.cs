using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLadderBehaviour : StateMachineBehaviour
{
    public Transform LeftHandLadderTarget;
    public Transform RightHandLadderTarget;

    private float _weight = 1.0f;
    private float _weightValueHand = 0.1f;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("ClimbingLadder"), _weight);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandLadderTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _weightValueHand);

        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandLadderTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weightValueHand);
    }
}
