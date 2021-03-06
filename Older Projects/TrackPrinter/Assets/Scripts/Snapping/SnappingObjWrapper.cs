using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Snapping
{
    public class SnappingObjWrapper : MonoBehaviour
    {
        #region GlobalSettings

        /// <summary>
        /// Whether to use a preview to indicate where the object will snap, if let go, or to snap the object directly.
        /// </summary>
        public static bool UseSnappingPreviews { get; set; } = false;

        #endregion


        private readonly List<Anchor> _anchors = new List<Anchor>();

        public ObjToSnap objToSnap { get; private set; }

        private bool _isBeingMoved = false;
        /// <summary>
        /// Whether this object is currently selected and being moved.
        /// </summary>
        private bool IsBeingMoved
        {
            get => _isBeingMoved;
            set
            {
                _isBeingMoved = value;
                _anchors.ForEach(anchor => anchor.IsBeingMoved = value);
            }
        }

        public bool IsSnapping => CurrentSnapping != null;
        [CanBeNull] public SnappingResult CurrentSnapping { get; private set; }
        [CanBeNull] private GameObject _currentSnappingPreviewGO;

        [CanBeNull]
        private GameObject CurrentSnappingPreviewGO
        {
            get => _currentSnappingPreviewGO;
            set
            {
                if (_currentSnappingPreviewGO != null)
                {
                    Destroy(_currentSnappingPreviewGO);
                }

                _currentSnappingPreviewGO = value;
            }
        }


        private void Awake()
        {
            objToSnap ??= GetComponentInChildren<ObjToSnap>();
            if (objToSnap == null)
            { // We probably created this wrapper manually in code. Add a basic objToSnap. IMPORTANT: If you did this, don't forget to UpdateAnchors() again once you added them.
                objToSnap ??= new GameObject("ObjToSnap").AddComponent<ObjToSnap>();
                objToSnap.transform.parent = this.transform;
            }

            UpdateAnchors();
            if (!objToSnap.transform.localPosition.Equals(Vector3.zero))
            {
                Debug.LogWarning(
                    $"The ObjToSnap should be at (0, 0, 0) instead of {transform.localPosition} (relative to it's parent object)");
                ResetTransformLocally(objToSnap.transform);
            }

            Debug.Log($"Anchors Found: {_anchors.Count}");
        }


        private void FixedUpdate()
        {
            if (IsBeingMoved)
            {
                UpdateCurrentSnapping();
            }
        }
        
        /// <summary>
        /// Update the list of all anchors in child game objects and optionally move them to a separate child game object, for the snapping logic to work.
        /// </summary>
        public void UpdateAnchors(bool collectAnchors = false)
        {
            _anchors.AddRange(gameObject.GetComponentsInChildren<Anchor>());
            if (collectAnchors)
            {
                // Collect all anchors in new collection parent object
                var anchorCollection = new GameObject("AutomaticAnchorCollection").AddComponent<AnchorCollection>();
                anchorCollection.AddAnchorsAsChildren(_anchors);
                
                // Destroy previous anchor collections
                foreach (var oldAnchorCollections in GetComponentsInChildren<AnchorCollection>())
                    Destroy(oldAnchorCollections);
                // Add new collection to this wrapper
                anchorCollection.transform.parent = this.transform;
            }
        }


        /// <summary>
        /// Tells the object that it has been selected and is now being moved.
        /// From now on it will check for snapping and snap to places until let go again with LetGoAndSnap.
        /// </summary>
        public void MovementHasStarted()
        {
            IsBeingMoved = true;
        }
        
        /// <summary>
        /// Let's the object go (deselect it) and tells it to stay where it is (or be moved by physics, depending on the
        /// object, and the rigidbody etc., but not by the player anymore).
        /// If it is currently in snapping range of an other object, it will now snap permanently to it (TODO with a fixed joint),
        /// and reset the wrapped object to it's parents origin (but keeping it in place, as we move the wrapper as well).
        ///
        /// If we ever want to move two pieces simultaneously (maybe left and right hand in VR), we can check here if it
        /// would snap to the piece in the other hand if let go.
        /// </summary>
        /// <exception cref="Exception">Thrown if the object has not been held in the first place.</exception>
        public void LetGoAndSnap()
        {
            if (!IsBeingMoved)
                throw new Exception("Can't let go what was not being moved in the first place!");

            if (IsSnapping)
            {
                if (UseSnappingPreviews)
                    CurrentSnappingPreviewGO = null;
                else
                    ResetTransformLocally(objToSnap.transform);

                ApplySnappingToTransform(transform, CurrentSnapping);
            }

            IsBeingMoved = false;
        }

        private static void ResetTransformLocally(Transform transformToReset)
        {
            transformToReset.localPosition = Vector3.zero;
            // _objToSnap.transform.rotation.Set(Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w);
            transformToReset.localEulerAngles = Vector3.zero;
        }

        private static void ApplySnappingToTransform(Transform transform, SnappingResult snappingResult)
        {
            var movementVector = snappingResult.GetMovementVector();
            var rotation = snappingResult.GetRotation();
            transform.Translate(movementVector, Space.World);
            RotateAround(transform, snappingResult.OtherAnchor.transform.position, rotation);
            // transform.localRotation = rotation;
        }

        /// <summary>
        /// From https://answers.unity.com/questions/1751620/rotating-around-a-pivot-point-using-a-quaternion.html
        /// </summary>
        /// <param name="transform">The transform to rotate</param>
        /// <param name="pivotPoint">The point to rotate it around</param>
        /// <param name="rot">The rotation to apply</param>
        static void RotateAround(Transform transform, Vector3 pivotPoint, Quaternion rot)
        {
            transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
            transform.rotation = rot * transform.rotation;
        }

        /// <summary>
        /// Snaps either the ObjToSnap or the Preview Object to the position the snap would happen if let go,
        /// but without letting it go (meaning the parent GO stays in place).
        /// First resets the ObjToSnap or the preview.
        /// </summary>
        /// <exception cref="NullReferenceException">If (CurrentSnapping == null) or (UseSnappingPreviews && CurrentSnappingPreviewGO == null)</exception>
        private void SnapToCurrentSnappingPosition()
        {
            if (CurrentSnapping == null)
                throw new NullReferenceException("We can't snap when we have nothing to snap to");
            if (UseSnappingPreviews && CurrentSnappingPreviewGO == null)
                throw new NullReferenceException(
                    "Please create the snapping preview object before calling SnapToCurrentSnappingPosition()");

            var transformToSnap = UseSnappingPreviews ? CurrentSnappingPreviewGO.transform : objToSnap.transform;
            ResetTransformLocally(transformToSnap);
            // _objToSnap.transform.Translate(CurrentSnapping.GetMovementVector());
            ApplySnappingToTransform(transformToSnap, CurrentSnapping);
        }

        /// <summary>
        /// Check if we are in snapping radius, and handle the cases of a new snapping, a changed snapping, or a stopped snapping.
        /// </summary>
        private void UpdateCurrentSnapping()
        {
            var wasSnappingBefore = IsSnapping;
            var snappingAnchorBefore = CurrentSnapping?.OtherAnchor;

            CurrentSnapping = GetNearestSnapping();

            if (CurrentSnapping == null)
            {
                if (!wasSnappingBefore) return;
                // We were snapping before, but now aren't (meaning we left the radius of the anchor).
                // Clean up snapping stuff:
                if (UseSnappingPreviews)
                    CurrentSnappingPreviewGO = null;
                else
                    ResetTransformLocally(objToSnap.transform);

                return;
            }

            // We found a snapping
            Debug.Log($"Found Snapping: {CurrentSnapping}");
            if (!wasSnappingBefore || snappingAnchorBefore == CurrentSnapping.OtherAnchor)
            {
                // It's a new snapping! Create a preview object if we use previews.
                if (UseSnappingPreviews)
                {
                    CurrentSnappingPreviewGO = objToSnap.CreateSnappingPreviewObject(this.transform);
                }
            }

            SnapToCurrentSnappingPosition();
        }

        private SnappingResult GetNearestSnapping()
        {
            return _anchors
                .SelectMany(ownAnchor => ownAnchor.GetOtherAnchorsInRange(_anchors)
                    .Select(otherAnchor => new SnappingResult(ownAnchor, otherAnchor,
                        Vector3.Distance(ownAnchor.AnchorPosition, otherAnchor.AnchorPosition)))
                )
                .OrderBy(result => result.Distance)
                .FirstOrDefault();
        }


        #region DEBUG

        private void OnDrawGizmos()
        {
            if (CurrentSnapping != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(CurrentSnapping.OwnAnchor.AnchorPosition,
                    CurrentSnapping.OwnAnchor.AnchorPosition + CurrentSnapping.GetMovementVector());
            }
        }

        #endregion
    }
}