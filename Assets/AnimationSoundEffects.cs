using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEffects : MonoBehaviour
{

    [SerializeField]
    private AudioClip footstep;
    [SerializeField]
    private AudioClip dash;
    [SerializeField]
    private AudioClip swordSwing;
    [SerializeField]
    private AudioClip parried;
    [SerializeField]
    private AudioClip damaged;



    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        audioSource.PlayOneShot(footstep);
    }

    private void Dash()
    {
        audioSource.PlayOneShot(dash);
    }

    private void SwordSwing()
    {
        audioSource.PlayOneShot(swordSwing);
    }

    private void Parried()
    {
        audioSource.PlayOneShot(parried);
    }

    private void Damaged()
    {
        audioSource.PlayOneShot(damaged);
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

