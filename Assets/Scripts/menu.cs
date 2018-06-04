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
        playBtn.onClick.AddListener(() => { SceneManager.LoadScene("Stage1"); });
        optionsBtn.onClick.AddListener(() => {  SceneManager.LoadScene("Controls"); });
        exitBtn.onClick.AddListener(() => {  Application.Quit(); });
        controlsBtn.onClick.AddListener(() => { SceneManager.LoadScene("Controls"); });
        aboutGameBtn.onClick.AddListener(() => { SceneManager.LoadScene("AboutGame"); });



    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
