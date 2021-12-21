using UnityEngine;

public class TrackBuilder
{
    private readonly Draggable _draggable; // TODO only used for testing
    public TrackType type { get; }
    private Vector3 lastTrackPrinterPos { get; set; }
    private Vector3 lastDraggablePos { get; set; }

    private int _lastNumberOfElements = 0;

    private GameObject _track;

    public TrackBuilder(TrackType type, Vector3 trackPrinterPos, Vector3 draggablePos, Draggable draggable)
    {
        _draggable = draggable;
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
        var numberOfNeededElements = Mathf.RoundToInt(distance / pieceLength) + 1;

        if (numberOfNeededElements != _lastNumberOfElements) // TODO We Can't really check for that, as we also need to account for rotation changes of the Track Printer, and in VR this probably happens constantly. But it's helpful for Debug,
        { 
            Debug.Log("numberOfNeededElements = " + numberOfNeededElements);
            UpdateTrackPreview(numberOfNeededElements, outputDirection, trackPrinterPos);
        }

        _lastNumberOfElements = numberOfNeededElements;
        UpdatePositions(trackPrinterPos, draggablePos);

        if (numberOfNeededElements > 30 && _draggable.isGrabbed)
        {
            _draggable.LetGo(); // DEBUG only for testing
        }
    }

    private void UpdateTrackPreview(int numberOfElements, Vector3 outputDirection, Vector3 startPos)
    {
        var rotation = Quaternion.FromToRotation(Vector3.forward, outputDirection); // The rotation of the TrackPrinter in WorldCoordinates.
        DeleteTrackPreview();
        _track = new GameObject("Track Builder Pieces");
        _track.transform.SetPositionAndRotation(startPos + TrackPrefabManager.GetVectorFromPivotToCenterBottom(type), rotation);
        var trackPrefabManager = TrackPrefabManager.instance;
        var singlePieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        for (var i = 0; i < numberOfElements; i++)
        {
            var trackPiece = GameObject.Instantiate(trackPrefabManager.StraightPiece, _track.transform);
            trackPiece.name = $"New TrackPiece {i}";
            trackPiece.transform.SetPositionAndRotation(_track.transform.position + i * singlePieceLength * outputDirection, rotation);
        }
    }

    private void DeleteTrackPreview()
    {
        GameObject.Destroy(_track);
    }

    public void DestroyYourself()
    {
        DeleteTrackPreview();
    }
}