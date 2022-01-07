using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapping
{
    public class Anchor : MonoBehaviour
    {
        #region GlobalSettings

        /// <summary>
        /// It only snaps, if the angle of the other normalDirection to this Anchor's normalDirection is [180-SnappingAngle] (as normals snap to opposite direction)
        /// </summary>
        public static float SnappingAngle { get; set; } = 180;

        #endregion


        [field: SerializeField]
        [field: Range(0, 10f)]
        public float SnappingRadius { get; private set; } = 0.5f;

        [SerializeField] private int snappingLayer = 0;

        [SerializeField] private Vector3 forwardDirection;
        [SerializeField] private Vector3 upwardDirection;
        public Vector3 ForwardVector => transform.rotation * forwardDirection.normalized;
        public Vector3 UpwardVector => transform.rotation * upwardDirection.normalized;
        
        public bool IsBeingMoved { get; set; }

        /// <summary>
        /// The position of this Anchor in World Coordinates.
        ///
        /// Just returns transform.position, but now it could be changed, so an anchor can be created differently, eg just from code.
        /// </summary>
        public Vector3 AnchorPosition => transform.position;


        /// <summary>
        /// Returns a list of other Anchors which are in Range of this Anchor's SnappingRadius, belong to the same snappingLayer,
        /// and do not belong to the same SnappingObj.
        /// </summary>
        /// <param name="ownAnchors">All the Anchors belonging to the same SnappingObj</param>
        /// <returns>A list of other Anchors which are in Range of this Anchor's SnappingRadius and do not belong to the same SnappingObj.</returns>
        public IEnumerable<Anchor> GetOtherAnchorsInRange(List<Anchor> ownAnchors)
        {
            // Debug.Log("GetOtherAnchorsInRange");
            ownAnchors ??= new List<Anchor>();
            var anchorsInRange = Physics.OverlapSphere(this.AnchorPosition, this.SnappingRadius)
                // Get the Anchor Components (if they exist):
                .Select(coll => coll.gameObject.GetComponent<Anchor>())
                .Where(anchor => anchor != null)
                // Only check anchors not belonging to this SnappingObj:
                .Where(anchor => !ownAnchors.Contains(anchor))
                // Only take anchors on the same layer:
                .Where(anchor => anchor.snappingLayer == this.snappingLayer)
                // only take anchors where the angle between the two normals are big enough (no small, as they need to point in opposite directions)
                .Where(anchor => Mathf.Abs(Vector3.Angle(anchor.ForwardVector, ForwardVector)) > 180 - SnappingAngle)
                .ToList();
            if (anchorsInRange.Count > 0)
            {
                Debug.Log("Anchor::GetOtherAnchorsInRange - Found Anchors in Range!");
            }

            return anchorsInRange;
        }


        #region Debug

        [Header("Debug Settings")] [SerializeField]
        private bool showDebugSettings = true;
        
        private Color _gizmosForwardColor = Color.green;
        private Color _gizmosUpwardsColor = Color.cyan;

        [SerializeField] private float gizmosSnappingVisibility = 0.2f;

        [SerializeField] private float objectRadius = 0.2f;

        private static List<Color> snappingLayerColors = new List<Color>(new[]
            { Color.red, Color.blue, Color.cyan, Color.green, Color.yellow, Color.black });
        
        private Color gizmosColor => snappingLayerColors[snappingLayer % snappingLayerColors.Count];


        private void OnDrawGizmos()
        {
            if (!showDebugSettings)
                return;
            var position = transform.position;
            if (IsBeingMoved)
            {
                // Draw the radius sphere, but only if it is actually currently used.
                DrawSnappingRadiusGizmos(position);
            }
            // Draw a sphere to represent the anchor itself. Take a color to distinguish the different layers. 
            
            Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, 1f);
            Gizmos.DrawSphere(position, objectRadius);
            // Draw a sphere to represent the normal vector.
            Gizmos.color = new Color(_gizmosForwardColor.r, _gizmosForwardColor.g, _gizmosForwardColor.b, 1f);
            Gizmos.DrawLine(position, position + ForwardVector);
            Gizmos.color = new Color(_gizmosUpwardsColor.r, _gizmosUpwardsColor.g, _gizmosUpwardsColor.b, 1f);
            Gizmos.DrawLine(position, position + UpwardVector);
        }

        private void OnDrawGizmosSelected()
        {
            // Also draw the sphere if we have the object selected, to better adjust it in the inspector.
            DrawSnappingRadiusGizmos(transform.position);
        }

        private void DrawSnappingRadiusGizmos(Vector3 position)
        {
            // Draw a sphere to indicate the snapping radius.
            Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, gizmosSnappingVisibility);
            Gizmos.DrawSphere(position, SnappingRadius);
        }
        
        #endregion
    }
}