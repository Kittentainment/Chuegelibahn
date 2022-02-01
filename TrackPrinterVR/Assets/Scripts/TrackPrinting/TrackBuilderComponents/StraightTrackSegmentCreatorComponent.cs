using UnityEngine;

namespace TrackPrinting.TrackBuilderComponents
{
    public class StraightTrackSegmentCreatorComponent : TrackSegmentCreatorComponent
    {
    
        protected override Vector3 GetPiecePosition(TrackType type, TrackSegment trackBuilderSegment, Vector3 outputDirection,
            Vector3 upwardsDirection, int i, float singlePieceLength, float singlePieceRotation, int numberOfElements)
        {
            var position = trackBuilderSegment.transform.position + i * singlePieceLength * outputDirection;
            return position;
        }
    
        protected override Quaternion GetPieceRotation(TrackType type, TrackSegment trackBuilderSegment,
            Vector3 outputDirection, Vector3 upwardsDirection, Quaternion trackPrinterRotation, int i,
            float singlePieceRotation, int numberOfElements)
        {
            return trackPrinterRotation;
        }
    }
}