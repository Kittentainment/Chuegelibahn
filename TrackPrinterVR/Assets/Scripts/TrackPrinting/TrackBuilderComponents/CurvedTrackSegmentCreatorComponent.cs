using System;
using ExtensionMethods;
using UnityEngine;

namespace TrackPrinting.TrackBuilderComponents;

public class CurvedTrackSegmentCreatorComponent : TrackSegmentCreatorComponent
{
    protected override Vector3 GetPiecePosition(TrackType type, TrackSegment trackBuilderSegment, Vector3 outputDirection,
        Vector3 upwardsDirection, int i, float singlePieceLength, float singlePieceRotation, int numberOfElements)
    {
        var position = trackBuilderSegment.transform.position;
        if (i == numberOfElements - 1)
        {
            // Curves end pieces need special handling
            position += outputDirection.RotateAround(upwardsDirection, (i - 1) * singlePieceRotation).normalized *
                        singlePieceLength;
        }
        return position;
    }
    
    protected override Quaternion GetPieceRotation(TrackType type, TrackSegment trackBuilderSegment,
        Vector3 outputDirection, Vector3 upwardsDirection, Quaternion trackPrinterRotation, int i,
        float singlePieceRotation, int numberOfElements)
    {
        if (i == numberOfElements - 1)
        {
            // The last element should not be rotated again, as it is a straight piece
            i--;
        }
        var rotation = Quaternion.LookRotation(outputDirection.RotateAround(upwardsDirection, i * singlePieceRotation), upwardsDirection);
        
        return rotation;
    }
}