using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameControll
{
    public class Projectile : MonoBehaviour
    {
        Rigidbody rigid;

        public float hSpeed = 5;
        public float vSpeed = 2;

        public Transform target;

        public GameObject explosionPrefab;

        public void Init()
        {
            rigid = GetComponent<Rigidbody>();

            Vector3 targetForce = transform.forward * hSpeed;
            targetForce += transform.up * vSpeed;
            rigid.AddForce(targetForce, ForceMode.Impulse);

        }
        private void OnTriggerEnter(Collider other)
        {
            EnemyStates es = other.GetComponentInParent<EnemyStates>();
            if(es != null)
            {
                es.health -= 40;
            }
            GameObject go = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
            Destroy(this.gameObject);
        }
    }
}

