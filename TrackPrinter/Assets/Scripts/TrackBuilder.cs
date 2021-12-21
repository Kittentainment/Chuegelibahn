using UnityEngine;

public class TrackBuilder
{
    public TrackType type { get; }
    private Vector3 lastTrackPrinterPos { get; set; }
    private Vector3 lastDraggablePos { get; set; }

    private int lastNumberOfElements = 0;

    private GameObject _track;

    public TrackBuilder(TrackType type, Vector3 trackPrinterPos, Vector3 draggablePos)
    {
        this.type = type;
        UpdatePositions(trackPrinterPos, draggablePos);
        _track = new GameObject();
        _track.transform.position = trackPrinterPos;
    }

    private void UpdatePositions(Vector3 trackPrinterPos, Vector3 draggablePos)
    {
        lastTrackPrinterPos = trackPrinterPos;
        lastDraggablePos = draggablePos;
    }

    
    
    public void OnDrag(Transform trackPrinter, Transform draggable)
    {
        var trackPrinterPos = trackPrinter.position;
        var draggablePos = draggable.position;
        var drawLine = draggablePos - trackPrinterPos;
        var outputDirection = trackPrinter.forward;
        var distance = Vector3.Dot(drawLine, outputDirection);
        var pieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        var numberOfNeededElements = Mathf.RoundToInt(distance / pieceLength);
        Debug.Log("numberOfNeededElements = " + numberOfNeededElements);

        if (numberOfNeededElements != lastNumberOfElements)
        {
            UpdateTrackPreview(numberOfNeededElements, outputDirection, trackPrinterPos);
        }

        lastNumberOfElements = numberOfNeededElements;
        UpdatePositions(trackPrinterPos, draggablePos);
    }

    private void UpdateTrackPreview(int numberOfElements, Vector3 outputDirection, Vector3 startPos)
    {
        var rotation = Quaternion.Euler(outputDirection);
        var rotationWithDefault =
            Quaternion.Euler(outputDirection + TrackPrefabManager.GetDefaultRotationOfPiece(type));
        DeleteTrackPreview();
        _track = new GameObject();
        _track.transform.SetPositionAndRotation(startPos, rotation);
        var trackPrefabManager = TrackPrefabManager.instance;
        var singlePieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        for (var i = 0; i < numberOfElements; i++)
        {
            var trackPiece = GameObject.Instantiate(trackPrefabManager.StraightPiece, _track.transform);
            trackPiece.transform.SetPositionAndRotation(startPos + i * singlePieceLength * outputDirection, rotationWithDefault);
        }
    }

    private void DeleteTrackPreview()
    {
        GameObject.Destroy(_track);
    }
}