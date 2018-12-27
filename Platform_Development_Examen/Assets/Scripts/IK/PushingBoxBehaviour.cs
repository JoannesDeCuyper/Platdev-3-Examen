using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingBoxBehaviour : StateMachineBehaviour {

    public Transform LeftHandBoxTarget;
    public Transform RightHandBoxTarget;
    public bool IsIK = false;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsIK)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandBoxTarget.position);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

            animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandBoxTarget.position);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }
    }
}
