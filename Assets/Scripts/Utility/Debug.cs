using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug : MonoBehaviour {

    Animator anim;
    [Range(0, 1)] public float vertical;
    [Range(-1, 1)] public float horizontal;

    public string animationName;
    public bool playAnimation;
    public bool lockon;
    public string[] oh_attacks;
    public string[] th_attacks;
    public bool useItem;
    public bool interacting;
    public bool twoHanded;
    public bool enableRootMotion;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        
        enableRootMotion = !anim.GetBool("canMove");
        anim.applyRootMotion = enableRootMotion;
        interacting = anim.GetBool("interacting");

        if (lockon == false)
        {
            horizontal = 0;
            vertical = Mathf.Clamp01(vertical);
        }
        anim.SetBool("lockOn", lockon);
        if (enableRootMotion)
            return;

        if (useItem)
        {
          
            anim.CrossFade("use_item",0.2f);
            useItem = false;
        }

        if (interacting)
        {
            playAnimation = false;
            vertical = Mathf.Clamp(vertical, 0, 0.5f);
        }
        anim.SetBool("twoHanded", twoHanded);

            if (playAnimation)
        {
            string targetAnim;

            if (!twoHanded)
            {
                int r = Random.Range(0, oh_attacks.Length);
                targetAnim = oh_attacks[r];

                if (vertical > 0.5f)
                    targetAnim = "oh_attack_3";
                 
            }
            else
            {
                int r = Random.Range(0, th_attacks.Length);
                targetAnim = th_attacks[r];

                if (vertical > 0.5f)
                    targetAnim = "oh_attack_3";
            }
            vertical = 0;     
            anim.CrossFade(targetAnim, 0.2f);
           // anim.SetBool("canMove",false);
           // enableRootMotion = true;
            playAnimation = false;
        }
        anim.SetFloat("vertical", vertical);
        anim.SetFloat("horizontal", horizontal);

    }
}
