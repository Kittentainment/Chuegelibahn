using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    [SerializeField] private TrackType _type;
    public TrackType type => _type;
}
