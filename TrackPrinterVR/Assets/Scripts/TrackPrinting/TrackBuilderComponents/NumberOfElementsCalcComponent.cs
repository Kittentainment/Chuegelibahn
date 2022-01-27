namespace TrackPrinting.TrackBuilderComponents;

public abstract class NumberOfElementsCalcComponent
{
    public abstract int CalculateNumberOfNeededElements(TrackType type, float distance, float horizontalAngle);
}