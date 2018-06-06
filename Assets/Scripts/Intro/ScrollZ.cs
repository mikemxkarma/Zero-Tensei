﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollZ : MonoBehaviour {

    public float scrollSpeed = 20;
    Vector3 pos;
    Vector3 localVectorUp;
	// Update is called once per frame
	void Update () {
        pos = transform.position;

        localVectorUp = transform.TransformDirection(0, 1, 0);

        pos += localVectorUp * scrollSpeed * Time.deltaTime;

        transform.position = pos;

	}
}