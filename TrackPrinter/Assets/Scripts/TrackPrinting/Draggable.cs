#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class Draggable : MonoBehaviour
{
    private const float RetractingSpeed = 1f;

    public TrackPrinter? trackPrinter { get; set; }

    public TrackType selectedType { get; set; } = TrackType.Straight;

    public TrackBuilder? currentTrackBuilder { get; private set; }

    public DraggableState currentState { get; private set; }

    public enum DraggableState { PulledIn, Grabbed, Retracting }

    public bool isGrabbed => currentState == DraggableState.Grabbed;
    
    
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case DraggableState.PulledIn:
                StartGrab(); // TODO only for DEBUG, until we have input system and VR.
                break;
            case DraggableState.Grabbed:
                DragToLocation(transform.position); // TODO only for DEBUG, until we have input system and VR.
                break;
            case DraggableState.Retracting:
                Retract();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
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
            transform.position = goalPosition;
            SwitchFromRetractingToPulledIn();
        }
        else
        {
            var transform = this.transform;
            transform.position += totalDistance.normalized * currMoveDistance;
            currentTrackBuilder?.OnDrag(trackPrinter.transform, transform);
        }
    }

    private void SwitchFromRetractingToPulledIn()
    {
        currentState = DraggableState.PulledIn;
        currentTrackBuilder?.DestroyYourself();
    }


    public void StartGrab()
    {
        Debug.Assert(trackPrinter != null, nameof(trackPrinter) + " != null");
        Debug.Assert(!isGrabbed, "Draggable is already Grabbed!");

        currentTrackBuilder?.DestroyYourself();
        if (currentTrackBuilder == null) // If we were retracting before and still have one, we can just keep it.
        {
            currentTrackBuilder = new TrackBuilder(trackPrinter!.selectedType, trackPrinter.transform.position, this.transform.position, this);
        }

        currentState = DraggableState.Grabbed;
    }

    public void LetGo()
    {
        if (!isGrabbed) throw new InvalidOperationException("Has to be Grabbed to let go");
        
        // TODO move Draggable to the point in front of the current track, to always retract it at the same speed.

        currentState = DraggableState.Retracting;
    }
    

    public void DragToLocation(Vector3 newLocation)
    {
        Debug.Assert(trackPrinter != null, nameof(trackPrinter) + " != null");

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

}