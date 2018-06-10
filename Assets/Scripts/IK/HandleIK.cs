using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class HandleIK : MonoBehaviour
    {
        Animator anim;

        Transform handHelper;
        Transform bodyHelper;
        //Transform headHelper;
        Transform shoulderHelper;
        Transform animShoulder;
        //Transform headTrans;

        public float weight;

        public IKSnapShot[] ikSnapshots;
        public Vector3 defaultHeadPos;

        IKSnapShot GetSnapshot(IKSnapshotType type)
        {
            for (int i = 0; i < ikSnapshots.Length; i++)
            {
                if (ikSnapshots[i].type == type)
                {
                    return ikSnapshots[i];
                }
            }

            return null;
        }


        public void Init(Animator a)
        {
            anim = a;
        
            //headHelper = new GameObject().transform;
            //headHelper.name = "head helper";
            handHelper = new GameObject().transform;
            handHelper.name = "hand helper";
            bodyHelper = new GameObject().transform;
            bodyHelper.name = "body helper";
            shoulderHelper = new GameObject().transform;
            shoulderHelper.name = "shoulder helper";

            shoulderHelper.parent = transform.parent;
            shoulderHelper.localPosition = Vector3.zero;
            shoulderHelper.localRotation = Quaternion.identity;
            //headHelper.parent = shoulderHelper;
            bodyHelper.parent = shoulderHelper;
            handHelper.parent = shoulderHelper;

            //headTrans = anim.GetBoneTransform(HumanBodyBones.Head);
        }

        public void UpdateIKTargets(IKSnapshotType type, bool isLeft)
        {
            IKSnapShot snap = GetSnapshot(type);

            Vector3 targetBodyPos = snap.bodyPos;
            if (isLeft)
                targetBodyPos.x = -targetBodyPos.x;
            bodyHelper.localPosition = targetBodyPos;

            handHelper.localPosition = snap.handPos;
            handHelper.localEulerAngles = snap.hand_eulers;
            /*
            if (snap.overwriteHeadPos)
                headHelper.localPosition = snap.headPos;
            else
                headHelper.localPosition = defaultHeadPos;
                */
        }

        public void OnAnimatorMoveTick(bool isLeft)
        {
            Transform shoulder = anim.GetBoneTransform(
                (isLeft) ? HumanBodyBones.LeftShoulder : HumanBodyBones.RightShoulder);

            shoulderHelper.transform.position = shoulder.position;
        }

        public void IKTick(AvatarIKGoal goal, float w)
        {
            weight = Mathf.Lerp(weight, w, Time.deltaTime * 5);

            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKRotationWeight(goal, weight);

            anim.SetIKPosition(goal, handHelper.position);
            anim.SetIKRotation(goal, handHelper.rotation);

            anim.SetLookAtWeight(weight, 0.8f, 1, 1, 1);
            anim.SetLookAtPosition(bodyHelper.position);
        }

        public void LateTick()
        {
            /*
            if (headTrans == null || headHelper == null)
                return;

            Vector3 direction = headHelper.position - headTrans.position;
            if (direction == Vector3.zero)
                direction = headTrans.forward;

            Quaternion targetRot = Quaternion.LookRotation(direction);
            Quaternion curRot = Quaternion.Slerp(headTrans.rotation, targetRot, weight);
            headTrans.rotation = curRot;
            */
        }

    }

    public enum IKSnapshotType
    {
        breath, shield_r, shield_l
    }

    [System.Serializable]
    public class IKSnapShot
    {
        public IKSnapshotType type;
        public Vector3 handPos;
        public Vector3 hand_eulers;
        public Vector3 bodyPos;
        public bool overwriteHeadPos;
        public Vector3 headPos;


    }
}

