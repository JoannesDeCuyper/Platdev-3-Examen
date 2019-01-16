using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingBoxBehaviour : StateMachineBehaviour {

    public Transform LeftHandBoxTarget;
    public Transform RightHandBoxTarget;

    private float _weight = 1.0f;
    private float _weightValueHand = 1.0f;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Pushing"), _weight);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandBoxTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _weightValueHand);

        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandBoxTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weightValueHand);
    }
}
