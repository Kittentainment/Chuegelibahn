using System;
using Moving;
using Snapping;
using TrackPrinting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrackBuilder
{
    private readonly Draggable _draggable; // TODO only used for testing

    private static int _currTrackID = 0;
    
    public TrackType type { get; }
    private Vector3 lastTrackPrinterPos { get; set; }
    private Vector3 lastDraggablePos { get; set; }

    private int _lastNumberOfElements = 0;

    private TrackSegment _track;
    private static TrackPrefabManager prefabManager => TrackPrefabManager.instance;

    public TrackBuilder(TrackType type, Vector3 trackPrinterPos, Vector3 draggablePos, Draggable draggable)
    {
        _draggable = draggable;
        this.type = type;
        UpdatePositions(trackPrinterPos, draggablePos);
        _track = new GameObject().AddComponent<TrackSegment>();
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
        var upwardsDirection = trackPrinter.up;
        var distance = Vector3.Dot(drawLine, outputDirection);
        var pieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        var numberOfNeededElements = Mathf.RoundToInt(distance / pieceLength) + 1;
        
        // TODO add max number of elements

        // if (numberOfNeededElements != _lastNumberOfElements) // TODO We Can't really check for that, as we also need to account for rotation changes of the Track Printer, and in VR this probably happens constantly. But it's helpful for Debug,
        // { 
            Debug.Log("numberOfNeededElements = " + numberOfNeededElements);
            UpdateTrackPreview(numberOfNeededElements, outputDirection, upwardsDirection, trackPrinterPos);
        // }

        _lastNumberOfElements = numberOfNeededElements;
        UpdatePositions(trackPrinterPos, draggablePos);

        if (numberOfNeededElements > 30 && _draggable.isGrabbed)
        {
            // _draggable.LetGo(); // DEBUG only for testing
            // _draggable.trackPrinter!.PrintCurrentTrack(); // DEBUG only for testing
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numberOfElements"></param>
    /// <param name="outputDirection"></param>
    /// <param name="startPos">The position of the printer, where we start printing the objects from</param>
    private void UpdateTrackPreview(int numberOfElements, Vector3 outputDirection, Vector3 upwardsDirection, Vector3 startPos)
    {
        var trackPrinterRotation = Quaternion.LookRotation(outputDirection, upwardsDirection);// Quaternion.FromToRotation(Vector3.forward, outputDirection); // The rotation of the TrackPrinter in WorldCoordinates.
        DeleteTrackPreview();
        _track = new GameObject("Track Builder Pieces").AddComponent<TrackSegment>();
        _track.transform.SetPositionAndRotation(startPos + TrackPrefabManager.GetVectorFromPivotToCenterBottom(type), trackPrinterRotation);
        var singlePieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        var singlePieceRotation = TrackPrefabManager.GetRotationOfTrackPiece(type);
        for (var i = 0; i < numberOfElements; i++)
        {
            var trackPiece = GameObject.Instantiate(prefabManager.StraightPiece, _track.transform);
            trackPiece.name = $"New TrackPiece {i}";
            var position = type switch {
                TrackType.Straight => _track.transform.position + i * singlePieceLength * outputDirection,
                TrackType.Left => _track.transform.position,
                _ => throw new ArgumentOutOfRangeException()
            };
            var rotation = type switch {
                TrackType.Straight => trackPrinterRotation,
                TrackType.Left => Quaternion.FromToRotation(Vector3.forward, outputDirection + new Vector3(0, singlePieceRotation * i, 0)),
                _ => throw new ArgumentOutOfRangeException()
            };
            trackPiece.transform.SetPositionAndRotation(position, rotation);
            _track.trackPieces.Add(trackPiece);
        }
    }

    public GameObject PrintCurrentTrack()
    {
        var currentTrack = _track;
        _track = null;
        return FinishTrackPiece(currentTrack);
    }

    private GameObject FinishTrackPiece(TrackSegment segment)
    {
        AddSnappingPoints(segment);
        var wrapper = PackSegmentInWrapper(segment);
        MakeInteractable(segment, wrapper);
        return wrapper;
    }

    private void MakeInteractable(TrackSegment trackSegment, GameObject wrapper)
    {
        var centerPiece = trackSegment.GetMiddleTrackPiece;
        var grabInteractable = wrapper.AddComponent<XRGrabInteractable>();
        var rigidbody = grabInteractable.gameObject.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        grabInteractable.attachTransform = centerPiece.transform;
        // Events for when a piece is interacted with:
        grabInteractable.selectEntered.AddListener(_ =>
        {
            Debug.Log("Grabbed a Segment which should now be selected and snapping to other segments.");
            MoveObjectController.Instance.SelectAnObject(wrapper.GetComponent<SnappingObjWrapper>());
        });
        grabInteractable.selectExited.AddListener(_ =>
        {
            Debug.Log("Let go of an object which should now be deselected and snapped to anything if there is something near");
            MoveObjectController.Instance.DeselectObject();
        });
    }

    /// <summary>
    /// Replaces the TrackPieces at the ends with TrackPieces with an Anchor.
    /// </summary>
    /// <param name="track">The segment to work on.</param>
    private void AddSnappingPoints(TrackSegment track)
    {
        var trackPiecePrefabs = prefabManager.GetTrackPrefabsForType(track.trackPieces[0].type);
        var firstTrackPieceToReplace = track.trackPieces[0];
        var firstTrackPieceToReplaceTF = firstTrackPieceToReplace.transform;
        var lastTrackPieceToReplace = track.trackPieces[track.trackPieces.Count - 1];
        var lastTrackPieceToReplaceTF = lastTrackPieceToReplace.transform;

        var firstTpNew = GameObject.Instantiate(trackPiecePrefabs.first);
        firstTpNew.transform.position = firstTrackPieceToReplaceTF.position;
        firstTpNew.transform.rotation = firstTrackPieceToReplaceTF.rotation;
        firstTpNew.transform.parent = track.transform;
        
        var lastTpNew = GameObject.Instantiate(trackPiecePrefabs.last);
        lastTpNew.transform.position = lastTrackPieceToReplaceTF.position;
        lastTpNew.transform.rotation = lastTrackPieceToReplaceTF.rotation;
        lastTpNew.transform.parent = track.transform;

        track.trackPieces.Remove(firstTrackPieceToReplace);
        track.trackPieces.Remove(lastTrackPieceToReplace);
        GameObject.DestroyImmediate(firstTrackPieceToReplace.gameObject);
        GameObject.DestroyImmediate(lastTrackPieceToReplace.gameObject);
    }

    private GameObject PackSegmentInWrapper(TrackSegment track)
    {
        var wrapperGO = new GameObject($"Track Piece {_currTrackID++}");
        wrapperGO.transform.position = track.GetMiddleTrackPiece.transform.position;
        var wrapper = wrapperGO.AddComponent<SnappingObjWrapper>();
        track.transform.parent = wrapper.objToSnap.transform;
        wrapper.UpdateAnchors(collectAnchors: true);
        return wrapperGO;
    }


    private void DeleteTrackPreview()
    {
        if (_track == null) return;
        _track.trackPieces.Clear();
        GameObject.Destroy(_track.gameObject);
    }

    public void DestroyYourself()
    {
        DeleteTrackPreview();
    }
}