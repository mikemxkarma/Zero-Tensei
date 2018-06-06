using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class ChangeScreen : MonoBehaviour {
    public int scene;
    VideoPlayer player;
    bool hasPlayed;
	// Use this for initialization
	void Start () {
        player = this.GetComponent<VideoPlayer>();
        hasPlayed = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (player.isPlaying)
        {
            hasPlayed = true;
        }
        if (!player.isPlaying && hasPlayed == true)
        {
            SceneManager.LoadScene(scene);
        }
	}
}
