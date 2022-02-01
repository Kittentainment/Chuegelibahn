using UnityEngine;

namespace TrackPrinting.TrackBuilderComponents
{
    public class AngleBasedNoECalcComponent : NumberOfElementsCalcComponent
    {
    
        public override int CalculateNumberOfNeededElements(TrackType type, float distance, float horizontalAngle)
        {
            var numberOfNeededElements =
                Mathf.RoundToInt(GetActualAngle(horizontalAngle) / TrackPrefabManager.GetRotationOfTrackPiece(type)) + 2;
        
            if (numberOfNeededElements > TrackPrefabManager.GetMaximumNumberOfPieces(type))
            {
                numberOfNeededElements = TrackPrefabManager.GetMaximumNumberOfPieces(type);
            }

            return numberOfNeededElements;
        }
    
    
        /// <summary>
        /// The angle used for determining the number of pieces if we want a curved piece.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static float GetActualAngle(float angle)
        {
            const int step = 15;
            if (angle >= 0 && angle < 90)
                return 180;
            if (angle >= 90)
                return 0;
            return Mathf.RoundToInt(angle / step) * step + 180;
        }
    }
}