using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour

{
    public Button playBtn;
    public Button optionsBtn;
    public Button exitBtn;
    public Button controlsBtn;
    public Button aboutGameBtn;

    void Start () {
        playBtn.onClick.AddListener(() => { SceneManager.LoadScene(4); });
        optionsBtn.onClick.AddListener(() => {  SceneManager.LoadScene(2); });
        exitBtn.onClick.AddListener(() => {  Application.Quit(); });
        controlsBtn.onClick.AddListener(() => { SceneManager.LoadScene(2); });
        aboutGameBtn.onClick.AddListener(() => { SceneManager.LoadScene(3); });



    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
