using System.Collections.Generic;
using UnityEngine;

namespace TrackPrinting
{
    /// <summary>
    /// A Segment is a collection of TrackPieces which are printed together and appears to the user as one single entity
    /// </summary>
    public class TrackSegment : MonoBehaviour
    {

        public List<TrackPiece> trackPieces = new List<TrackPiece>();

        public TrackPiece GetMiddleTrackPiece => trackPieces[trackPieces.Count / 2];

    }

}