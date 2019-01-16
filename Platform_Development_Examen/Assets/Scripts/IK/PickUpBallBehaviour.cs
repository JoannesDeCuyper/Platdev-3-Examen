﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBallBehaviour : StateMachineBehaviour
{
    public Transform RightHandBallTarget;
    public AnimationClip PickingUpClip;
    public float PickingUpTimer;
    public bool IsPickingUpBall;
    public bool IsThrowIdle;

    private float _weightValueHand = 1.0f;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandBallTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weightValueHand);

        if (IsPickingUpBall)
        {
            PickingUpTimer -= Time.deltaTime;
            if (PickingUpTimer <= 0.0f)
            {
                PickingUpTimer = 0;
                animator.SetLayerWeight(animator.GetLayerIndex("Ball"), 0);
            }
        }
        if(IsThrowIdle)
            animator.SetLayerWeight(animator.GetLayerIndex("Ball"), 1);
    }
}
