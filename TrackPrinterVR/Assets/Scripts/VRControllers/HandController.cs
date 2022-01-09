using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    private ActionBasedController _controller;

    [SerializeField] private Hand hand;

    private void Start()
    {
        _controller = GetComponent<ActionBasedController>();
        hand ??= GetComponentInChildren<Hand>();
    }

    private void Update()
    {
        hand.SetGrip(_controller.selectAction.action.ReadValue<float>());
        hand.SetTrigger(_controller.activateAction.action.ReadValue<float>());
    }
}
