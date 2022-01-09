using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private float animationSpeed = 10;
    public float AnimationSpeed => animationSpeed;

    private int _gripParameterID;
    private int _triggerParameterID;
    private SkinnedMeshRenderer mesh;

    public float gripTarget { get; private set; }
    public float triggerTarget { get; private set; }
    public float gripCurrent { get; private set; }
    public float triggerCurrent { get; private set; }


    private void Awake()
    {
        _gripParameterID = Animator.StringToHash("Grip");
        _triggerParameterID = Animator.StringToHash("Trigger");
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void FixedUpdate()
    {
        AnimateHand();
    }

    private void AnimateHand()
    {
        if (Mathf.Abs(gripCurrent - gripTarget) > 0.01f)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * animationSpeed);
            _animator.SetFloat(_gripParameterID, gripCurrent);
        }
        if (Mathf.Abs(triggerCurrent - triggerTarget) > 0.01f)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * animationSpeed);
            _animator.SetFloat(_triggerParameterID, triggerCurrent);
        }
    }

    public void SetVisible()
    {
        mesh.enabled = true;
    }
    
    public void SetInvisible()
    {
        mesh.enabled = false;
    }
    
    public void ToggleVisibility()
    {
        mesh.enabled = !mesh.enabled;
    }
    

    public void SetGrip(float value)
    {
        gripTarget = value;
    }


    public void SetTrigger(float value)
    {
        triggerTarget = value;
    }

}
