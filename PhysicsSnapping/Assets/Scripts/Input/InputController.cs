using System;
using Moving;
using Snapping;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputController : MonoBehaviour
    {
        private MoveObjectController _moveObjectController;

        private Vector2 _movementInput = Vector2.zero;

        private void Awake()
        {
            _moveObjectController = FindObjectOfType<MoveObjectController>();
        }

        private void FixedUpdate()
        {
            if (!_movementInput.Equals(Vector2.zero))
            {
                _moveObjectController.MoveSelection(new Vector3(_movementInput.x, 0, _movementInput.y),
                    Time.fixedDeltaTime);
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (Camera.main is null) return;

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            SnappingObjWrapper otherObjWrapper = null;
            if (Physics.Raycast(ray, out var hit))
            {
                var go = hit.transform.gameObject;
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