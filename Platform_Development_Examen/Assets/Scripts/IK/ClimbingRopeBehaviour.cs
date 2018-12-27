using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingRopeBehaviour : StateMachineBehaviour
{

    public Transform LeftHandRopeTarget;
    public Transform RightHandRopeTarget;

    public Transform LeftFootRopeTarget;
    public Transform RightFootRopeTarget;

    private float _weightValueHand = 0.1f;
    private float _weightValueFoot = 0.1f;

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Hand
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandRopeTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _weightValueHand);

        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandRopeTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weightValueHand);

        //Foot
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootRopeTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _weightValueFoot);

        animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootRopeTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _weightValueFoot);
    } 
}
