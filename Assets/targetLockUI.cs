using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControll;

public class targetLockUI : MonoBehaviour {

    public CameraManager camera;
    public CanvasRenderer trans;

    // Use this for initialization
    void Start()
    {
        trans.SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (camera.lockOnMode){
            transform.position = Camera.main.WorldToScreenPoint(camera.lockEffectTarget.position);
            trans.SetAlpha(1);
        }
        else
        {
            trans.SetAlpha(0);
        }
    }
}
