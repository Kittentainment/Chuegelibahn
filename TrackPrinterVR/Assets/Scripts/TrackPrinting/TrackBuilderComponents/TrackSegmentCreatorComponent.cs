using UnityEngine;

namespace TrackPrinting.TrackBuilderComponents
{
    public abstract class TrackSegmentCreatorComponent
    {
    
        private static TrackPrefabManager prefabManager => TrackPrefabManager.instance;

    
        // protected abstract Vector3 GetPiecePosition(Vector3 outputDirection, Vector3 upwardsDirection, int i, float singlePieceLength,
        //     float singlePieceRotation, int numberOfElements);
        // {
        //     var position = type switch
        //     {
        //         TrackType.Straight => _trackBuilderSegment.transform.position + i * singlePieceLength * outputDirection,
        //         TrackType.Left => _trackBuilderSegment.transform.position,
        //         _ => throw new ArgumentOutOfRangeException()
        //     };
        //     if (type == TrackType.Left && i == numberOfElements - 1)
        //     {
        //         // Curves end pieces need special handling
        //         position += outputDirection.RotateAround(upwardsDirection, (i - 1) * singlePieceRotation).normalized *
        //                     singlePieceLength;
        //     }
        //     return position;
        // }

        public TrackSegment CreateSegment(TrackType type, int numberOfElements, Vector3 outputDirection, Vector3 upwardsDirection, Vector3 rightDirection, Vector3 startPos, Quaternion trackPrinterRotation)
        {
            var trackBuilderSegment = new GameObject("Track Segment").AddComponent<TrackSegment>();
            trackBuilderSegment.transform.SetPositionAndRotation(startPos + TrackPrefabManager.GetVectorFromPivotToCenterBottom(type, rightDirection), trackPrinterRotation);
            var singlePieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
            var singlePieceRotation = TrackPrefabManager.GetRotationOfTrackPiece(type);
            var piecePrefabs = prefabManager.GetTrackPrefabsForType(type);
            for (var i = 0; i < numberOfElements; i++)
            {
                var trackPiece = CreateSinglePiece(type, piecePrefabs, numberOfElements, outputDirection, upwardsDirection, trackPrinterRotation, i, trackBuilderSegment, singlePieceRotation, singlePieceLength);
            }

            return trackBuilderSegment;
        }

        private TrackPiece CreateSinglePiece(TrackType type, TrackPiecePrefabs piecePrefabs,
            int numberOfElements, Vector3 outputDirection,
            Vector3 upwardsDirection, Quaternion trackPrinterRotation, int i, TrackSegment trackBuilderSegment,
            float singlePieceRotation, float singlePieceLength)
        {
            TrackPiece original;
            // Use the correct piece for type and whether in the middle or at the end.
            if (i == 0 || i == numberOfElements - 1)
                original = piecePrefabs.EndPieces;
            else
                original = piecePrefabs.Middle;
            var trackPiece = GameObject.Instantiate(original, trackBuilderSegment.transform);
            trackPiece.name = $"New TrackPiece {i}";
            // Use the correct position and rotation logic according to type and end/middle
            var rotation = GetPieceRotation(type, trackBuilderSegment, outputDirection, upwardsDirection, trackPrinterRotation,
                i, singlePieceRotation, numberOfElements);
            var position = GetPiecePosition(type, trackBuilderSegment, outputDirection, upwardsDirection, i, singlePieceLength,
                singlePieceRotation, numberOfElements);
            // End pieces for the curve
            trackPiece.transform.SetPositionAndRotation(position, rotation);
            trackBuilderSegment.trackPieces.Add(trackPiece);
            return trackPiece;
        }

        protected abstract Vector3 GetPiecePosition(TrackType type, TrackSegment trackBuilderSegment, Vector3 outputDirection,
            Vector3 upwardsDirection, int i, float singlePieceLength, float singlePieceRotation, int numberOfElements);

        protected abstract Quaternion GetPieceRotation(TrackType type, TrackSegment trackBuilderSegment,
            Vector3 outputDirection, Vector3 upwardsDirection,
            Quaternion trackPrinterRotation,
            int i, float singlePieceRotation, int numberOfElements);

    }
}