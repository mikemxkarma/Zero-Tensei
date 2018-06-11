using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playvideo : MonoBehaviour
{

    public bool started = false;


    public class Example : MonoBehaviour
    {
        void Start()
        {
            Handheld.PlayFullScreenMovie("SplashScreen.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        }
    }
}