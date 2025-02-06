 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSCript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator navTarget;

    

    public void ButtonPress(){
        animator.SetBool("ButtonPress",true);
        animator.SetBool("pullDownPressed", false);
    }
    public void DropDownButtonPress(){
        animator.SetBool("pullDownPressed",true);
        animator.SetBool("NavigationTargetSelected",true);
        animator.SetBool("ButtonPress", false);
    }

    public void navigationTargetSelected(){
        animator.SetBool("NavigationTargetSelected",true);
        animator.SetBool("ButtonPress",false);
        animator.SetBool("pullDownPressed",true);
        navTarget.SetBool("locationPressed",true);

    }
    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("ButtonPress",false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ButtonPress: " + animator.GetBool("ButtonPress"));
    }
}
