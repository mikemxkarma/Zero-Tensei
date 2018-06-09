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
    public Button backBtn;
    public GameObject controlsBtnReveal;
    public GameObject aboutBtnReveal;

    void Start () {

        controlsBtnReveal.gameObject.SetActive(false);
        aboutBtnReveal.gameObject.SetActive(false);
        playBtn.onClick.AddListener(() => { SceneManager.LoadScene(4); });
        optionsBtn.onClick.AddListener(() => {controlsBtnReveal.gameObject.SetActive(true); aboutBtnReveal.gameObject.SetActive(true); });
        exitBtn.onClick.AddListener(() => {  Application.Quit(); });
        controlsBtn.onClick.AddListener(() => { SceneManager.LoadScene(3); });
        aboutGameBtn.onClick.AddListener(() => { SceneManager.LoadScene(2); });
        backBtn.onClick.AddListener(() => { SceneManager.LoadScene(1); });
    }
	
	void Update () {          
	}
}
