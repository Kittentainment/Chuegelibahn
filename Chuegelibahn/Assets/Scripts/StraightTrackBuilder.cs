using UnityEngine;

public class StraightTrackBuilder : ITrackBuilder
{
    public TrackType type => TrackType.Straight;
    private Vector3 lastTrackPrinterPos { get; set; }
    private Vector3 lastDraggablePos { get; set; }

    public StraightTrackBuilder(Vector3 trackPrinterPos, Vector3 draggablePos)
    {
        UpdatePositions(trackPrinterPos, draggablePos);
    }

    private void UpdatePositions(Vector3 trackPrinterPos, Vector3 draggablePos)
    {
        lastTrackPrinterPos = trackPrinterPos;
        lastDraggablePos = draggablePos;
    }

    
    
    public void OnDrag(Transform trackPrinter, Transform draggable)
    {
        Vector3 trackPrinterPos = trackPrinter.position;
        Vector3 draggablePos = draggable.position;
        Vector3 drawLine = draggablePos - trackPrinterPos;
        Vector3 outputLine = trackPrinter.forward;
        float distance = Vector3.Dot(drawLine, outputLine);
        int numberOfNeededElements = Mathf.RoundToInt(distance / TrackPrefabs.GetLengthOfTrackPiece(type));
        
        Debug.Log("numberOfNeededElements = " + numberOfNeededElements);
        UpdatePositions(trackPrinterPos, draggablePos);
    }
    
}