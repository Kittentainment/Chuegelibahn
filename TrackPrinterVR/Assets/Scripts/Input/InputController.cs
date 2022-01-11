using System;
using Moving;
using Snapping;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Input
{
    public class InputController : MonoBehaviour
    {
        
        public static InputController Instance { get; private set; }
        
        private MoveObjectController _moveObjectController;

        private Vector3 _movementInput = Vector3.zero;

        [SerializeField] private XRRayInteractor xrRayInteractor;
        public XRRayInteractor RightHand => xrRayInteractor;

        private void Awake()
        {
            if (Instance != null) Debug.LogError("Only One InputController allowed!");
            Instance = this;
            _moveObjectController = FindObjectOfType<MoveObjectController>();
        }

        private void FixedUpdate()
        {
            if (!_movementInput.Equals(Vector3.zero))
            {
                _moveObjectController.MoveSelection(new Vector3(_movementInput.x, _movementInput.y, 0),
                    Time.fixedDeltaTime);
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Debug.Log("InputController::OnCLick");
            if (!context.performed) return;

            if (Camera.main is null) return;

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            SnappingObjWrapper otherObjWrapper = null;
            if (Physics.Raycast(ray, out var hit))
            {
                var go = hit.transform.gameObject;
                if (go != null)
                {
                    Debug.Log($"Hit a go: {go.name}");
                }
                var snappingObj = go.GetComponentInParent<SnappingObjWrapper>();
                if (snappingObj != null)
                {
                    otherObjWrapper = snappingObj;
                }
            }

            if (otherObjWrapper is null)
            {
                _moveObjectController.DeselectObject();
            }
            else
            {
                _moveObjectController.SelectAnObject(otherObjWrapper);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _movementInput = context.ReadValue<Vector2>();
            }

            if (context.canceled)
            {
                _movementInput = Vector2.zero;
            }
        }
    }
}