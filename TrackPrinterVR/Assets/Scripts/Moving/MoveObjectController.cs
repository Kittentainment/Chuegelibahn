using System;
using JetBrains.Annotations;
using Snapping;
using UnityEngine;

namespace Moving
{
    public class MoveObjectController : MonoBehaviour
    {
        
        public static MoveObjectController Instance { get; private set; }
        
        [SerializeField] private float movementSpeed = 1;

        [CanBeNull] private SnappingObjWrapper _selection;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("MoveObjectController already exists");
                Destroy(Instance.gameObject);
            }
            Instance = this;
        }

        public void SelectAnObject([NotNull] SnappingObjWrapper objWrapper)
        {
            Debug.Log("MoveObjectController::SelectAnObject");
            DeselectObject();
            _selection = objWrapper ? objWrapper : throw new ArgumentNullException(nameof(objWrapper));
            _selection.MovementHasStarted();
            Debug.Log($"selected an object: {_selection.gameObject.name}");
        }

        public void DeselectObject()
        {
            if (_selection != null) _selection.LetGoAndSnap();
            _selection = null;
        }

        public void MoveSelection(Vector3 movement, float deltaTime)
        {
            // Debug.Log($"MoveObjectController::MoveSelection({movement}, {deltaTime})");
            if (_selection != null)
            {
                _selection.transform.Translate(movement * deltaTime * movementSpeed, Space.World);
            }
        }


        #region DEBUG

        private void OnDrawGizmos()
        {
            if (_selection)
            {
                Gizmos.color = new Color(0, 0, 1, 0.2f);
                Gizmos.DrawCube(_selection.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
            }
        }

        #endregion
    }
}