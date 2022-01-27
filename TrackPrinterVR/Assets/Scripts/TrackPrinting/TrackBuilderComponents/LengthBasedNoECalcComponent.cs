using UnityEngine;

namespace TrackPrinting.TrackBuilderComponents;

public class LengthBasedNoECalcComponent : NumberOfElementsCalcComponent
{
    public override int CalculateNumberOfNeededElements(TrackType type, float distance, float horizontalAngle)
    {
        var pieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        var numberOfNeededElements = Mathf.RoundToInt(distance / pieceLength);
        
        if (numberOfNeededElements > TrackPrefabManager.GetMaximumNumberOfPieces(type))
            numberOfNeededElements = TrackPrefabManager.GetMaximumNumberOfPieces(type);

        return numberOfNeededElements;
    }
    
}