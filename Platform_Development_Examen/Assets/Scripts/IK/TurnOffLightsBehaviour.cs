using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLightsBehaviour : StateMachineBehaviour
{
    public Transform LightSwitchTarget;

    private float _weightValueHand = 1.0f;

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.RightHand, LightSwitchTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weightValueHand);
	}
}
