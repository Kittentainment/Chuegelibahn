using System;
using ExtensionMethods;
using Moving;
using Snapping;
using TMPro;
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

    private TrackSegment _trackBuilderSegment;
    private static TrackPrefabManager prefabManager => TrackPrefabManager.instance;

    public TrackBuilder(TrackType type, Vector3 trackPrinterPos, Vector3 draggablePos, Draggable draggable)
    {
        _draggable = draggable;
        this.type = type;
        UpdateLastPositions(trackPrinterPos, draggablePos);
        _trackBuilderSegment = new GameObject().AddComponent<TrackSegment>();
        _trackBuilderSegment.transform.position = trackPrinterPos;
    }

    private void UpdateLastPositions(Vector3 trackPrinterPos, Vector3 draggablePos)
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
        var horizontalAngle = Vector3.SignedAngle(outputDirection, draggable.forward, upwardsDirection);
        var pieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        var numberOfNeededElements = type switch
        {
            TrackType.Straight => Mathf.RoundToInt(distance / pieceLength),
            TrackType.Left => Mathf.RoundToInt(GetActualAngle(horizontalAngle) / TrackPrefabManager.GetRotationOfTrackPiece(type)),
            _ => throw new ArgumentOutOfRangeException()
        };
        numberOfNeededElements += 1; // Because they are rounded down.
        if (numberOfNeededElements <= 0) return;

        if (numberOfNeededElements > TrackPrefabManager.GetMaximumNumberOfPieces(type))
        {
            numberOfNeededElements = TrackPrefabManager.GetMaximumNumberOfPieces(type);
        }

        // if (numberOfNeededElements != _lastNumberOfElements) // TODO We Can't really check for that, as we also need to account for rotation changes of the Track Printer, and in VR this probably happens constantly. But it's helpful for Debug,
        // { 
            Debug.Log("numberOfNeededElements = " + numberOfNeededElements);
            UpdateTrackPreview(numberOfNeededElements, outputDirection, upwardsDirection, trackPrinter);
        // }

        _lastNumberOfElements = numberOfNeededElements;
        UpdateLastPositions(trackPrinterPos, draggablePos);

        // if (numberOfNeededElements > 30 && _draggable.isGrabbed)
        // {
        //     // _draggable.LetGo(); // DEBUG only for testing
        //     // _draggable.trackPrinter!.PrintCurrentTrack(); // DEBUG only for testing
        // }
    }

    /// <summary>
    /// The angle used for determining the number of pieces if we want a curved piece.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float GetActualAngle(float angle)
    {
        if (angle >= 0 && angle < 90)
            return 180;
        if (angle >= 90)
            return 0;
        return angle + 180;
    }

    /// <summary>
    /// Update the pieces which show where the piece to create will be created.
    /// They will also be used to create the actual pieces.
    /// </summary>
    /// <param name="numberOfElements">How many Pieces should be created.</param>
    /// <param name="outputDirection">The direction the printer is facing, where at least the first piece should also face to.</param>
    /// <param name="upwardsDirection">The upward direction of the printer.</param>
    /// <param name="printerTransform">The position of the printer, where we start printing the objects from.</param>
    private void UpdateTrackPreview(int numberOfElements, Vector3 outputDirection, Vector3 upwardsDirection, Transform printerTransform)
    {
        Vector3 startPos = printerTransform.position;
        var trackPrinterRotation = Quaternion.LookRotation(outputDirection, upwardsDirection);// Quaternion.FromToRotation(Vector3.forward, outputDirection); // The rotation of the TrackPrinter in WorldCoordinates.
        DeleteTrackPreview();
        _trackBuilderSegment = new GameObject("Track Segment").AddComponent<TrackSegment>();
        _trackBuilderSegment.transform.SetPositionAndRotation(startPos + TrackPrefabManager.GetVectorFromPivotToCenterBottom(type, printerTransform), trackPrinterRotation);
        var singlePieceLength = TrackPrefabManager.GetLengthOfTrackPiece(type);
        var singlePieceRotation = TrackPrefabManager.GetRotationOfTrackPiece(type);
        for (var i = 0; i < numberOfElements; i++)
        {
            TrackPiece original;
            // Use the correct piece for type and whether in the middle or at the end.
            if (i == 0 || i == numberOfElements - 1)
                original = prefabManager.GetTrackPrefabsForType(type).EndPieces;
            else
                original = prefabManager.GetTrackPrefabsForType(type).Middle;
            var trackPiece = GameObject.Instantiate(original, _trackBuilderSegment.transform);
            trackPiece.name = $"New TrackPiece {i}";
            // Use the correct position and rotation logic according to type and end/middle
            var rotation = GetPieceRotation(outputDirection, upwardsDirection, trackPrinterRotation, i, singlePieceRotation, numberOfElements);
            var position = GetPiecePosition(outputDirection, upwardsDirection, i, singlePieceLength, singlePieceRotation, numberOfElements);
            // End pieces for the curve
            trackPiece.transform.SetPositionAndRotation(position, rotation);
            _trackBuilderSegment.trackPieces.Add(trackPiece);
        }
    }

    private Vector3 GetPiecePosition(Vector3 outputDirection, Vector3 upwardsDirection, int i, float singlePieceLength, float singlePieceRotation, int numberOfElements)
    {
        var position = type switch
        {
            TrackType.Straight => _trackBuilderSegment.transform.position + i * singlePieceLength * outputDirection,
            TrackType.Left => _trackBuilderSegment.transform.position,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (type == TrackType.Left && i == numberOfElements - 1)
        {
            // Curves end pieces need special handling
            position += outputDirection.RotateAround(upwardsDirection, (i - 1) * singlePieceRotation).normalized *
                        singlePieceLength;
        }
        return position;
    }

    private Quaternion GetPieceRotation(Vector3 outputDirection, Vector3 upwardsDirection, Quaternion trackPrinterRotation,
        int i, float singlePieceRotation, int numberOfElements)
    {
        if (type == TrackType.Left && i == numberOfElements - 1)
        {
            // The last element should not be rotated again, as it is a straight piece
            i--;
        }
        var rotation = type switch
        {
            TrackType.Straight => trackPrinterRotation,
            TrackType.Left => Quaternion.LookRotation(
                outputDirection.RotateAround(upwardsDirection, i * singlePieceRotation), upwardsDirection),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return rotation;
    }

    public SnappingObjWrapper PrintCurrentTrack()
    {
        
        if (_lastNumberOfElements < TrackPrefabManager.GetMinimumNumberOfPieces(type))
        {
            return null;
        }
        var currentTrack = _trackBuilderSegment;
        _trackBuilderSegment = null;
        return FinishTrackPiece(currentTrack);
    }

    private SnappingObjWrapper FinishTrackPiece(TrackSegment segment)
    {
        AddSnappingPoints(segment);
        var wrapper = PackSegmentInWrapper(segment);
        MakeInteractable(segment, wrapper);
        return wrapper;
    }

    private void MakeInteractable(TrackSegment trackSegment, SnappingObjWrapper wrapper)
    {
        var centerPiece = trackSegment.GetMiddleTrackPiece;
        var grabInteractable = wrapper.gameObject.AddComponent<XRGrabInteractable>();
        var rigidbody = grabInteractable.gameObject.GetComponent<Rigidbody>();
        var attachTransformGO = new GameObject("AttachTransform");
        attachTransformGO.transform.parent = wrapper.transform;
        attachTransformGO.transform.position = centerPiece.transform.position;
        rigidbody.isKinematic = true;
        grabInteractable.attachTransform = attachTransformGO.transform;
        grabInteractable.smoothPosition = true;
        grabInteractable.smoothRotation = true;
        // Events for when a piece is interacted with:
        grabInteractable.selectEntered.AddListener(_ =>
        {
            Debug.Log("Grabbed a Segment which should now be selected and snapping to other segments.");
            MoveObjectController.Instance.SelectAnObject(wrapper);
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

        var firstTpNew = GameObject.Instantiate(trackPiecePrefabs.First);
        firstTpNew.transform.position = firstTrackPieceToReplaceTF.position;
        firstTpNew.transform.rotation = firstTrackPieceToReplaceTF.rotation;
        firstTpNew.transform.parent = track.transform;
        
        var lastTpNew = GameObject.Instantiate(trackPiecePrefabs.Last);
        lastTpNew.transform.position = lastTrackPieceToReplaceTF.position;
        lastTpNew.transform.rotation = lastTrackPieceToReplaceTF.rotation;
        lastTpNew.transform.parent = track.transform;

        track.trackPieces.Remove(firstTrackPieceToReplace);
        track.trackPieces.Remove(lastTrackPieceToReplace);
        GameObject.DestroyImmediate(firstTrackPieceToReplace.gameObject);
        GameObject.DestroyImmediate(lastTrackPieceToReplace.gameObject);
    }

    private SnappingObjWrapper PackSegmentInWrapper(TrackSegment track)
    {
        var wrapperGO = new GameObject($"Track Piece {_currTrackID++}");
        wrapperGO.transform.position = track.GetMiddleTrackPiece.transform.position;
        var wrapper = wrapperGO.AddComponent<SnappingObjWrapper>();
        track.transform.parent = wrapper.objToSnap.transform;
        wrapper.UpdateAnchors(collectAnchors: true);
        return wrapper;
    }


    private void DeleteTrackPreview()
    {
        if (_trackBuilderSegment == null) return;
        _trackBuilderSegment.trackPieces.Clear();
        GameObject.Destroy(_trackBuilderSegment.gameObject);
    }

    public void DestroyYourself()
    {
        DeleteTrackPreview();
    }
}