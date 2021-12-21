using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrackBuilder
{
    
    public TrackType type { get; }
    
    void OnDrag(Transform trackPrinter, Transform draggable);
    
    
}
