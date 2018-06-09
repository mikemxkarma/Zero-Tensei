using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textriger : MonoBehaviour {



    public GameObject UInfoDisplay;

    private void Start()
    {
        UInfoDisplay.SetActive(false);

    }

    void OnTriggerEnter()
    {
        UInfoDisplay.SetActive(true);
    }
    void OnTriggerExit()
    {
        UInfoDisplay.SetActive(false);
        Destroy(UInfoDisplay);
        Destroy(gameObject);
    }
}

