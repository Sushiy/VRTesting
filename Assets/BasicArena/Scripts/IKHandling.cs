using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandling : MonoBehaviour
{

    Animator avatar;

    public float leftIKWeight = 1;
    public float rightIKWeight = 1;

    // insert Object to target IKs
    public Transform leftIKTarget;
    public Transform rightIKTarget;



    void Start ()
    {

       avatar = GetComponent<Animator>();
    }
	

    void OnAnimatorIK()
    {
        if (avatar)
        {
            // Setting Weights of IK Limbs
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftIKWeight);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftIKWeight);

            avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, rightIKWeight);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, rightIKWeight);

            // Setting target for IK Limbs
            avatar.SetIKPosition(AvatarIKGoal.LeftHand, leftIKTarget.position);
            avatar.SetIKRotation(AvatarIKGoal.LeftHand, leftIKTarget.rotation);

            avatar.SetIKPosition(AvatarIKGoal.RightHand, rightIKTarget.position);        
            avatar.SetIKRotation(AvatarIKGoal.RightHand, rightIKTarget.rotation);
        }
    }
}
