using Copy;
using Snapping;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TrackPrinting;

public static class TrackFinisher
{
    private static int _currTrackID = 0;

    private static TrackPrefabManager prefabManager => TrackPrefabManager.instance;

    /// <summary>
    /// When a TrackPiece is being dragged out of the TrackBuilder and cut off, it is finished after this call.
    /// - Adds a Wrapper for Snapping
    /// - Makes it grabbable for VR
    /// - Makes it throwable in trash
    /// - Makes it copyable
    /// </summary>
    /// <param name="segment">The <code>TrackSegment</code> with the correct number of pieces to change into an actual TrackSegment</param>
    /// <returns></returns>
    internal static SnappingObjWrapper FinishTrackPiece(TrackSegment segment)
    {
        AddSnappingPoints(segment);
        var wrapper = PackSegmentInWrapper(segment);
        var interactable = MakeInteractable(segment, wrapper);
        MakeThrowableInTrash(wrapper, interactable);
        MakeCopyable(wrapper);
        return wrapper;
    }

    private static void MakeCopyable(SnappingObjWrapper wrapper)
    {
        wrapper.gameObject.AddComponent<Copyable>();
    }

    private static void MakeThrowableInTrash(SnappingObjWrapper wrapper, XRGrabInteractable xrGrabInteractable)
    {
        var throwableInTrash = wrapper.gameObject.AddComponent<ThrowableInTrash>();
        xrGrabInteractable.selectExited.AddListener(arg0 =>
        {
            throwableInTrash.OnLetGo();
        });
    }

    private static XRGrabInteractable MakeInteractable(TrackSegment trackSegment, SnappingObjWrapper wrapper)
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
        wrapper.AddGrabListeners(grabInteractable);
        return grabInteractable;
    }

    /// <summary>
    /// Replaces the TrackPieces at the ends with TrackPieces with an Anchor.
    /// </summary>
    /// <param name="track">The segment to work on.</param>
    private static void AddSnappingPoints(TrackSegment track)
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

    private static SnappingObjWrapper PackSegmentInWrapper(TrackSegment track)
    {
        var wrapperGO = new GameObject($"Track Piece {_currTrackID++}");
        wrapperGO.transform.position = track.GetMiddleTrackPiece.transform.position;
        var wrapper = wrapperGO.AddComponent<SnappingObjWrapper>();
        track.transform.parent = wrapper.objToSnap.transform;
        wrapper.UpdateAnchors(collectAnchors: true);
        return wrapper;
    }
}