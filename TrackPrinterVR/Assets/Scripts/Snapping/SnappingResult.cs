using UnityEngine;

namespace Snapping
{
    public class SnappingResult
    {
        public SnappingResult(Anchor ownAnchor, Anchor otherAnchor, float distance)
        {
            OwnAnchor = ownAnchor;
            OtherAnchor = otherAnchor;
            Distance = distance;
            Debug.Log($"Created a new SnappingResult: {this}");
        }

        /// <summary>
        /// The Anchor of the object which is being moved.
        /// </summary>
        public readonly Anchor OwnAnchor;

        /// <summary>
        /// The Anchor of the object which OwnAnchor is in range of and should snap to.
        /// </summary>
        public readonly Anchor OtherAnchor;

        /// <summary>
        /// The Distance between the two Anchor's positions.
        /// Needs to be calculated once and then stay fixed, to get the lowest one by exact comparison with the LINQ. (See Anchor::GetOtherAnchorsInRange)
        /// </summary>
        public readonly float Distance;

        /// <summary>
        /// The vector the ownAnchor needs to move, to be at the same position as the otherAnchor.
        /// This can then be applied to the ObjToSnap, to snap perfectly to the otherAnchor's Object.
        /// </summary>
        /// <returns>The vector the ownAnchor needs to move, to be at the same position as the otherAnchor.</returns>
        public Vector3 GetMovementVector()
        {
            return OtherAnchor.AnchorPosition - OwnAnchor.AnchorPosition;
        }

        public Quaternion GetForwardRotation()
        {
            return Quaternion.FromToRotation(OwnAnchor.ForwardVector, OtherAnchor.ForwardVector * -1);
        }

        public Quaternion GetUpwardRotation()
        {
            return Quaternion.FromToRotation(OwnAnchor.UpwardVector, OtherAnchor.UpwardVector);
        }

        public override string ToString()
        {
            return $"Distance: {Distance}";
        }
    }
}