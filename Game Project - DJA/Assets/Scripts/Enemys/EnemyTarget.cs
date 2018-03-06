using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameControll
{
    public class EnemyTarget : MonoBehaviour
    {
        public int index;
        public List<Transform> targets = new List<Transform>();
        public List<HumanBodyBones> humanoidBones = new List<HumanBodyBones>();
        Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();
            if (anim.isHuman == false)
                return;
            for (int i = 0; i < humanoidBones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(humanoidBones[i]));
            }

        }


        public Transform GetTarget(bool negative = false)
        {
            if (targets.Count == 0)
                return transform;          

            if (negative == false)
            {
                if (index < targets.Count - 1)
                    index++;
                else
                    index = 0;                                
            }
            else
            {
                if (index <= 0)
                    index = targets.Count - 1;
                else
                    index--;
            }

            index = Mathf.Clamp(index, 0, targets.Count);
            return targets[index];//return target from above33
        }         
    }
}

