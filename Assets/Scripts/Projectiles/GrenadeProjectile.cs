using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameControll
{
    public class GrenadeProjectile : MonoBehaviour
    {
        Rigidbody rigid;

        public float hSpeed = 5;
        public float vSpeed = 2;

        public Transform target;

        public void Init(Vector3 targetDir)
        {
            rigid = GetComponent<Rigidbody>();

            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = targetRot;

            Vector3 targetForce = transform.forward * hSpeed;
            targetForce += transform.up * vSpeed;
            rigid.AddForce(targetForce, ForceMode.Impulse);

        }
        private void Start()
        {
           
        }
    }
}

