#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using DebugAssert = System.Diagnostics.Debug;

public class Draggable : MonoBehaviour
{
    private const float RetractingSpeed = 1f;

    public TrackPrinter?  trackPrinter { get; set; }
    
    public XRGrabInteractable XRGrabInteractable { get; private set; }
    
    public TrackType selectedType => trackPrinter!.selectedType;

    public TrackBuilder? currentTrackBuilder { get; private set; }

    public DraggableState currentState { get; private set; }

    public enum DraggableState { Waiting, Grabbed, Retracting }

    public bool isGrabbed => currentState == DraggableState.Grabbed;

    private void Awake()
    {
        XRGrabInteractable = GetComponent<XRGrabInteractable>();
        if (XRGrabInteractable == null)
            throw new MissingComponentException("No XRGrabInteractable on Draggable");
    }

    private void Start()
    {
        GoIntoWaiting();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case DraggableState.Waiting:
                StartGrab(); // TODO only for DEBUG, until we have input system and VR.
                break;
            case DraggableState.Grabbed:
                DragToLocation(transform.position); // TODO find a better way to continuously check the position while it is grabbed.
                break;
            case DraggableState.Retracting:
                Retract();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
    
    
    public void OnSelected()
    {
        Debug.Log("Grabbed TrackPrinter");
        StartGrab();
    }
    
    public void OnDeselected()
    {
        Debug.Log("Let go of TrackPrinter");
        LetGo();
    }
    

    /// <summary>
    /// Retracts this Draggable back to its TrackPrinter.
    /// </summary>
    private void Retract()
    {
        Debug.Assert(trackPrinter != null, nameof(trackPrinter) + " != null");
        var goalPosition = trackPrinter!.transform.position;
        var currMoveDistance = Time.deltaTime * RetractingSpeed;
        var totalDistance = goalPosition - transform.position;
        if (currMoveDistance > totalDistance.magnitude)
        {
            // Now we have reached the track printer. Wait again.
            transform.position = goalPosition;
            transform.rotation = trackPrinter!.transform.rotation; // TODO rotate slowly back.
            SwitchFromRetractingWaiting();
        }
        else
        {
            // Retract back towards the printer. (Pulling in)
            var transform = this.transform;
            transform.position += totalDistance.normalized * currMoveDistance;
            currentTrackBuilder?.OnDrag(trackPrinter.transform, transform);
        }
    }

    private void GoIntoWaiting()
    {
        transform.position = trackPrinter!.transform.position;
        transform.rotation = trackPrinter!.transform.rotation;
        currentState = DraggableState.Waiting;
        transform.parent = trackPrinter!.transform;
    }
    
    private void SwitchFromRetractingWaiting()
    {
        currentTrackBuilder?.DestroyYourself();
        currentTrackBuilder = null;
        GoIntoWaiting();
    }


    public void StartGrab()
    {
        DebugAssert.Assert(trackPrinter != null, nameof(trackPrinter) + " != null");
        DebugAssert.Assert(!isGrabbed, "Draggable is already Grabbed!");

        if (currentTrackBuilder == null) // If we were retracting before and still have one, we can just keep it.
        {
            currentTrackBuilder = new TrackBuilder(trackPrinter!.selectedType, trackPrinter.transform.position, this.transform.position, this);
        }

        transform.parent = null;

        currentState = DraggableState.Grabbed;
    }

    public void LetGo()
    {
        if (!isGrabbed) Debug.LogWarning("Has to be Grabbed to let go");
        
        // TODO move Draggable to the point in front of the current track, to always retract it at the same speed.

        currentState = DraggableState.Retracting;
    }
    

    public void DragToLocation(Vector3 newLocation)
    {
        // TODO Add max distance (maybe or just max elements in the TrackBuilder)
        DebugAssert.Assert(trackPrinter != null, nameof(trackPrinter) + " != null");

        transform.position = newLocation;
        if (isGrabbed == false || currentTrackBuilder == null)
        {
            throw new InvalidOperationException("Can't use DragToLocation if Draggable is not grabbed!");
        }
        else
        {
            currentTrackBuilder.OnDrag(trackPrinter!.transform, this.transform);
        }
    }

    public void PrintCurrentTrack()
    {
        DebugAssert.Assert(trackPrinter != null, nameof(trackPrinter) + " != null");
        
        currentTrackBuilder!.PrintCurrentTrack();

        ResetPosition();
        currentState = DraggableState.Waiting;
        
        currentTrackBuilder!.DestroyYourself();
        currentTrackBuilder = null;
    }

    private void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }
}