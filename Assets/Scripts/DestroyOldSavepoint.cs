using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOldSavepoint : MonoBehaviour {

    public GameObject savePoint1;
    public GameObject savePoint2;
    public GameObject savePoint3;
    // Use this for initialization
    void Start () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        Destroy(savePoint1);
        Destroy(gameObject);
        Destroy(savePoint2);
        Destroy(gameObject);
        Destroy(savePoint3);
        Destroy(gameObject);

    }
}
