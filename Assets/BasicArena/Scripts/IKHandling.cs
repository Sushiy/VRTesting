using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandling : MonoBehaviour {

    Animator avatar;

    public float leftIKWeight = 1;
    public float rightIKWeight = 1;


    public Transform leftIKTarget;
    public Transform rightIKTarget;



    // Use this for initialization
    void Start () {

       avatar = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnAnimatorIK()
    {
        if (avatar)
        {
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftIKWeight);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, rightIKWeight);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftIKWeight);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, rightIKWeight);

            avatar.SetIKPosition(AvatarIKGoal.LeftHand, leftIKTarget.position);
            avatar.SetIKPosition(AvatarIKGoal.RightHand, rightIKTarget.position);

            avatar.SetIKRotation(AvatarIKGoal.LeftHand, leftIKTarget.rotation);
            avatar.SetIKRotation(AvatarIKGoal.RightHand, rightIKTarget.rotation);
        }
    }
}
